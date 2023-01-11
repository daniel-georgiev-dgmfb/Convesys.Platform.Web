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
    public class HttpClientPutTests
    {
        [Test]
        public void Put_with_http_schema_throws_exception()
        {
            //ARRANGE
            var backchannelValidator = new Mock<IBackchannelCertificateValidator>();
            var logger = new Mock<IEventLogger<Convesys.Platform.Web.HttpClient.HttpClient>>();
            var url = "http://localhost/";
            var message = "json_content";
            //ACT
            var httpClient = new Convesys.Platform.Web.HttpClient.HttpClient(backchannelValidator.Object, logger.Object);
            //ASSERT
            Assert.ThrowsAsync<ArgumentException>(() => httpClient.PutAsyncAsJson(url, message, CancellationToken.None));
        }

        [Test]
        public void Put_empty_url_throws_exception()
        {
            //ARRANGE
            var backchannelValidator = new Mock<IBackchannelCertificateValidator>();
            var url = String.Empty;
            var message = "json_content";
            var logger = new Mock<IEventLogger<Convesys.Platform.Web.HttpClient.HttpClient>>();
            //ACT
            var httpClient = new Convesys.Platform.Web.HttpClient.HttpClient(backchannelValidator.Object, logger.Object);
            //ASSERT
            Assert.ThrowsAsync<ArgumentNullException>(() => httpClient.PutAsyncAsJson(url, message, CancellationToken.None));
        }

        [Test]
        public void Put_null_url_throws_exception()
        {
            //ARRANGE
            var logger = new Mock<IEventLogger<Convesys.Platform.Web.HttpClient.HttpClient>>();
            var backchannelValidator = new Mock<IBackchannelCertificateValidator>();
            var url = (string)null;
            var message = "json_content";
            //ACT
            var httpClient = new Convesys.Platform.Web.HttpClient.HttpClient(backchannelValidator.Object, logger.Object);
            //ASSERT
            Assert.ThrowsAsync<ArgumentNullException>(() => httpClient.PutAsyncAsJson(url, message, CancellationToken.None));
        }

        [Test]
        public async Task Put_with_https_schema_success_response()
        {
            //ARRANGE
            var backchannelValidator = new Mock<IBackchannelCertificateValidator>();
            var logger = new Mock<IEventLogger<Convesys.Platform.Web.HttpClient.HttpClient>>();
            backchannelValidator.Setup(x => x.Validate(It.IsAny<object>(), It.IsAny<X509Certificate>(), It.IsAny<X509Chain>(), It.IsAny<SslPolicyErrors>()))
                .Returns(true);
            var url = "https://localhost/";
            var message = "json_content";
            var contentResponseMessage = "HttpResponse";
            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(Encoding.UTF8.GetBytes(contentResponseMessage)) };
            var httpClient = new MockHttpClient(backchannelValidator.Object, _ => Task.FromResult(response), logger.Object);
            //ACT

            var result = await httpClient.PutAsyncAsJson(url, message, CancellationToken.None);
            //ASSERT
            Assert.AreEqual(contentResponseMessage, result);
        }

        [Test]
        public void Put_with_https_schema_500_response()
        {
            //ARRANGE
            var backchannelValidator = new Mock<IBackchannelCertificateValidator>();
            var logger = new Mock<IEventLogger<Convesys.Platform.Web.HttpClient.HttpClient>>();
            backchannelValidator.Setup(x => x.Validate(It.IsAny<object>(), It.IsAny<X509Certificate>(), It.IsAny<X509Chain>(), It.IsAny<SslPolicyErrors>()))
                .Returns(true);
            var url = "https://localhost/";
            var contentResponseMessage = "HttpResponse";
            var message = "json_content";
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new ByteArrayContent(Encoding.UTF8.GetBytes(contentResponseMessage)) };
            var httpClient = new MockHttpClient(backchannelValidator.Object, _ => Task.FromResult(response), logger.Object);
            //ACT
            
            //ASSERT
            Assert.ThrowsAsync<IOException>(() => httpClient.PutAsyncAsJson(url, message, CancellationToken.None));
        }

        [Test]
        public async Task Put_with_http_schema_verify_configuration_called()
        {
            //ARRANGE
            var configuration = new Mock<ICustomConfigurator<IHttpResourceRetriever>>();
            var logger = new Mock<IEventLogger<Convesys.Platform.Web.HttpClient.HttpClient>>();
            configuration.Setup(x => x.Configure(It.IsAny<IHttpResourceRetriever>()));
            var backchannelValidator = new Mock<IBackchannelCertificateValidator>();
            backchannelValidator.Setup(x => x.Validate(It.IsAny<object>(), It.IsAny<X509Certificate>(), It.IsAny<X509Chain>(), It.IsAny<SslPolicyErrors>()))
                .Returns(true);
            var url = "http://localhost/";
            var contentMessage = "HttpResponse";
            var message = "json_content";
            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(Encoding.UTF8.GetBytes(contentMessage)) };
            var httpClient = new MockHttpClient(backchannelValidator.Object, _ => Task.FromResult(response), logger.Object);
            httpClient.HttpDocumentRetrieverConfigurator = configuration.Object;
            httpClient.RequireHttps = false;
            //ACT

            var result = await httpClient.PutAsyncAsJson(url, message, CancellationToken.None);
            //ASSERT
            configuration.Verify(x => x.Configure(It.IsAny<IHttpResourceRetriever>()));
        }
    }
}