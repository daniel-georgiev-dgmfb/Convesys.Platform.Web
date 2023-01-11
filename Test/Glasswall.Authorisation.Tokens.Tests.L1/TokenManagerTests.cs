using Convesys.Common.Serialisation.JSON;
using Convesys.Common.Serialisation.JSON.SettingsProviders;
using Convesys.Kernel.Logging;
using Convesys.Kernel.Security.Validation;
using Convesys.Kernel.Web.Authorisation;
using Convesys.Platform.Web.Tokens;
using Convesys.Platform.Web.Tokens.Contexts;
using Moq;
using NUnit.Framework;
using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Convesys.Authorisation.Tokens.Tests.L1
{
    [TestFixture]
    [Category("Convesys.Authorisation.Tokens.Tests.L1")]
    public class TokenManagerTests
    {
        [Test]
        public async Task When_get_token_with_right_credencials()
        {
            //ARRANGE
            var uri = new Uri("https://cas.wotsits.filetrust.io/Connect/Token");
            var httplogger = new Mock<IEventLogger<Convesys.Platform.Web.HttpClient.HttpClient>>();
            var logger = new Mock<IEventLogger<TokenManager>>();
            var defaultSettingsProvider = new DefaultSettingsProvider();
            var jsonSerializer = new NSJsonSerializer(defaultSettingsProvider);
            var parser = new BearerTokenParser(jsonSerializer);
            var cache = new MemoryCacheRuntimeImplementor();
            var sertificateValidator = new Mock<IBackchannelCertificateValidator>();
            sertificateValidator.Setup(x => x.Validate(It.IsAny<object>(), It.IsAny<X509Certificate>(), It.IsAny<X509Chain>(), It.IsAny<SslPolicyErrors>()))
                .Returns(true);
            var httpClient = new Convesys.Platform.Web.HttpClient.HttpClient(sertificateValidator.Object, httplogger.Object);
            var tokenManager = new TokenManager(httpClient, cache, parser, logger.Object);
            var context = new ResoureOwnerTokenContext("john.doe@domain.com", "Password_1", new Kernel.Web.Endpoint(uri.AbsoluteUri));
            var foo = new System.Net.Http.HttpClient();
            
            //ACT
            var token = await tokenManager.GetToken(context, CancellationToken.None);
            //ASSERT
            Assert.IsInstanceOf<TokenDescriptor>(token);
        }

        [Test]
        public async Task When_get_token_with_wrong_credencials()
        {
            //ARRANGE
            var uri = new Uri("https://cas.wotsits.filetrust.io/Connect/Token");
            var httplogger = new Mock<IEventLogger<Convesys.Platform.Web.HttpClient.HttpClient>>();
            var logger = new Mock<IEventLogger<TokenManager>>();
            var defaultSettingsProvider = new DefaultSettingsProvider();
            var jsonSerializer = new NSJsonSerializer(defaultSettingsProvider);
            var parser = new BearerTokenParser(jsonSerializer);
            var cache = new MemoryCacheRuntimeImplementor();
            var sertificateValidator = new Mock<IBackchannelCertificateValidator>();
            sertificateValidator.Setup(x => x.Validate(It.IsAny<object>(), It.IsAny<X509Certificate>(), It.IsAny<X509Chain>(), It.IsAny<SslPolicyErrors>()))
                .Returns(true);
            var httpClient = new Convesys.Platform.Web.HttpClient.HttpClient(sertificateValidator.Object, httplogger.Object);
            var tokenManager = new TokenManager(httpClient, cache, parser, logger.Object);
            var context = new ResoureOwnerTokenContext("john.doe@domain.com", "Password1", new Kernel.Web.Endpoint(uri.AbsoluteUri));
            var foo = new System.Net.Http.HttpClient();

            //ACT
            var token = await tokenManager.GetToken(context, CancellationToken.None);
            //ASSERT
            Assert.IsNull(token);
        }

        [Test]
        public async Task When_get_token_with_right_credencials_writes_in_cache()
        {
            //ARRANGE
            var readFromCache = false;
            var writeToCache = false;
            var uri = new Uri("https://cas.wotsits.filetrust.io/Connect/Token");
            var httplogger = new Mock<IEventLogger<Convesys.Platform.Web.HttpClient.HttpClient>>();
            var logger = new Mock<IEventLogger<TokenManager>>();
            var defaultSettingsProvider = new DefaultSettingsProvider();
            var jsonSerializer = new NSJsonSerializer(defaultSettingsProvider);
            var parser = new BearerTokenParser(jsonSerializer);
            var cache = new MemoryCacheRuntimeImplementor();
            cache.ReadFrom += new EventHandler((_, ev) =>
            {
                var cacheEvent = ev as CacheEvent;
                if (cacheEvent == null || cacheEvent.Entry == null)
                    return;

                readFromCache = true;
            });
            cache.WrittenTo += new EventHandler((_, __) => { writeToCache = true; });
            var sertificateValidator = new Mock<IBackchannelCertificateValidator>();
            sertificateValidator.Setup(x => x.Validate(It.IsAny<object>(), It.IsAny<X509Certificate>(), It.IsAny<X509Chain>(), It.IsAny<SslPolicyErrors>()))
                .Returns(true);
            var httpClient = new Convesys.Platform.Web.HttpClient.HttpClient(sertificateValidator.Object, httplogger.Object);
            var tokenManager = new TokenManager(httpClient, cache, parser, logger.Object);
            var context = new ClientSecretTokenContext("service", "Glasswall", new Kernel.Web.Endpoint(uri.AbsoluteUri));
            var foo = new System.Net.Http.HttpClient();

            //ACT
            var token = await tokenManager.GetToken(context, CancellationToken.None);
            //ASSERT
            Assert.IsFalse(readFromCache);
            Assert.IsTrue(writeToCache);
            Assert.IsInstanceOf<TokenDescriptor>(token);
        }

        [Test]
        public async Task When_get_token_with_right_credencials_reads_from_cache()
        {
            //ARRANGE
            var readFromCache = false;
            var writeToCache = false;
            var uri = new Uri("https://cas.wotsits.filetrust.io/Connect/Token");
            var httplogger = new Mock<IEventLogger<Convesys.Platform.Web.HttpClient.HttpClient>>();
            var logger = new Mock<IEventLogger<TokenManager>>();
            var defaultSettingsProvider = new DefaultSettingsProvider();
            var jsonSerializer = new NSJsonSerializer(defaultSettingsProvider);
            var parser = new BearerTokenParser(jsonSerializer);
            var cache = new MemoryCacheRuntimeImplementor();
            cache.ReadFrom += new EventHandler((_, ev) =>
            {
                var cacheEvent = ev as CacheEvent;
                if (cacheEvent == null || cacheEvent.Entry == null)
                    return;

                readFromCache = true;
            });
            cache.WrittenTo += new EventHandler((_, __) => { writeToCache = true; });
            var sertificateValidator = new Mock<IBackchannelCertificateValidator>();
            sertificateValidator.Setup(x => x.Validate(It.IsAny<object>(), It.IsAny<X509Certificate>(), It.IsAny<X509Chain>(), It.IsAny<SslPolicyErrors>()))
                .Returns(true);
            var httpClient = new Convesys.Platform.Web.HttpClient.HttpClient(sertificateValidator.Object, httplogger.Object);
            var tokenManager = new TokenManager(httpClient, cache, parser, logger.Object);
            var context = new ResoureOwnerTokenContext("john.doe@domain.com", "Password_1", new Kernel.Web.Endpoint(uri.AbsoluteUri));
            var foo = new System.Net.Http.HttpClient();

            //ACT
            var token = await tokenManager.GetToken(context, CancellationToken.None);
            await Task.Delay(500);
            writeToCache = false;
            token = await tokenManager.GetToken(context, CancellationToken.None);

            //ASSERT
            Assert.IsTrue(readFromCache);
            Assert.IsFalse(writeToCache);
            Assert.IsInstanceOf<TokenDescriptor>(token);
        }

        [Test]
        public async Task When_get_token_with_right_credencials_does_not_writes_in_cache()
        {
            //ARRANGE
            var readFromCache = false;
            var writeToCache = false;
            var uri = new Uri("https://cas.wotsits.filetrust.io/Connect/Token");
            var httplogger = new Mock<IEventLogger<Convesys.Platform.Web.HttpClient.HttpClient>>();
            var logger = new Mock<IEventLogger<TokenManager>>();
            var defaultSettingsProvider = new DefaultSettingsProvider();
            var jsonSerializer = new NSJsonSerializer(defaultSettingsProvider);
            var parser = new BearerTokenParser(jsonSerializer);
            var cache = new MemoryCacheRuntimeImplementor();
            cache.ReadFrom += new EventHandler((_, ev) =>
            {
                var cacheEvent = ev as CacheEvent;
                if (cacheEvent == null || cacheEvent.Entry == null)
                    return;

                readFromCache = true;
            });
            cache.WrittenTo += new EventHandler((_, __) => { writeToCache = true; });
            var sertificateValidator = new Mock<IBackchannelCertificateValidator>();
            sertificateValidator.Setup(x => x.Validate(It.IsAny<object>(), It.IsAny<X509Certificate>(), It.IsAny<X509Chain>(), It.IsAny<SslPolicyErrors>()))
                .Returns(true);
            var httpClient = new Convesys.Platform.Web.HttpClient.HttpClient(sertificateValidator.Object, httplogger.Object);
            var tokenManager = new TokenManager(httpClient, cache, parser, logger.Object);
            var context = new ResoureOwnerTokenContext("john.doe@domain.com", "Password1", new Kernel.Web.Endpoint(uri.AbsoluteUri));
            var foo = new System.Net.Http.HttpClient();

            //ACT
            var token = await tokenManager.GetToken(context, CancellationToken.None);
            //ASSERT
            Assert.IsFalse(readFromCache);
            Assert.IsFalse(writeToCache);
            Assert.IsNull(token);
        }
    }
}