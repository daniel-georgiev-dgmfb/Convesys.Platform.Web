using System;
using Twiligth.Kernel.DependencyResolver;
using Twiligth.Kernel.Web.Authorisation;

namespace Twiligth.Platform.Web.Tokens.Extensions
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