//using System.Net.Http;
using Convesys.Common.Serialisation.JSON;
using Convesys.Common.Serialisation.JSON.SettingsProviders;
using Convesys.Kernel.Caching;
using Convesys.Kernel.DependencyResolver;
using Convesys.Kernel.Serialisation;
using Convesys.Kernel.Web;
using Convesys.Kernel.Web.Authorisation;
using Convesys.MemoryCacheProvider;
using Convesys.Platform.Web.Tokens;
using Newtonsoft.Json;
using System;

namespace Convesys.Platform.Web.Api.Client.Extensions
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