using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Twilight.Kernel.Reflection.Reflection;
using Twilight.Kernel.Security.Configuration;
using Twilight.Kernel.Security.Validation;

namespace Twilight.Platform.Cryptography.Certificates.Backchannel.Validation
{
    public class BackchannelCertificateValidationRulesFactory
    {
        static BackchannelCertificateValidationRulesFactory()
        {
            BackchannelCertificateValidationRulesFactory.InstanceCreator = t =>
            {
                var instance = (IBackchannelCertificateValidationRule)Activator.CreateInstance(t);
                return instance;
            };
            BackchannelCertificateValidationRulesFactory.CertificateValidatorResolverFactory = t => (ICertificateValidatorResolver)Activator.CreateInstance(t);
            BackchannelCertificateValidationRulesFactory.AssembliesToScan = new List<Assembly>();
        }
        public static IEnumerable<IBackchannelCertificateValidationRule> GetRules(BackchannelConfiguration configuration)
        {
            var rules = Enumerable.Empty<IBackchannelCertificateValidationRule>();
            if (BackchannelCertificateValidationRulesFactory.RulesFactory != null)
            {
                rules = BackchannelCertificateValidationRulesFactory.RulesFactory();
                if (rules != null)
                    return rules;
            }
            if (BackchannelCertificateValidationRulesFactory.AssembliesToScan.Count == 0)
            {
                AssemblyScanner.ScannableAssemblies.Aggregate(BackchannelCertificateValidationRulesFactory.AssembliesToScan, (c, next) => { c.Add(next); return c; });
            }
            var types = ReflectionHelper.GetAllTypes(BackchannelCertificateValidationRulesFactory.AssembliesToScan, t =>
            !t.IsAbstract && !t.IsInterface && typeof(IBackchannelCertificateValidationRule).IsAssignableFrom(t));
            rules = types.Select(t => BackchannelCertificateValidationRulesFactory.InstanceCreator(t))
                .Where(x => x != null);
            return rules;
        }

        public static Func<Type, IBackchannelCertificateValidationRule> InstanceCreator { get; set; }

        public static Func<Type, ICertificateValidatorResolver> CertificateValidatorResolverFactory { get; set; }

        public static ICollection<Assembly> AssembliesToScan { get; }

        public static Func<IEnumerable<IBackchannelCertificateValidationRule>> RulesFactory { get; set; }
    }
}