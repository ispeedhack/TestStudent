using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestCreator.Data.Models;
using TestCreator.Data.Repositories.Interfaces;
using TestCreator.WebApp.Controllers;
using TestCreator.WebApp.Helpers;
using TestCreator.WebApp.Services.Interfaces;
using TestCreator.WebApp.ViewModels;

namespace TestCreator.Tests.Controllers
{
    [TestFixture]
    public class TokenControllerTests
    {
        [Test]
        public void Auth_WhenBodyViewModelWithPasswordGrantTypeAndUserName_ShouldReturnTokenResponse()
        {
            var username = "username1";
            var userId = Guid.NewGuid().ToString();
            var clientId = Guid.NewGuid().ToString();

            var token = new Token
            {
                ClientId = clientId,
                UserId = userId,
                Id = 1,
                CreationDate = DateTime.Now,
                Value = Guid.NewGuid().ToString()
            };

            var tokenData = new TokenData
            {
                ExporationTimeInMinutes = 60,
                EncodedToken = token.Value
            };

            TokenRequestViewModel viewModel = new TokenRequestViewModel
            {
                Username = username,
                ClientId = clientId,
                Password = "fehfuiyf8eywfkj",
                GrantType = "password"
            };
            ApplicationUser user = new ApplicationUser
            {
                Id = userId,
                UserName = username
            };


            var mockService = new Mock<ITokenService>();
            mockService.Setup(x => x.GenerateRefreshToken(clientId, userId)).Returns(token);
            mockService.Setup(x => x.CreateAccessToken(userId)).Returns(tokenData);

            var mockUserAndRolesRepo = new Mock<IUserAndRoleRepository>();
            mockUserAndRolesRepo.Setup(x => x.GetUserByNameAsync(username)).Returns(Task.FromResult(user));
            mockUserAndRolesRepo.Setup(x => x.CheckPasswordAsync(user, It.IsAny<string>())).Returns(Task.FromResult(true));

            var mockTokenRepo = new Mock<ITokenRepository>();
            mockTokenRepo.Setup(x => x.AddRefreshToken(It.IsAny<Token>())).Verifiable();


            var controller = new TokenController(mockUserAndRolesRepo.Object, mockTokenRepo.Object, mockService.Object);

            var result = controller.Auth(viewModel).Result as JsonResult;

            Assert.IsNotNull(result);

            var json = new JsonResult(new TokenResponseViewModel
            {
                Expiration = tokenData.ExporationTimeInMinutes,
                RefreshToken = tokenData.EncodedToken,
                Token = token.Value
            });

            Assert.AreEqual(result.Value.ToString(), json.Value.ToString());
        }

        [Test]
        public void Auth_WhenBodyViewModelWithPasswordGrantTypeAndEmail_ShouldReturnTokenResponse()
        {
            var username = "username1@wp.pl";
            var userId = Guid.NewGuid().ToString();
            var clientId = Guid.NewGuid().ToString();

            var token = new Token
            {
                ClientId = clientId,
                UserId = userId,
                Id = 1,
                CreationDate = DateTime.Now,
                Value = Guid.NewGuid().ToString()
            };

            var tokenData = new TokenData
            {
                ExporationTimeInMinutes = 60,
                EncodedToken = token.Value
            };

            TokenRequestViewModel viewModel = new TokenRequestViewModel
            {
                Username = username,
                ClientId = clientId,
                Password = "fehfuiyf8eywfkj",
                GrantType = "password"
            };
            ApplicationUser user = new ApplicationUser
            {
                Id = userId,
                UserName = username
            };


            var mockServie = new Mock<ITokenService>();
            mockServie.Setup(x => x.GenerateRefreshToken(clientId, userId)).Returns(token);
            mockServie.Setup(x => x.CreateAccessToken(userId)).Returns(tokenData);

            var mockUserAndRolesRepo = new Mock<IUserAndRoleRepository>();
            mockUserAndRolesRepo.Setup(x => x.GetUserByNameAsync(username)).Returns(Task.FromResult<ApplicationUser>(null));
            mockUserAndRolesRepo.Setup(x => x.GetUserByEmailAsync(username)).Returns(Task.FromResult(user));
            mockUserAndRolesRepo.Setup(x => x.CheckPasswordAsync(user, It.IsAny<string>())).Returns(Task.FromResult(true));

            var mockTokenRepo = new Mock<ITokenRepository>();
            mockTokenRepo.Setup(x => x.AddRefreshToken(It.IsAny<Token>())).Verifiable();


            var controller = new TokenController(mockUserAndRolesRepo.Object, mockTokenRepo.Object, mockServie.Object);

            var result = controller.Auth(viewModel).Result as JsonResult;

            Assert.IsNotNull(result);

            var json = new JsonResult(new TokenResponseViewModel
            {
                Expiration = tokenData.ExporationTimeInMinutes,
                RefreshToken = tokenData.EncodedToken,
                Token = token.Value
            });

            Assert.AreEqual(result.Value.ToString(), json.Value.ToString());
        }

        [Test]
        public void Auth_WhenBodyViewModelWithPasswordGrantTypeWrongPassword_ShouldReturnUnauthorizedResult()
        {
            var username = "username1@wp.pl";
            var userId = Guid.NewGuid().ToString();
            var clientId = Guid.NewGuid().ToString();

            TokenRequestViewModel viewModel = new TokenRequestViewModel
            {
                Username = username,
                ClientId = clientId,
                Password = "fehfuiyf8eywfkj",
                GrantType = "password"
            };
            ApplicationUser user = new ApplicationUser
            {
                Id = userId,
                UserName = username
            };


            var mockUserAndRolesRepo = new Mock<IUserAndRoleRepository>();
            mockUserAndRolesRepo.Setup(x => x.GetUserByNameAsync(username)).Returns(Task.FromResult(user));
            mockUserAndRolesRepo.Setup(x => x.CheckPasswordAsync(user, It.IsAny<string>())).Returns(Task.FromResult(false));

            var mockTokenRepo = new Mock<ITokenRepository>();
            mockTokenRepo.Setup(x => x.AddRefreshToken(It.IsAny<Token>())).Verifiable();


            var controller = new TokenController(mockUserAndRolesRepo.Object, mockTokenRepo.Object, null);

            Assert.IsInstanceOf<UnauthorizedResult>(controller.Auth(viewModel).Result);
        }

        [Test]
        public void Auth_WhenNullViewModel_ShouldReturnBadRequest()
        {
            var controller = new TokenController(null, null, null);

            var result = controller.Auth(null).Result as BadRequestResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, 400);
        }

        [Test]
        public void Auth_WhenBodyViewModelWithRefreshTokenGrantTypeAndUserName_ShouldReturnTokenResponse()
        {
            var username = "username1";
            var userId = Guid.NewGuid().ToString();
            var clientId = Guid.NewGuid().ToString();

            var refreshToken = new Token
            {
                ClientId = clientId,
                UserId = userId,
                Id = 1,
                CreationDate = DateTime.Now,
                Value = Guid.NewGuid().ToString()
            };

            var newRefreshToken = new Token
            {
                ClientId = clientId,
                UserId = userId,
                Id = 1,
                CreationDate = DateTime.Now,
                Value = Guid.NewGuid().ToString()
            };

            var tokenData = new TokenData
            {
                ExporationTimeInMinutes = 60,
                EncodedToken = refreshToken.Value
            };

            TokenRequestViewModel viewModel = new TokenRequestViewModel
            {
                Username = username,
                ClientId = clientId,
                Password = "fehfuiyf8eywfkj",
                GrantType = "refresh_token"
            };
            ApplicationUser user = new ApplicationUser
            {
                Id = userId,
                UserName = username
            };


            var mockServie = new Mock<ITokenService>();
            mockServie.Setup(x => x.GenerateRefreshToken(clientId, userId)).Returns(newRefreshToken);
            mockServie.Setup(x => x.CreateAccessToken(userId)).Returns(tokenData);

            var mockUserAndRolesRepo = new Mock<IUserAndRoleRepository>();
            mockUserAndRolesRepo.Setup(x => x.GetUserById(userId)).Returns(Task.FromResult(user));

            var mockTokenRepo = new Mock<ITokenRepository>();
            mockTokenRepo.Setup(x => x.AddRefreshToken(It.IsAny<Token>())).Verifiable();
            mockTokenRepo.Setup(x => x.RemoveRefreshToken(It.IsAny<Token>())).Verifiable();
            mockTokenRepo.Setup(x => x.CheckRefreshTokenForClient(viewModel.ClientId, It.IsAny<string>()))
                .Returns(Task.FromResult(refreshToken));

            var controller = new TokenController(mockUserAndRolesRepo.Object, mockTokenRepo.Object, mockServie.Object);

            var result = controller.Auth(viewModel).Result as JsonResult;

            Assert.IsNotNull(result);

            var json = new JsonResult(new TokenResponseViewModel
            {
                Expiration = tokenData.ExporationTimeInMinutes,
                RefreshToken = tokenData.EncodedToken,
                Token = newRefreshToken.Value
            });

            Assert.AreEqual(result.Value.ToString(), json.Value.ToString());
        }

        [Test]
        public void Auth_WhenBodyViewModelWithRefreshTokenGrantTypeAndWrongUserName_ShouldReturnUnauthorizedResult()
        {
            var username = "username1";
            var userId = Guid.NewGuid().ToString();
            var clientId = Guid.NewGuid().ToString();

            var refreshToken = new Token
            {
                ClientId = clientId,
                UserId = userId,
                Id = 1,
                CreationDate = DateTime.Now,
                Value = Guid.NewGuid().ToString()
            };

            TokenRequestViewModel viewModel = new TokenRequestViewModel
            {
                Username = username,
                ClientId = clientId,
                Password = "fehfuiyf8eywfkj",
                GrantType = "refresh_token"
            };

            var mockUserAndRolesRepo = new Mock<IUserAndRoleRepository>();
            mockUserAndRolesRepo.Setup(x => x.GetUserById(userId)).Returns(Task.FromResult<ApplicationUser>(null));

            var mockTokenRepo = new Mock<ITokenRepository>();
            mockTokenRepo.Setup(x => x.CheckRefreshTokenForClient(viewModel.ClientId, It.IsAny<string>()))
                .Returns(Task.FromResult(refreshToken));

            var controller = new TokenController(mockUserAndRolesRepo.Object, mockTokenRepo.Object, null);

            Assert.IsInstanceOf<UnauthorizedResult>(controller.Auth(viewModel).Result);
        }

        [Test]
        public void Auth_WhenBodyViewModelWithRefreshTokenGrantTypeAndInvalidRefreshToken_ShouldReturnUnauthorizedResult()
        {
            var username = "username1";
            var clientId = Guid.NewGuid().ToString();

            TokenRequestViewModel viewModel = new TokenRequestViewModel
            {
                Username = username,
                ClientId = clientId,
                Password = "fehfuiyf8eywfkj",
                GrantType = "refresh_token"
            };

            var mockTokenRepo = new Mock<ITokenRepository>();
            mockTokenRepo.Setup(x => x.CheckRefreshTokenForClient(viewModel.ClientId, It.IsAny<string>()))
                .Returns(Task.FromResult((Token)null));

            var controller = new TokenController(null, mockTokenRepo.Object, null);

            Assert.IsInstanceOf<UnauthorizedResult>(controller.Auth(viewModel).Result);
        }

        [Test]
        public void Auth_WhenInvalidViewModelWithRefreshTokenGrantType_ShouldReturnUnauthorizedResult()
        {
            TokenRequestViewModel viewModel = new TokenRequestViewModel
            {
                Username = null,
                ClientId = null,
                Password = "fehfuiyf8eywfkj",
                GrantType = "refresh_token"
            };

            var mockTokenRepo = new Mock<ITokenRepository>();
            mockTokenRepo.Setup(x => x.CheckRefreshTokenForClient(viewModel.ClientId, It.IsAny<string>()))
                .Returns(Task.FromResult((Token)null));

            var controller = new TokenController(null, mockTokenRepo.Object, null);

            Assert.IsInstanceOf<UnauthorizedResult>(controller.Auth(viewModel).Result);
        }
    }
}
