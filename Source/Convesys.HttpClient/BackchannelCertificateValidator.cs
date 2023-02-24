using System;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Twilight.Kernel.Logging;
using Twilight.Kernel.Security.Validation;
using Twilight.Kernel.Web;
using Twilight.Platform.Cryptography.Certificates.Backchannel.Validation;

namespace Twiligth.Platform.Web.HttpClient
{
    /// <summary>
    /// Backchannel certificate validator. Validates remote certificate for https.
    /// Perform validation as follows: Locates pinning validators if enabled and invoke all in chain.
    /// if validated no more validation is perfrmed, run custom validation rules otherwise
    /// </summary>
    public class BackchannelCertificateValidator : IBackchannelCertificateValidator
    {
        private readonly IEventLogger<BackchannelCertificateValidator> _logProvider;
        private readonly ICertificateValidationConfigurationProvider _configurationProvider;
        public BackchannelCertificateValidator(ICertificateValidationConfigurationProvider configurationProvider, IEventLogger<BackchannelCertificateValidator> logProvider)
        {
            if (configurationProvider == null)
                throw new ArgumentNullException("configurationProvider");
            if (logProvider == null)
                throw new ArgumentNullException(nameof(logProvider));

            this._logProvider = logProvider;
            this._configurationProvider = configurationProvider;
        }

        public virtual bool Validate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            var httpMessage = sender as HttpRequestMessage;

            this._logProvider.Log(SeverityLevel.Info, 0, String.Format("Validating backhannel certificate. sslPolicyErrors was: {0}", sslPolicyErrors), null, (o, e) => e.Message);

            var configiration = httpMessage != null ?
                this._configurationProvider.GeBackchannelConfiguration(httpMessage.RequestUri)
                : this._configurationProvider.GeBackchannelConfiguration(String.Empty);
            var context = new BackchannelCertificateValidationContext(certificate, chain, sslPolicyErrors);

            //if pinning validation is enabled it takes precedence
            if (configiration.UsePinningValidation && configiration.BackchannelValidatorResolver != null)
            {
                this._logProvider.Log(SeverityLevel.Info, 0, String.Format("Pinning validation entered. Validator type: {0}", configiration.BackchannelValidatorResolver.Type), null, (o, e) => e.Message);

                var type = configiration.BackchannelValidatorResolver.Type;
                var instance = BackchannelCertificateValidationRulesFactory.CertificateValidatorResolverFactory(type);
                if (instance != null)
                {
                    var validators = instance.Resolve(configiration)
                        .Where(x => x != null)
                        .ToList();

                    Func<object, BackchannelCertificateValidationContext, Task> seed1 = (o, c) => Task.CompletedTask;
                    var del = validators.Aggregate(seed1, (next, validator) => new Func<object, BackchannelCertificateValidationContext, Task>((o, c) => validator.Validate(o, c, next)));
                    var backChannelValidationTask = del(sender, context);
                    backChannelValidationTask.Wait();
                    return context.IsValid;
                }
            }

            //if pinning validation is disabled run validation rules if any
            //default rule. SslPolicyErrors no error vaidation.
            Func<BackchannelCertificateValidationContext, Task> seed = x =>
            {
                var isLocal = httpMessage != null && Utility.IsLocalIpAddress(httpMessage.RequestUri.Host);
                if (isLocal || (!x.IsValid && x.SslPolicyErrors == SslPolicyErrors.None))
                    x.Validated();
                return Task.CompletedTask;
            };

            var rules = BackchannelCertificateValidationRulesFactory.GetRules(configiration) ?? Enumerable.Empty<IBackchannelCertificateValidationRule>();
            var validationDelegate = rules.Aggregate(seed, (f, next) => new Func<BackchannelCertificateValidationContext, Task>(c => next.Validate(c, f)));
            var task = validationDelegate(context);
            task.Wait();
            return context.IsValid;
        }
    }
}