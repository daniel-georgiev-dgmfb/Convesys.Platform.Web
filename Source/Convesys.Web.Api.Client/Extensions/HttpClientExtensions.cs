//using System.Net.Http;
using Newtonsoft.Json;
using System;
using Twilight.Common.Serialisation.JSON;
using Twilight.Common.Serialisation.JSON.SettingsProviders;
using Twilight.Kernel.Caching;
using Twilight.Kernel.DependencyResolver;
using Twilight.Kernel.Serialisation;
using Twilight.Kernel.Web;
using Twilight.Kernel.Web.Authorisation;
using Twilight.Platform.Web.Api.Client;
using Twilight.Platform.Web.Tokens;

namespace Twilight.Platform.Web.Api.Client.Extensions
{
    public static class HttpClientExtensions
    {
        public static IDependencyResolver AddApiClient(this IDependencyResolver dependencyResolver)
        {
            if (dependencyResolver == null)
                throw new ArgumentNullException(nameof(dependencyResolver));

            if (!dependencyResolver.Contains< IApiClient, ApiClient>())
                dependencyResolver.RegisterType<IApiClient, ApiClient>(Lifetime.Transient);

            if (!dependencyResolver.Contains<IHttpResourceRetriever, HttpClient>())
                dependencyResolver.RegisterType<IHttpResourceRetriever, Twilight.Platform.Web.HttpClient>(Lifetime.Transient);

            if (!dependencyResolver.Contains<IBearerTokenParser, BearerTokenParser>())
                dependencyResolver.RegisterType<IBearerTokenParser, BearerTokenParser>(Lifetime.Transient);

            if (!dependencyResolver.Contains<IBearerTokenManager, TokenManager>())
                dependencyResolver.RegisterType<IBearerTokenManager, TokenManager>(Lifetime.Transient);

            if (!dependencyResolver.Contains<ICacheProvider, MemoryCacheRuntimeImplementor>())
                dependencyResolver.RegisterType<ICacheProvider, MemoryCacheRuntimeImplementor>(Lifetime.Transient);

            if (!dependencyResolver.Contains<IJsonSerialiser, NSJsonSerializer>())
                dependencyResolver.RegisterType<IJsonSerialiser, NSJsonSerializer>(Lifetime.Transient);

            if (!dependencyResolver.Contains<ISerialisationSettingsProvider<JsonSerializerSettings>, DefaultSettingsProvider>())
                dependencyResolver.RegisterType<ISerialisationSettingsProvider<JsonSerializerSettings>, DefaultSettingsProvider>(Lifetime.Transient);

            return dependencyResolver;
        }
    }
}