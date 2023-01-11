using Convesys.Kernel.Logging;
using Convesys.Kernel.Security.Validation;
using Moq;
using NUnit.Framework;
using System;

namespace Glasswall.HttpClient.Tests.L0
{
    [TestFixture]
    [Category("Convesys.HttpClient.Tests.L0")]
    public class HttpClientConstructorTests
    {
        [Test]
        public void Instantiate_HttpCkient_backchannelCertificateValidator_null()
        {
            //ARRANGE
            var logger = new Mock<IEventLogger<Convesys.Platform.Web.HttpClient.HttpClient>>();
            //ACT
            //ASSERT
            Assert.Throws<ArgumentNullException>(() => new Convesys.Platform.Web.HttpClient.HttpClient(null, logger.Object));
        }

        [Test]
        public void Instantiate_HttpCkient_logger_null()
        {
            //ARRANGE
            var backchannelValidator = new Mock<IBackchannelCertificateValidator>();
            var logger = new Mock<IEventLogger<Convesys.Platform.Web.HttpClient.HttpClient>>();
            //ACT
            //ASSERT
            Assert.Throws<ArgumentNullException>(() => new Convesys.Platform.Web.HttpClient.HttpClient(backchannelValidator.Object, null));
        }

        [Test]
        public void Default_settings_when_instance_is_created()
        {
            //ARRANGE
            var logger = new Mock<IEventLogger<Convesys.Platform.Web.HttpClient.HttpClient>>();
            var backchannelValidator = new Mock<IBackchannelCertificateValidator>();
            //ACT
            var httpClient = new Convesys.Platform.Web.HttpClient.HttpClient(backchannelValidator.Object, logger.Object);
            //ASSERT
            Assert.IsTrue(httpClient.RequireHttps);
            Assert.AreEqual(TimeSpan.FromSeconds(30), httpClient.Timeout);
            Assert.AreEqual(10485760L, httpClient.MaxResponseContentBufferSize);
        }
    }
}