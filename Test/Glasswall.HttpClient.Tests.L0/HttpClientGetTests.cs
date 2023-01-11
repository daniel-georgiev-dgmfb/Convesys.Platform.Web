using Convesys.HttpClient.Tests.L0.MockData;
using Convesys.Kernel.Configuration;
using Convesys.Kernel.Logging;
using Convesys.Kernel.Security.Validation;
using Convesys.Kernel.Web;
using Moq;
using NUnit.Framework;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Convesys.HttpClient.Tests.L0
{
    [TestFixture]
    [Category("Convesys.HttpClient.Tests.L0")]
    public class HttpClientGetTests
    {
        [Test]
        public void Get_with_http_schema_throws_exception()
        {
            //ARRANGE
            var backchannelValidator = new Mock<IBackchannelCertificateValidator>();
            var logger = new Mock<IEventLogger<Convesys.Platform.Web.HttpClient.HttpClient>>();
            var url = "http://localhost/";
            //ACT
            var httpClient = new Convesys.Platform.Web.HttpClient.HttpClient(backchannelValidator.Object, logger.Object);
            //ASSERT
            Assert.ThrowsAsync<ArgumentException>(() => httpClient.GetAsync(url, CancellationToken.None));
        }

        [Test]
        public void Get_empty_url_throws_exception()
        {
            //ARRANGE
            var backchannelValidator = new Mock<IBackchannelCertificateValidator>();
            var logger = new Mock<IEventLogger<Convesys.Platform.Web.HttpClient.HttpClient>>();
            var url = String.Empty;
            //ACT
            var httpClient = new Convesys.Platform.Web.HttpClient.HttpClient(backchannelValidator.Object, logger.Object);
            //ASSERT
            Assert.ThrowsAsync<ArgumentNullException>(() => httpClient.GetAsync(url, CancellationToken.None));
        }

        [Test]
        public void Get_null_url_throws_exception()
        {
            //ARRANGE
            var backchannelValidator = new Mock<IBackchannelCertificateValidator>();
            var logger = new Mock<IEventLogger<Convesys.Platform.Web.HttpClient.HttpClient>>();
            var url = (string)null;
            //ACT
            var httpClient = new Convesys.Platform.Web.HttpClient.HttpClient(backchannelValidator.Object, logger.Object);
            //ASSERT
            Assert.ThrowsAsync<ArgumentNullException>(() => httpClient.GetAsync(url, CancellationToken.None));
        }

        [Test]
        public async Task Get_with_https_schema_success_response()
        {
            //ARRANGE
            var backchannelValidator = new Mock<IBackchannelCertificateValidator>();
            var logger = new Mock<IEventLogger<Convesys.Platform.Web.HttpClient.HttpClient>>();
            backchannelValidator.Setup(x => x.Validate(It.IsAny<object>(), It.IsAny<X509Certificate>(), It.IsAny<X509Chain>(), It.IsAny<SslPolicyErrors>()))
                .Returns(true);
            var url = "http://localhost:4449/";
            var contentMessage = "HttpResponse";
            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(Encoding.UTF8.GetBytes(contentMessage)) };
            var httpClient = new MockHttpClient(backchannelValidator.Object, _ => Task.FromResult(response), logger.Object);
            httpClient.RequireHttps = false;
            //ACT

            var result = await httpClient.GetAsync(url, CancellationToken.None);
            //ASSERT
            Assert.AreEqual(contentMessage, result);
        }

        [Test]
        public void Get_with_https_schema_500_response()
        {
            //ARRANGE
            var backchannelValidator = new Mock<IBackchannelCertificateValidator>();
            var logger = new Mock<IEventLogger<Convesys.Platform.Web.HttpClient.HttpClient>>();
            backchannelValidator.Setup(x => x.Validate(It.IsAny<object>(), It.IsAny<X509Certificate>(), It.IsAny<X509Chain>(), It.IsAny<SslPolicyErrors>()))
                .Returns(true);
            var url = "https://localhost/";
            var contentMessage = "HttpResponse";
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new ByteArrayContent(Encoding.UTF8.GetBytes(contentMessage)) };
            var httpClient = new MockHttpClient(backchannelValidator.Object, _ => Task.FromResult(response), logger.Object);
            //ACT
            
            //ASSERT
            Assert.ThrowsAsync<IOException>(() => httpClient.GetAsync(url, CancellationToken.None));
        }

        [Test]
        public async Task Get_with_http_schema_verify_configuration_called()
        {
            //ARRANGE
            var configuration = new ResourceRetrieverCustomConfigurator();
            var logger = new Mock<IEventLogger<Convesys.Platform.Web.HttpClient.HttpClient>>();
            var backchannelValidator = new Mock<IBackchannelCertificateValidator>();
            backchannelValidator.Setup(x => x.Validate(It.IsAny<object>(), It.IsAny<X509Certificate>(), It.IsAny<X509Chain>(), It.IsAny<SslPolicyErrors>()))
                .Returns(true);
            var url = "http://localhost/";
            var contentMessage = "HttpResponse";
            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(Encoding.UTF8.GetBytes(contentMessage)) };
            var httpClient = new MockHttpClient(backchannelValidator.Object, _ => Task.FromResult(response), logger.Object);
            httpClient.HttpDocumentRetrieverConfigurator = configuration;
            httpClient.RequireHttps = false;
            //ACT

            var result = await httpClient.GetAsync(url, CancellationToken.None);
            //ASSERT
        }
    }
}