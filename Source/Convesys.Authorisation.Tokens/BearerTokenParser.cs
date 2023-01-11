using Convesys.Kernel.Serialisation;
using Convesys.Kernel.Web.Authorisation;
using System;
using System.Threading.Tasks;

namespace Convesys.Platform.Web.Tokens
{
    public class BearerTokenParser : IBearerTokenParser
    {
        private readonly IJsonSerialiser _serialiser;
        public BearerTokenParser(IJsonSerialiser serialiser)
        {
            if (serialiser == null)
                throw new ArgumentNullException(nameof(serialiser));

            this._serialiser = serialiser;
        }
        public async Task<TokenDescriptor> Parse(string source)
        {
            if (String.IsNullOrWhiteSpace(source))
                throw new InvalidOperationException(String.Format("Empty or null token response"));
            var tokenJson = await this._serialiser.DeserialiseFromJson<Token>(source);
            if (tokenJson == null)
                throw new InvalidOperationException(String.Format("Cannot deserialise response: {0} to type: {1}", tokenJson, typeof(Token).FullName));
            tokenJson.Validate();
            return new TokenDescriptor(tokenJson.token_type, tokenJson.access_token, DateTimeOffset.Now, tokenJson.expires_in);
        }
        
        private class Token
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public uint expires_in { get; set; }
            public void Validate()
            {
                if (String.IsNullOrWhiteSpace(this.access_token))
                    throw new ArgumentNullException(nameof(access_token));
                if (String.IsNullOrWhiteSpace(this.token_type))
                    throw new ArgumentNullException(nameof(token_type));
                if (expires_in <= 0)
                    throw new ArgumentException(String.Format("expires_in must be a positive value greater than zero. It was: {0}", expires_in));
            }
        }
    }
}