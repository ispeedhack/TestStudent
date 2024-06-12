using System;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestCreator.Data.Models;
using TestCreator.Data.Repositories.Interfaces;
using TestCreator.Tests.Helpers;
using TestCreator.WebApp.Controllers;
using TestCreator.WebApp.ViewModels;

namespace TestCreator.Tests.Controllers
{
    [TestFixture]
    public class UserControllerTests
    {
        private Mock<IUserAndRoleRepository> _mockRepo;

        private UserController _sut;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mockRepo = new Mock<IUserAndRoleRepository>();
            _sut = new UserController(_mockRepo.Object);
        }

        [Test]
        public void Post_WhenCorrectViewModelGivenUserDoesNotExist_ShouldReturnJsonViewModel()
        {
            var applicationUser = new ApplicationUser
            {
                Email = "user1@wp.pl",
                UserName = "user1"
            };

            _mockRepo.Setup(x => x.CreateUserAndAddToRolesAsync(It.Is<ApplicationUser>(u => u.Email == "user1@wp.pl"), 
                    It.IsAny<string[]>()))
                .Returns(Task.FromResult(applicationUser));
            _mockRepo.Setup(x => x.GetUserByNameAsync(It.IsAny<string>()))
                .Returns(Task.FromResult((ApplicationUser)null));
            _mockRepo.Setup(x => x.GetUserByEmailAsync(It.IsAny<string>()))
                .Returns(Task.FromResult((ApplicationUser)null));

            var result = _sut.Post(applicationUser.Adapt<UserViewModel>()).Result as JsonResult;

            Assert.IsNotNull(result);
            var viewModel = result.GetObjectFromJsonResult<UserViewModel>();
            Assert.AreEqual(viewModel.Email, applicationUser.Email);
            Assert.AreEqual(viewModel.UserName, applicationUser.UserName);
        }

        [Test]
        public void Post_WhenCorrectViewModelGivenUserWithNameExists_ShouldReturnJsonViewModel()
        {
            var viewModel = new UserViewModel
            {
                Email = "user1@wp.pl",
                UserName = "user1",
                Password = "password123"
            };
            var user = new ApplicationUser
            {
                UserName = viewModel.UserName,
                Email = viewModel.Email
            };

            _mockRepo.Setup(x => x.CreateUserAndAddToRolesAsync(user, It.IsAny<string[]>()))
                .Returns(Task.FromResult(user));
            _mockRepo.Setup(x => x.GetUserByNameAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(user));
            _mockRepo.Setup(x => x.GetUserByEmailAsync(It.IsAny<string>()))
                .Returns(Task.FromResult((ApplicationUser)null));

            var result = _sut.Post(viewModel).Result as BadRequestObjectResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Post_WhenCorrectViewModelGivenUserWithEmailExists_ShouldReturnJsonViewModel()
        {
            var viewModel = new UserViewModel
            {
                Email = "user1@wp.pl",
                UserName = "user1",
                Password = "password123"
            };
            var user = new ApplicationUser
            {
                UserName = viewModel.UserName,
                Email = viewModel.Email
            };

            _mockRepo.Setup(x => x.CreateUserAndAddToRolesAsync(user, It.IsAny<string[]>()))
                .Returns(Task.FromResult(user));
            _mockRepo.Setup(x => x.GetUserByNameAsync(It.IsAny<string>()))
                .Returns(Task.FromResult((ApplicationUser)null));
            _mockRepo.Setup(x => x.GetUserByEmailAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(user));

            var result = _sut.Post(viewModel).Result as BadRequestObjectResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Post_WhenErrorDuringProcessing_ShouldReturnStatusCode500()
        {
            var viewModel = new UserViewModel
            {
                Email = "user1@wp.pl",
                UserName = "user1",
                Password = "password123"
            };
            var user = new ApplicationUser
            {
                UserName = viewModel.UserName,
                Email = viewModel.Email
            };

            _mockRepo.Setup(x => x.GetUserByNameAsync(It.IsAny<string>()))
                .Throws(new Exception());

            var result = _sut.Post(viewModel).Result as StatusCodeResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, 500);
        }

        [Test]
        public void Post_WhenInvalidViewModelGiven_ShouldReturnBadRequest()
        {
            var result = _sut.Post(null).Result as BadRequestResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, 400);
        }
    }
}
