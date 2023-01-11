using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Convesys.Kernel.Web;
using Convesys.Kernel.Web.Authorisation;
using Kernel.Cryptography.DataProtection;

namespace Convesys.Platform.Web.Tokens.Contexts
{
    public class ResoureOwnerTokenContext : IBearerTokenContext
    {
        private readonly string _username;
        private readonly string _password;
        public ResoureOwnerTokenContext(string username, string password, Endpoint endpoint)
        {
            if (String.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException(nameof(username));
            if (String.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException(nameof(password));
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint));
            this._username = username;
            this._password = password;
            this.Endpoint = endpoint;
        }

        public HttpContent Content
        {
            get
            {
                return new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", this.GrantType),
                    new KeyValuePair<string, string>("username", this._username),
                    new KeyValuePair<string, string>("password", this._password),
                });
            }
        }

        public string GrantType { get { return "password"; } }

        public Endpoint Endpoint { get; }

        public void HeaderHandler(HttpRequestHeaders headers)
        {
        }

        public string ContextKey()
        {
            var userNameBytes = Encoding.UTF8.GetBytes(this._username);
            var encryptor = new PasswordEncryptor();
            var keyBytes = encryptor.GetDeriveBytes(this._password, 1000, userNameBytes, 256);
            return Convert.ToBase64String(keyBytes);
        }
    }
}