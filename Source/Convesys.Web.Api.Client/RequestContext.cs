using System;
using Twilight.Kernel.Web;
using Twilight.Kernel.Web.Authorisation;

namespace Twilight.Platform.Web.Api.Client
{
    public class RequestContext
    {
        public RequestContext(Endpoint resourceEndpoint, IBearerTokenContext clientCredentials = null)
        {
            if (resourceEndpoint == null)
                throw new ArgumentNullException(nameof(resourceEndpoint));

            this.ClientCredentials = clientCredentials;
            this.ResourceEndpoint = resourceEndpoint;
        }

        public Endpoint ResourceEndpoint { get; }
        
        public string Content { get; set; }

        public IBearerTokenContext ClientCredentials { get; }

        public override string ToString()
        {
            return String.Format("Endpoint: {0}", this.ResourceEndpoint.ToString());
        }
    }
}