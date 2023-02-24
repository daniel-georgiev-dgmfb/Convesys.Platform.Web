using System;
using Twilight.Kernel.DependencyResolver;
using Twilight.Kernel.Web.Authorisation;


namespace Twilight.Platform.Web.Tokens.Extensions
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