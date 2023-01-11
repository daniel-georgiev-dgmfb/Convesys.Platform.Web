using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using Convesys.Kernel.Web;
using Convesys.Kernel.Web.Authorisation;

namespace Convesys.Platform.Web.Tokens.Contexts
{
    public class PrincipalTokenContext : IBearerTokenContext
    {
        private readonly ClaimsPrincipal _cliamPrincipal;
        
        public PrincipalTokenContext(ClaimsPrincipal cliamPrincipal)
        {
            if (cliamPrincipal == null)
                throw new ArgumentNullException(nameof(cliamPrincipal));
            
            this._cliamPrincipal = cliamPrincipal;
        }

        public HttpContent Content
        {
            get
            {
                return null;
            }
        }

        public string GrantType { get { throw new NotImplementedException(); } }

        public Endpoint Endpoint { get; }

        public void HeaderHandler(HttpRequestHeaders headers)
        {
        }

        public string ContextKey()
        {
            var contextKeyClaim = this._cliamPrincipal.Claims.First(x => x.Type == ClaimTypes.Authentication);
            return contextKeyClaim.Value;
        }
    }
}