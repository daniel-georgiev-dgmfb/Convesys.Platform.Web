using System;
using Twiligth.Kernel.DependencyResolver;
using Twiligth.Kernel.Web;

namespace Twiligth.Platform.Web.HttpClient.Extensions
{
    public static class HttpClientExtensions
    {
        public static IDependencyResolver AddHttpClient(this IDependencyResolver dependencyResolver)
        {
            if (dependencyResolver == null)
                throw new ArgumentNullException(nameof(dependencyResolver));

            dependencyResolver.RegisterType<IHttpResourceRetriever, HttpClient>(Lifetime.Transient);
            return dependencyResolver;
        }
    }
}