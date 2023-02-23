using Twiligth.Kernel.Web;
using Twiligth.Kernel.Web.Authorisation;
using Kernel.Cryptography.DataProtection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Twiligth.Platform.Web.Tokens.Contexts
{
    public class ClientSecretTokenContext : IBearerTokenContext
    {
        private readonly string _clientId;
        private readonly string _secret;
        public ClientSecretTokenContext(string clientId, string secret, Endpoint endpoint)
        {
            if (String.IsNullOrWhiteSpace(clientId))
                throw new ArgumentNullException(nameof(clientId));
            if (String.IsNullOrWhiteSpace(secret))
                throw new ArgumentNullException(nameof(secret));
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint));
            this._clientId = clientId;
            this._secret = secret;
            this.Endpoint = endpoint;
        }

        public HttpContent Content
        {
            get
            {
                return new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", this.GrantType),
                });
            }
        }

        public string GrantType { get { return "client_credentials"; } }

        public Endpoint Endpoint { get; }

        public void HeaderHandler(HttpRequestHeaders headers)
        {
            var base64Credencial = Convert.ToBase64String(Encoding.UTF8.GetBytes(String.Format("{0}:{1}", this._clientId, this._secret)));
            headers.Authorization = new AuthenticationHeaderValue("Basic", base64Credencial);
        }

        public string ContextKey()
        {
            var userNameBytes = Encoding.UTF8.GetBytes(this._clientId);
            var encryptor = new PasswordEncryptor();
            var keyBytes = encryptor.GetDeriveBytes(this._secret, 1000, userNameBytes, 256);
            return Convert.ToBase64String(keyBytes);
        }
    }
}