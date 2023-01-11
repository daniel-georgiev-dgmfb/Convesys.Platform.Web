//  ----------------------------------------------------------------------- 
//   <copyright file="WhenCallingSendAsyncWithNullClientCredentials.cs" company="Glasswall Solutions Ltd.">
//       Glasswall Solutions Ltd.
//   </copyright>
//  ----------------------------------------------------------------------- 

namespace Glasswall.Web.Api.Client.Tests.L0.SendAsync
{
    using System;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using Kernel.Logging;
    using Kernel.Web;
    using Kernel.Web.Authorisation;
    using Moq;
    using NUnit.Framework;
    using Platform.Web.Api.Client;

    [TestFixture]
    [Category("Glasswall.Platform.Web.Api.Client")]
    public class WhenCallingSendAsyncWithNullClientCredentials
    {
        private Mock<IBearerTokenManager> _tokenManager;
        private Mock<IHttpResourceRetriever> _httpResourceRetriever;
        private Mock<IGWLogger<ApiClient>> _logger;

        [OneTimeSetUp]
        public async Task SetUp()
        {
            _tokenManager = new Mock<IBearerTokenManager>();
            _logger = new Mock<IGWLogger<ApiClient>>();
            _httpResourceRetriever = new Mock<IHttpResourceRetriever>();
            var cancellationToken = CancellationToken.None;
            var resourceRetriever = _httpResourceRetriever.Object;
            
            var client = new ApiClient(_tokenManager.Object, resourceRetriever, _logger.Object);
            
            var request = new RequestContext(new Endpoint("https://localhost"));
            
            await client.GetAsync(request, cancellationToken);
        }

        [Test]
        public void TokenManager_Is_Not_Called()
        {
            _tokenManager.Verify(x => x.GetToken(It.IsAny<IBearerTokenContext>(), It.IsAny<CancellationToken>()), Times.Exactly(0));
        }

        [Test]
        public void HttpClient_Has_Its_HeadersHandler_Set()
        {
            _httpResourceRetriever.VerifySet(a => a.HeadersHandler = It.IsAny<Action<HttpRequestHeaders>>(), Times.Once);
        }

        [Test]
        public void Logger_Called_Informing_Null_Header()
        {
            _logger.Verify(
                a => a.Log(It.Is<LogLevel>(b => b == LogLevel.Information), It.IsAny<EventId>(),
                    It.Is<string>(b => b.Equals("Null Authorisation header set.")), It.Is<Exception>(b => b == null),
                    It.IsAny<Func<string, Exception, string>>()), Times.Once);
        }
    }
}