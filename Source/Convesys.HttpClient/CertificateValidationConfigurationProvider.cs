using Convesys.Kernel.Security.Configuration;
using Convesys.Kernel.Security.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convesys.Platform.Web.HttpClient
{
    public class CertificateValidationConfigurationProvider : ICertificateValidationConfigurationProvider
    {
        public BackchannelConfiguration GeBackchannelConfiguration(string federationPartyId)
        {
            return new BackchannelConfiguration
            {
                UsePinningValidation = false
            };
        }

        public BackchannelConfiguration GeBackchannelConfiguration(Uri partyUri)
        {
            return new BackchannelConfiguration
            {
                UsePinningValidation = false
            };
        }

        public CertificateValidationConfiguration GetConfiguration(string federationPartyId)
        {
            return new CertificateValidationConfiguration();
        }

        public void Dispose()
        {
        }
    }
}
