//  ----------------------------------------------------------------------- 
//   <copyright file="WhenCallingSendAsyncWithNullToken.cs" company="Glasswall Solutions Ltd.">
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
    public class WhenCallingSendAsyncWithNullToken
    {
        private Mock<IBearerTokenManager> _tokenManager;
        private Mock<IHttpResourceRetriever> _httpResourceRetriever;
        private Mock<IGWLogger<ApiClient>> _logger;

        [OneTimeSetUp]
        public async Task SetUp()
        {
            var credentials = new Mock<IBearerTokenContext>();
            _tokenManager = new Mock<IBearerTokenManager>();
            _logger = new Mock<IGWLogger<ApiClient>>();
            _httpResourceRetriever = new Mock<IHttpResourceRetriever>();
            var bearerTokenContext = credentials.Object;
            var cancellationToken = CancellationToken.None;
            var resourceRetriever = _httpResourceRetriever.Object;

            _tokenManager.Setup(x => x.GetToken(It.Is<IBearerTokenContext>(a => a == bearerTokenContext), It.Is<CancellationToken>(a => a == cancellationToken)))
                .Returns(Task.FromResult<TokenDescriptor>(null)).Verifiable();
            
            var client = new ApiClient(_tokenManager.Object, resourceRetriever, _logger.Object);
            
            var request = new RequestContext(new Endpoint("https://localhost"), bearerTokenContext);
            
            await client.GetAsync(request, cancellationToken);
        }

        [Test]
        public void TokenManager_Is_Called_To_Get_The_Token_With_Token_Context_From_Request_And_Cancellation_Token_From_Call()
        {
            Assert.DoesNotThrow(() => _tokenManager.Verify());
        }

        [Test]
        public void HttpClient_Has_Its_HeadersHandler_Set()
        {
            // Set twice - once for default.
            _httpResourceRetriever.VerifySet(a => a.HeadersHandler = It.IsAny<Action<HttpRequestHeaders>>(), Times.Once);
        }

        [Test]
        public void Logger_Called_Informing_Header_Set()
        {
            _logger.Verify(
                a => a.Log(It.Is<LogLevel>(b => b == LogLevel.Debug), It.IsAny<EventId>(),
                    It.Is<string>(b => b.Equals("Token: not acquired. Anonymous request")), It.Is<Exception>(b => b == null),
                    It.IsAny<Func<string, Exception, string>>()), Times.Once);
        }
    }
}