using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using TestCreator.WebApp.Services;
using TestCreator.WebApp.Services.Interfaces;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace TestCreator.Tests.Service
{
    [TestFixture]
    public class TokenServiceTests
    {
        [Test]
        public void CreateClaims_UserIdProvided_ReturnsClaimsCollection()
        {
            ITokenService service = new TokenService(null);

            string guid = Guid.NewGuid().ToString();
            var result = service.CreateClaims(guid);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Length, 3);
            Assert.AreEqual(result.First().Value, guid);
            Assert.AreEqual(result.First().Type, JwtRegisteredClaimNames.Sub);
        }

        [Test]
        public void CreateClaims_NullArgument_ThrowsArgumentException()
        {
            ITokenService service = new TokenService(null);

            Assert.Throws<ArgumentException>(() => service.CreateClaims(null));
        }

        [Test]
        public void GenerateRefreshToken_ValidClientIdAndUserId_ReturnsGeneratedToken()
        {
            ITokenService service = new TokenService(null);

            string clientId = Guid.NewGuid().ToString();
            string userId = Guid.NewGuid().ToString();

            var result = service.GenerateRefreshToken(clientId, userId);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.ClientId, clientId);
            Assert.AreEqual(result.UserId, userId);
            Assert.AreEqual(result.Type, 0);
        }

        [Test]
        public void GenerateRefreshToken_NullArguments_ThrowsArgumentException()
        {
            ITokenService service = new TokenService(null);

            Assert.Throws<ArgumentException>(() => service.GenerateRefreshToken(null, null));
        }


        [Test]
        public void CreateAccessToken_ValidUserId_ReturnsToken()
        {
            var configuration = new Mock<IConfiguration>();
            configuration.SetupGet(x => x[It.Is<string>(s => s == "Auth:Jwt:Key")]).Returns("------key123------");
            configuration.SetupGet(x => x[It.Is<string>(s => s == "Auth:Jwt:Issuer")]).Returns("issuer1");
            configuration.SetupGet(x => x[It.Is<string>(s => s == "Auth:Jwt:Audience")]).Returns("audience1");


            //https://dejanstojanovic.net/aspnet/2018/november/mocking-iconfiguration-getvalue-extension-methods-in-unit-test/
            //GetValue<T>() is static so some workaround is needed here to mock
            var mockSection = new Mock<IConfigurationSection>();
            mockSection.Setup(s => s.Value).Returns("100");
            configuration.Setup(c => c.GetSection("Auth:Jwt:TokenExpirationInMinutes")).Returns(mockSection.Object);


            ITokenService service = new TokenService(configuration.Object);

            string userId = Guid.NewGuid().ToString();

            var result = service.CreateAccessToken(userId);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.ExporationTimeInMinutes, 100);

            var decodedToken = new JwtSecurityTokenHandler().ReadJwtToken(result.EncodedToken);

            Assert.AreEqual(decodedToken.Issuer, "issuer1");
            Assert.AreEqual(decodedToken.Audiences.First(), "audience1");
        }

        [Test]
        public void CreateAccessToken_NullArgument_ReturnsToken()
        {
            var configuration = new Mock<IConfiguration>();
            configuration.SetupGet(x => x[It.Is<string>(s => s == "Auth:Jwt:Key")]).Returns("------key123------");
            configuration.SetupGet(x => x[It.Is<string>(s => s == "Auth:Jwt:Issuer")]).Returns("issuer1");
            configuration.SetupGet(x => x[It.Is<string>(s => s == "Auth:Jwt:Audience")]).Returns("audience1");

            var mockSection = new Mock<IConfigurationSection>();
            mockSection.Setup(s => s.Value).Returns("100");
            configuration.Setup(c => c.GetSection("Auth:Jwt:TokenExpirationInMinutes")).Returns(mockSection.Object);

            ITokenService service = new TokenService(configuration.Object);

            Assert.Throws<ArgumentException>(() => service.CreateAccessToken(null));
        }
    }
}
