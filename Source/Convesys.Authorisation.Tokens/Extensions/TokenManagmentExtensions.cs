using System;
using Convesys.Kernel.DependencyResolver;
using Convesys.Kernel.Web.Authorisation;

namespace Convesys.Platform.Web.Tokens.Extensions
{
    public static class TokenManagmentExtensions
    {
        public static IDependencyResolver AddTokenManagement(this IDependencyResolver dependencyResolver)
        {
            if (dependencyResolver == null)
                throw new ArgumentNullException(nameof(dependencyResolver));
            dependencyResolver.RegisterType<IBearerTokenParser, BearerTokenParser>(Lifetime.Transient);
            dependencyResolver.RegisterType<IBearerTokenManager, TokenManager>(Lifetime.Transient);
            return dependencyResolver;
        }
    }
}