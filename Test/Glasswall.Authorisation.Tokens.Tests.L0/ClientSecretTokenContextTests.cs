using System;
using System.Net.Http;
using System.Text;
using Convesys.Kernel.Web;
using Convesys.Platform.Web.Tokens.Contexts;
using Kernel.Cryptography.DataProtection;
using Moq;
using NUnit.Framework;

namespace Glasswall.Authorisation.Tokens.Tests.L0
{
    [TestFixture]
    [Category("Glasswall.Authorisation.Tokens.Tests.L0")]
    public class ClientSecretTokenContextTests
    {
        [Test]
        public void ClientId_null()
        {
            //ARRANGE
            //ACT
            //ASSERT
            Assert.Throws<ArgumentNullException>(() => new ClientSecretTokenContext(null, "Password", new Endpoint("https://localhost/")));
        }

        [Test]
        public void ClientId_Empty()
        {
            //ARRANGE
            //ACT
            //ASSERT
            Assert.Throws<ArgumentNullException>(() => new ClientSecretTokenContext(String.Empty, "Password", new Endpoint("https://localhost/")));
        }

        [Test]
        public void Password_null()
        {
            //ARRANGE
            //ACT
            //ASSERT
            Assert.Throws<ArgumentNullException>(() => new ClientSecretTokenContext("ClientId", null, new Endpoint("https://localhost/")));
        }

        [Test]
        public void Password_Empty()
        {
            //ARRANGE
            //ACT
            //ASSERT
            Assert.Throws<ArgumentNullException>(() => new ClientSecretTokenContext("ClientId", String.Empty, new Endpoint("https://localhost/")));
        }

        [Test]
        public void Endpint_null()
        {
            //ARRANGE
            //ACT
            //ASSERT
            Assert.Throws<ArgumentNullException>(() => new ClientSecretTokenContext("ClientId", "secret", null));
        }
        [Test]
        public void ClientSecretTokenContext_Content()
        {
            //ARRANGE
            var context = new ClientSecretTokenContext("ClientId", "Password", new Endpoint("https://localhost/"));
            //ACT
            var content = context.Content as FormUrlEncodedContent;
            //ASSERT
            Assert.IsNotNull(content);
        }

        [Test]
        public void ClientSecretTokenContext_GrantType()
        {
            //ARRANGE
            var context = new ClientSecretTokenContext("ClientId", "Password", new Endpoint("https://localhost/"));
            //ACT
            var grantType = context.GrantType;
            //ASSERT
            Assert.IsNotNull("client_credentials", grantType);
        }

        

        [Test]
        public void TokenContextKey()
        {
            //ARRANGE
            var context = new ClientSecretTokenContext("ClientId", "Password", new Endpoint("https://localhost/"));
            var userNameBytes = Encoding.UTF8.GetBytes("ClientId");
            var encryptor = new PasswordEncryptor();
            var keyBytes = encryptor.GetDeriveBytes("Password", 1000, userNameBytes, 256);
            var expected = Convert.ToBase64String(keyBytes);
            //ACT
            var key = context.ContextKey();
            //ASSERT
            Assert.IsNotNull(expected, key);
        }

        [Test]
        public void HeaderHandlerTest()
        {
            //ASSERT
            var base64CredencialExpected = Convert.ToBase64String(Encoding.UTF8.GetBytes(String.Format("{0}:{1}", "ClientId", "Password")));
            var context = new ClientSecretTokenContext("ClientId", "Password", new Endpoint("https://localhost/"));
            var mockHttpClient = new Mock<HttpClient>();
            //ACT
            context.HeaderHandler(mockHttpClient.Object.DefaultRequestHeaders);
            //ASSERT
            Assert.AreEqual("Basic", mockHttpClient.Object.DefaultRequestHeaders.Authorization.Scheme);
            Assert.AreEqual(base64CredencialExpected, mockHttpClient.Object.DefaultRequestHeaders.Authorization.Parameter);
        }
    }
}