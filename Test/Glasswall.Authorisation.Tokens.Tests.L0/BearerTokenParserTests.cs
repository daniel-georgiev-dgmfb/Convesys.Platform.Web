﻿using Convesys.Common.Serialisation.JSON;
using Convesys.Common.Serialisation.JSON.SettingsProviders;
using Convesys.Platform.Web.Tokens;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Convesys.Authorisation.Tokens.Tests.L0
{
    [TestFixture]
    [Category("Glasswall.Authorisation.Tokens.Tests.L0")]
    public class BearerTokenParserTests
    {
        [Test]
        public void BearerTokenParser_Serialiser_null()
        {
            //ARRANGE
            //ACT
            //ASSERT
            Assert.Throws<ArgumentNullException>(() => new BearerTokenParser(null));
        }

        [Test]
        public async Task BearerTokenParser_Parse()
        {
            //ARRANGE
            var serialiser = new NSJsonSerializer(new DefaultSettingsProvider());
            var parser = new BearerTokenParser(serialiser);
            var source = new { access_token = "Token", token_type = "Bearer", expires_in = 10 };
            var serialised = await serialiser.SerialiseToJson(source);
            //ACT
            var tokenDescriptor = await parser.Parse(serialised);
            //ASSERT
            Assert.AreEqual("Bearer", tokenDescriptor.TokenType);
            Assert.AreEqual("Token", tokenDescriptor.Token);
        }
    }
}