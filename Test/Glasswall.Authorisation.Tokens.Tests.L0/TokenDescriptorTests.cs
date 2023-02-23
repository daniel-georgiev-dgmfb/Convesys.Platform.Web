using System;
using Twiligth.Kernel.Web.Authorisation;
using NUnit.Framework;

namespace Twiligth.Authorisation.Tokens.Tests.L0
{
    [TestFixture]
    [Category("Twiligth.Authorisation.Tokens.Tests.L0")]
    public class TokenDescriptorTests
    {
        [Test]
        public void TokenType_null()
        {
            //ARRANGE
            //ACT
            //ASSERT
            Assert.Throws<ArgumentNullException>(() => new TokenDescriptor(null, "Token", DateTimeOffset.Now, 1));
        }

        [Test]
        public void TokenType_Empty()
        {
            //ARRANGE
            //ACT
            //ASSERT
            Assert.Throws<ArgumentNullException>(() => new TokenDescriptor(String.Empty, "Token", DateTimeOffset.Now, 1));
        }

        [Test]
        public void Token_null()
        {
            //ARRANGE
            //ACT
            //ASSERT
            Assert.Throws<ArgumentNullException>(() => new TokenDescriptor("Bearer", null, DateTimeOffset.Now, 1));
        }

        [Test]
        public void Token_Empty()
        {
            //ARRANGE
            //ACT
            //ASSERT
            Assert.Throws<ArgumentNullException>(() => new TokenDescriptor("Bearer", String.Empty, DateTimeOffset.Now, 1));
        }

        [Test]
        public void IssuedAt_in_the_future()
        {
            //ARRANGE
            //ACT
            //ASSERT
            Assert.Throws<InvalidOperationException>(() => new TokenDescriptor("Bearer", "Token", DateTimeOffset.Now.AddSeconds(1), 1));
        }

        [Test]
        public void Token_Get()
        {
            //ARRANGE
            var tokenDescriptor = new TokenDescriptor("Bearer", "Token", DateTimeOffset.Now, 1);
            //ACT
            //ASSERT
            Assert.AreEqual("Token", tokenDescriptor.Token);
        }

        [Test]
        public void TokenType_Get()
        {
            //ARRANGE
            var tokenDescriptor = new TokenDescriptor("Bearer", "Token", DateTimeOffset.Now, 1);
            //ACT
            //ASSERT
            Assert.AreEqual("Bearer", tokenDescriptor.TokenType);
        }

        [Test]
        public void TokenExpire_Get()
        {
            //ARRANGE
            var now = DateTimeOffset.Now;
            uint expireIn = 360;
            var expected = now.AddSeconds(expireIn);
            var tokenDescriptor = new TokenDescriptor("Bearer", "Token", now, expireIn);
            //ACT
            //ASSERT
            Assert.AreEqual(expected, tokenDescriptor.ExpireOn);
        }
    }
}