using Convesys.Kernel.Configuration;
using Convesys.Kernel.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convesys.HttpClient.Tests.L0.MockData
{
    internal class ResourceRetrieverCustomConfigurator : ICustomConfigurator<IHttpResourceRetriever>
    {
        public void Configure(IHttpResourceRetriever configurable)
        {
            configurable.RequireHttps = false;
            configurable.Timeout = TimeSpan.FromSeconds(10);
        }
    }
}
