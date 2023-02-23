using Twiligth.Kernel.Logging;
using Twiligth.Kernel.Web;
using Twiligth.Kernel.Web.Authorisation;
using Twiligth.Platform.Web.Api.Client;
using Twiligth.Platform.Web.Tokens.Contexts;
using Moq;
using NUnit.Framework;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Glasswall.Web.Api.Client.Tests.L0
{
    [TestFixture]
    [Category("Twiligth.Web.Api.Client.Tests.L0")]
    public class ApiClientTests
    {
        [Test]
        public void Constructor_token_manager_null()
        {
            //ARRANGE
            var httpResourceRetriever = new Mock<IHttpResourceRetriever>();
            var logger = new Mock<IEventLogger<ApiClient>>();
            var contextFactory = new Mock<Func<ClaimsPrincipal, PrincipalTokenContext>>();
            //ACT
            //ASSERT
            Assert.Throws<ArgumentNullException>(() => new ApiClient(null, httpResourceRetriever.Object, logger.Object));
        }

        [Test]
        public void Constructor_http_client_null()
        {
            //ARRANGE
            var tokenManager = new Mock<IBearerTokenManager>();
            var logger = new Mock<IEventLogger<ApiClient>>();
            //ACT
            //ASSERT
            Assert.Throws<ArgumentNullException>(() => new ApiClient(tokenManager.Object, null, logger.Object));
        }

        [Test]
        public void Constructorhttp_context_accessor_null()
        {
            //ARRANGE
            var tokenManager = new Mock<IBearerTokenManager>();
            var httpResourceRetriever = new Mock<IHttpResourceRetriever>();
            
            //ACT
            //ASSERT
            Assert.Throws<ArgumentNullException>(() => new ApiClient(tokenManager.Object, httpResourceRetriever.Object, null));
        }

        [Test]
        [Ignore("Find out how to test it")]
        public async Task Api_client_token_resolve_verify_get_token_called()
        {
            //ARRANGE
            var tokenDescriptor = new TokenDescriptor("Bearer", "Token", DateTimeOffset.Now, 3600);
            var credencials = new Mock<IBearerTokenContext>();
            var tokenManager = new Mock<IBearerTokenManager>();
            var logger = new Mock<IEventLogger<ApiClient>>();
            var httpResourceRetriever = new Mock<IHttpResourceRetriever>();

            tokenManager.Setup(x => x.GetToken(It.IsAny<IBearerTokenContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(tokenDescriptor)).Verifiable();
           
            var client = new ApiClient(tokenManager.Object, httpResourceRetriever.Object, logger.Object);
            var request = new RequestContext(new Endpoint("https://localhost"), credencials.Object);

            //ACT
            await client.GetAsync(request, CancellationToken.None);
            //ASSERT
            tokenManager.Verify();
        }

        [Test]
        [Ignore("To be implemented")]
        public async Task Api_client_token_resolve_authentication_header_added()
        {
            
        }
    }
}
