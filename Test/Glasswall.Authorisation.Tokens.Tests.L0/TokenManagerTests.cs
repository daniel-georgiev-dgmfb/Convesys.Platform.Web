using Convesys.Kernel.Caching;
using Convesys.Kernel.Logging;
using Convesys.Kernel.Web;
using Convesys.Kernel.Web.Authorisation;
using Convesys.MemoryCacheProvider;
using Convesys.Platform.Web.Tokens;
using Moq;
using NUnit.Framework;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Convesys.Authorisation.Tokens.Tests.L0
{
    [TestFixture]
    [Category("Convesys.Authorisation.Tokens.Tests.L0")]
    internal class TokenManagerTests
    {
        [Test]
        public void TokenManager_null_resource_retreiver()
        {
            //ARRANGE
            var httpResourceRetriever = new Mock<IHttpResourceRetriever>();
            var cacheProvider = new Mock<ICacheProvider>();
            var bearerTokenParser = new Mock<IBearerTokenParser>();
            var context = new Mock<IBearerTokenContext>();
            var logger = new Mock<IEventLogger<TokenManager>>();

            //ACT
            //ASSERT
            Assert.Throws<ArgumentNullException>(() => new TokenManager(null, cacheProvider.Object, bearerTokenParser.Object, logger.Object));
        }
        
        [Test]
        public void TokenManager_null_cache_provider()
        {
            //ARRANGE
            var httpResourceRetriever = new Mock<IHttpResourceRetriever>();
            var cacheProvider = new Mock<ICacheProvider>();
            var bearerTokenParser = new Mock<IBearerTokenParser>();
            var context = new Mock<IBearerTokenContext>();
            var logger = new Mock<IEventLogger<TokenManager>>();

            //ACT
            //ASSERT
            Assert.Throws<ArgumentNullException>(() => new TokenManager(httpResourceRetriever.Object, null, bearerTokenParser.Object, logger.Object));
        }

        [Test]
        public void TokenManager_null_token_parser()
        {
            //ARRANGE
            var httpResourceRetriever = new Mock<IHttpResourceRetriever>();
            var cacheProvider = new Mock<ICacheProvider>();
            var bearerTokenParser = new Mock<IBearerTokenParser>();
            var context = new Mock<IBearerTokenContext>();
            var logger = new Mock<IEventLogger<TokenManager>>();

            //ACT
            //ASSERT
            Assert.Throws<ArgumentNullException>(() => new TokenManager(httpResourceRetriever.Object, cacheProvider.Object, null, logger.Object));
        }

        [Test]
        public void TokenManager_null_logger()
        {
            //ARRANGE
            var httpResourceRetriever = new Mock<IHttpResourceRetriever>();
            var cacheProvider = new Mock<ICacheProvider>();
            var bearerTokenParser = new Mock<IBearerTokenParser>();
            var context = new Mock<IBearerTokenContext>();
            var logger = new Mock<IEventLogger<TokenManager>>();

            //ACT
            //ASSERT
            Assert.Throws<ArgumentNullException>(() => new TokenManager(httpResourceRetriever.Object, cacheProvider.Object, bearerTokenParser.Object, null));
        }

        [Test]
        public async Task TokenManager_get_token_http_call_is_made_http_code_200()
        {
            //ARRANGE
            var uri = new Uri("https://cas/token");
            var logger = new Mock<IEventLogger<TokenManager>>();
            var httpContent = new ByteArrayContent(Encoding.UTF8.GetBytes("Response"));
            var httpRequestContent = new ByteArrayContent(Encoding.UTF8.GetBytes("Request"));
            var httpResponseMessage = new HttpResponseMessage { Content = httpContent, StatusCode = System.Net.HttpStatusCode.OK };
            
            var httpResourceRetriever = new Mock<IHttpResourceRetriever>();
            httpResourceRetriever.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>(), true))
                .Returns(Task.FromResult(httpResponseMessage));

            var cacheProvider = new MemoryCacheRuntimeImplementor();

            var token = new TokenDescriptor("Bearer", "Token", DateTimeOffset.Now, 1000);
        
            var bearerTokenParser = new Mock<IBearerTokenParser>();
            bearerTokenParser.Setup(x => x.Parse(It.IsAny<string>()))
            .Returns(Task.FromResult(token));

            var context = new Mock<IBearerTokenContext>();
            context.Setup(x => x.ContextKey()).Returns(String.Format("_key_{0}", Guid.NewGuid()));
            context.SetupGet(x => x.Content).Returns(httpRequestContent);
            context.SetupGet(x => x.Endpoint)
                .Returns(new Endpoint(uri.AbsoluteUri));
            var tokenManager = new TokenManager(httpResourceRetriever.Object, cacheProvider, bearerTokenParser.Object, logger.Object);
            
            //ACT
            var actual = await tokenManager.GetToken(context.Object, CancellationToken.None);
            //ASSERT
            httpResourceRetriever.Verify(x => x.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>(), true));
        }

        [Test]
        public async Task TokenManager_get_token_http_call_is_made_http_code_500()
        {
            //ARRANGE
            var logger = new Mock<IEventLogger<TokenManager>>();
            var uri = new Uri("https://cas/token");
            var httpContent = new ByteArrayContent(Encoding.UTF8.GetBytes("Response"));
            var httpRequestContent = new ByteArrayContent(Encoding.UTF8.GetBytes("Request"));
            var httpResponseMessage = new HttpResponseMessage { Content = httpContent, StatusCode = System.Net.HttpStatusCode.InternalServerError };

            var httpResourceRetriever = new Mock<IHttpResourceRetriever>();
            httpResourceRetriever.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>(), true))
                .Returns(Task.FromResult(httpResponseMessage));

            var cacheProvider = new MemoryCacheRuntimeImplementor();

            var token = new TokenDescriptor("Bearer", "Token", DateTimeOffset.Now, 1000);

            var bearerTokenParser = new Mock<IBearerTokenParser>();
            bearerTokenParser.Setup(x => x.Parse(It.IsAny<string>()))
            .Returns(Task.FromResult(token));

            var context = new Mock<IBearerTokenContext>();
            context.Setup(x => x.ContextKey()).Returns(String.Format("_key_{0}", Guid.NewGuid()));
            context.SetupGet(x => x.Content).Returns(httpRequestContent);
            context.SetupGet(x => x.Endpoint)
                .Returns(new Endpoint(uri.AbsoluteUri));
            var tokenManager = new TokenManager(httpResourceRetriever.Object, cacheProvider, bearerTokenParser.Object, logger.Object);

            //ACT
            var tokenActual = await tokenManager.GetToken(context.Object, CancellationToken.None);
            //ASSERT
            Assert.IsNull(tokenActual);
        }

        [Test]
        public async Task TokenManager_get_token_is_called_and_parse_token_is_called()
        {
            //ARRANGE
            var logger = new Mock<IEventLogger<TokenManager>>();
            var uri = new Uri("https://cas/token");
            var httpContent = new ByteArrayContent(Encoding.UTF8.GetBytes("Response"));
            var httpRequestContent = new ByteArrayContent(Encoding.UTF8.GetBytes("Request"));
            var httpResponseMessage = new HttpResponseMessage { Content = httpContent, StatusCode = System.Net.HttpStatusCode.OK };

            var httpResourceRetriever = new Mock<IHttpResourceRetriever>();
            httpResourceRetriever.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>(), true))
                .Returns(Task.FromResult(httpResponseMessage));

            var cacheProvider = new MemoryCacheRuntimeImplementor();

            var token = new TokenDescriptor("Bearer", "Token", DateTimeOffset.Now, 1000);

            var bearerTokenParser = new Mock<IBearerTokenParser>();
            bearerTokenParser.Setup(x => x.Parse(It.IsAny<string>()))
            .Returns(Task.FromResult(token));

            var context = new Mock<IBearerTokenContext>();
            context.Setup(x => x.ContextKey()).Returns(String.Format("_key_{0}", Guid.NewGuid()));
            context.SetupGet(x => x.Content).Returns(httpRequestContent);
            context.SetupGet(x => x.Endpoint)
                .Returns(new Endpoint(uri.AbsoluteUri));
            var tokenManager = new TokenManager(httpResourceRetriever.Object, cacheProvider, bearerTokenParser.Object, logger.Object);

            //ACT
            var actual = await tokenManager.GetToken(context.Object, CancellationToken.None);
            //ASSERT
            bearerTokenParser.Verify(x => x.Parse(It.IsAny<string>()));

            Assert.AreEqual(actual, token);
        }

        [Test]
        public async Task TokenManager_get_token_called_and_token_is_expected_one()
        {
            //ARRANGE
            var logger = new Mock<IEventLogger<TokenManager>>();
            var uri = new Uri("https://cas/token");
            var httpContent = new ByteArrayContent(Encoding.UTF8.GetBytes("Response"));
            var httpRequestContent = new ByteArrayContent(Encoding.UTF8.GetBytes("Request"));
            var httpResponseMessage = new HttpResponseMessage { Content = httpContent, StatusCode = System.Net.HttpStatusCode.OK };

            var httpResourceRetriever = new Mock<IHttpResourceRetriever>();
            httpResourceRetriever.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>(), true))
                .Returns(Task.FromResult(httpResponseMessage));

            var cacheProvider = new MemoryCacheRuntimeImplementor();

            var token = new TokenDescriptor("Bearer", "Token", DateTimeOffset.Now, 1000);

            var bearerTokenParser = new Mock<IBearerTokenParser>();
            bearerTokenParser.Setup(x => x.Parse(It.IsAny<string>()))
            .Returns(Task.FromResult(token));

            var context = new Mock<IBearerTokenContext>();
            context.Setup(x => x.ContextKey()).Returns(String.Format("_key_{0}", Guid.NewGuid()));
            context.SetupGet(x => x.Content).Returns(httpRequestContent);
            context.SetupGet(x => x.Endpoint)
                .Returns(new Endpoint(uri.AbsoluteUri));
            var tokenManager = new TokenManager(httpResourceRetriever.Object, cacheProvider, bearerTokenParser.Object, logger.Object);

            //ACT
            var actual = await tokenManager.GetToken(context.Object, CancellationToken.None);
            //ASSERT
            
            Assert.AreEqual(actual, token);
        }

        [Test]
        public async Task TokenManager_get_token_called_and_token_is_expected_one_token_endpoint_as_a_string()
        {
            //ARRANGE
            var logger = new Mock<IEventLogger<TokenManager>>();
            var uri = new Uri("https://cas/token");
            var httpContent = new ByteArrayContent(Encoding.UTF8.GetBytes("Response"));
            var httpRequestContent = new ByteArrayContent(Encoding.UTF8.GetBytes("Request"));
            var httpResponseMessage = new HttpResponseMessage { Content = httpContent, StatusCode = System.Net.HttpStatusCode.OK };

            var httpResourceRetriever = new Mock<IHttpResourceRetriever>();
            httpResourceRetriever.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>(), true))
                .Returns(Task.FromResult(httpResponseMessage));

            var cacheProvider = new MemoryCacheRuntimeImplementor();

            var token = new TokenDescriptor("Bearer", "Token", DateTimeOffset.Now, 1000);

            var bearerTokenParser = new Mock<IBearerTokenParser>();
            bearerTokenParser.Setup(x => x.Parse(It.IsAny<string>()))
            .Returns(Task.FromResult(token));

            var context = new Mock<IBearerTokenContext>();
            context.Setup(x => x.ContextKey()).Returns(String.Format("_key_{0}", Guid.NewGuid()));
            context.SetupGet(x => x.Content).Returns(httpRequestContent);
            context.SetupGet(x => x.Endpoint)
                .Returns(new Endpoint(uri.AbsoluteUri));
            var tokenManager = new TokenManager(httpResourceRetriever.Object, cacheProvider, bearerTokenParser.Object, logger.Object);

            //ACT
            var actual = await tokenManager.GetToken(context.Object, CancellationToken.None);
            //ASSERT

            Assert.AreEqual(actual, token);
        }

        [Test]
        public async Task TokenManager_get_token_called_no_content_token_is_null()
        {
            //ARRANGE
            var logger = new Mock<IEventLogger<TokenManager>>();
            var uri = new Uri("https://cas/token");
            var httpResourceRetriever = new Mock<IHttpResourceRetriever>();
            
            var cacheProvider = new MemoryCacheRuntimeImplementor();
            
            var bearerTokenParser = new Mock<IBearerTokenParser>();
            
            var context = new Mock<IBearerTokenContext>();
            context.Setup(x => x.ContextKey()).Returns(String.Format("_key_{0}", Guid.NewGuid()));
            context.SetupGet(x => x.Content)
                .Returns((HttpContent)null);
            context.SetupGet(x => x.Endpoint)
                .Returns(new Endpoint(uri.AbsoluteUri));
            var tokenManager = new TokenManager(httpResourceRetriever.Object, cacheProvider, bearerTokenParser.Object, logger.Object);

            //ACT
            var actual = await tokenManager.GetToken(context.Object, CancellationToken.None);
            //ASSERT

            Assert.IsNull(actual);
        }

        [Test]
        public async Task TokenManager_get_token__write_to_cache()
        {
            //ARRANGE
            var readFromCache = false;
            var writeToCache = false;
            var logger = new Mock<IEventLogger<TokenManager>>();
            var uri = new Uri("https://cas/token");
            var httpContent = new ByteArrayContent(Encoding.UTF8.GetBytes("Response"));
            var httpRequestContent = new ByteArrayContent(Encoding.UTF8.GetBytes("Request"));
            var httpResponseMessage = new HttpResponseMessage { Content = httpContent, StatusCode = System.Net.HttpStatusCode.OK };

            var httpResourceRetriever = new Mock<IHttpResourceRetriever>();
            httpResourceRetriever.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>(), true))
                .Returns(Task.FromResult(httpResponseMessage));

            var cacheProvider = new MemoryCacheRuntimeImplementor();
            cacheProvider.ReadFrom += new EventHandler((_, cacheEvent) => 
            {
                if (cacheEvent == null)
                    return;

                readFromCache = true;
            });
            cacheProvider.WrittenTo += new EventHandler((_, __) => { writeToCache = true; });
            var token = new TokenDescriptor("Bearer", "Token", DateTimeOffset.Now, 1000);

            var bearerTokenParser = new Mock<IBearerTokenParser>();
            bearerTokenParser.Setup(x => x.Parse(It.IsAny<string>()))
            .Returns(Task.FromResult(token));

            var context = new Mock<IBearerTokenContext>();
            context.Setup(x => x.ContextKey()).Returns(String.Format("_key_{0}", Guid.NewGuid()));
            context.SetupGet(x => x.Content).Returns(httpRequestContent);
            context.SetupGet(x => x.Endpoint)
                .Returns(new Endpoint(uri.AbsoluteUri));
            var tokenManager = new TokenManager(httpResourceRetriever.Object, cacheProvider, bearerTokenParser.Object, logger.Object);

            //ACT
            var actual = await tokenManager.GetToken(context.Object, CancellationToken.None);
            //ASSERT
            Assert.IsFalse(readFromCache);
            Assert.IsTrue(writeToCache);
            Assert.AreEqual(actual, token);
        }

        [Test]
        public async Task TokenManager_get_token_read_from_cache_on_second_call()
        {
            //ARRANGE
            var readFromCache = false;
            var writeToCache = false;
            var logger = new Mock<IEventLogger<TokenManager>>();
            var uri = new Uri("https://cas/token");
            var httpContent = new ByteArrayContent(Encoding.UTF8.GetBytes("Response"));
            var httpRequestContent = new ByteArrayContent(Encoding.UTF8.GetBytes("Request"));
            var httpResponseMessage = new HttpResponseMessage { Content = httpContent, StatusCode = System.Net.HttpStatusCode.OK };

            var httpResourceRetriever = new Mock<IHttpResourceRetriever>();
            httpResourceRetriever.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>(), true))
                .Returns(Task.FromResult(httpResponseMessage));

            var cacheProvider = new MemoryCacheRuntimeImplementor();
            cacheProvider.ReadFrom += new EventHandler((_, __) => { readFromCache = true; });
            cacheProvider.WrittenTo += new EventHandler((_, __) => { writeToCache = true; });
            var token = new TokenDescriptor("Bearer", "Token", DateTimeOffset.Now, 1000);

            var bearerTokenParser = new Mock<IBearerTokenParser>();
            bearerTokenParser.Setup(x => x.Parse(It.IsAny<string>()))
            .Returns(Task.FromResult(token));

            var context = new Mock<IBearerTokenContext>();
            context.Setup(x => x.ContextKey()).Returns(String.Format("_key_{0}", Guid.NewGuid()));
            context.SetupGet(x => x.Content).Returns(httpRequestContent);
            context.SetupGet(x => x.Endpoint)
                .Returns(new Endpoint(uri.AbsoluteUri));
            var tokenManager = new TokenManager(httpResourceRetriever.Object, cacheProvider, bearerTokenParser.Object, logger.Object);

            //ACT
            var actual = await tokenManager.GetToken(context.Object, CancellationToken.None);
            await Task.Delay(1000);
            writeToCache = false;
            var actual1 = await tokenManager.GetToken(context.Object, CancellationToken.None);

            //ASSERT
            Assert.True(readFromCache);
            Assert.IsFalse(writeToCache);
            Assert.AreEqual(actual, token);
        }

        [Test]
        public async Task TokenManager_token_entry_is_removed_after_time_token_expired()
        {
            //ARRANGE
            var readFromCache = false;
            var writeToCache = false;
            var logger = new Mock<IEventLogger<TokenManager>>();
            var uri = new Uri("https://cas/token");
            var httpContent = new ByteArrayContent(Encoding.UTF8.GetBytes("Response"));
            var httpRequestContent = new ByteArrayContent(Encoding.UTF8.GetBytes("Request"));
            var httpResponseMessage = new HttpResponseMessage { Content = httpContent, StatusCode = System.Net.HttpStatusCode.OK };

            var httpResourceRetriever = new Mock<IHttpResourceRetriever>();
            httpResourceRetriever.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>(), true))
                .Returns(Task.FromResult(httpResponseMessage));

            var cacheProvider = new MemoryCacheRuntimeImplementor();
            cacheProvider.ReadFrom += new EventHandler((_, __) => { readFromCache = true; });
            cacheProvider.WrittenTo += new EventHandler((_, __) => { writeToCache = true; });
            var token = new TokenDescriptor("Bearer1", "Token1", DateTimeOffset.Now, 1);

            var bearerTokenParser = new Mock<IBearerTokenParser>();
            bearerTokenParser.Setup(x => x.Parse(It.IsAny<string>()))
            .Returns(Task.FromResult(token))
            .Verifiable();

            var context = new Mock<IBearerTokenContext>();
            context.Setup(x => x.ContextKey()).Returns(String.Format("_key_{0}", Guid.NewGuid()));
            context.SetupGet(x => x.Content).Returns(httpRequestContent);
            context.SetupGet(x => x.Endpoint)
                .Returns(new Endpoint(uri.AbsoluteUri));
            var tokenManager = new TokenManager(httpResourceRetriever.Object, cacheProvider, bearerTokenParser.Object, logger.Object);

            //ACT
            var actual = await tokenManager.GetToken(context.Object, CancellationToken.None);
            await Task.Delay(2000);
            writeToCache = false;
            token = new TokenDescriptor("Bearer2", "Token2", DateTimeOffset.Now, 1);
            var actual1 = await tokenManager.GetToken(context.Object, CancellationToken.None);
            //ASSERT
            Assert.IsTrue(writeToCache);
            bearerTokenParser.Verify(x => x.Parse(It.IsAny<string>()), Times.Exactly(2));
        }
    }
}