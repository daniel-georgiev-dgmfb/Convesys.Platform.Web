//using System.Net.Http;
using Twiligth.Common.Serialisation.JSON;
using Twiligth.Common.Serialisation.JSON.SettingsProviders;
using Twiligth.Kernel.Caching;
using Twiligth.Kernel.DependencyResolver;
using Twiligth.Kernel.Serialisation;
using Twiligth.Kernel.Web;
using Twiligth.Kernel.Web.Authorisation;
using Twiligth.MemoryCacheProvider;
using Twiligth.Platform.Web.Tokens;
using Newtonsoft.Json;
using System;

namespace Twiligth.Platform.Web.Api.Client.Extensions
{
    public static class HttpClientExtensions
    {
        public static IDependencyResolver AddApiClient(this IDependencyResolver dependencyResolver)
        {
            if (dependencyResolver == null)
                throw new ArgumentNullException(nameof(dependencyResolver));

            if (!dependencyResolver.Contains< IApiClient, ApiClient>())
                dependencyResolver.RegisterType<IApiClient, ApiClient>(Lifetime.Transient);

            if (!dependencyResolver.Contains<IHttpResourceRetriever, HttpClient.HttpClient>())
                dependencyResolver.RegisterType<IHttpResourceRetriever, HttpClient.HttpClient>(Lifetime.Transient);

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