using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moq;
using NUnit.Framework;
using TestCreator.Data.Models;
using TestCreator.Data.Repositories.Interfaces;
using TestCreator.Tests.Helpers;
using TestCreator.WebApp.Broadcast;
using TestCreator.WebApp.Broadcast.Interfaces;
using TestCreator.WebApp.Controllers;
using TestCreator.WebApp.ViewModels;

namespace TestCreator.Tests.Controllers
{
    [TestFixture]
    public class TestControllerTests
    {
        private Mock<ITestRepository> _mockRepo;
        private Mock<IHubContext<TestsHub, ITestsHubClient>> _mockHub;

        private TestController _sut;

        private readonly string _username = "username1";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mockRepo = new Mock<ITestRepository>();
            _mockHub = new Mock<IHubContext<TestsHub, ITestsHubClient>>();
            _sut = new TestController(_mockRepo.Object, _mockHub.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, _username)
            }, "mock"));

            _sut = new TestController(_mockRepo.Object, _mockHub.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user }
                }
            };
        }

        [Test]
        public void Get_WhenCorrectIdGiven_ShouldReturnJsonViewModel()
        {
            var testId = 1;
            var test = new Test
            {
                Id = 1,
                Title = "title1",
                UserId = _username
            };

            _mockRepo.Setup(x => x.GetTest(testId)).Returns(Task.FromResult(test));

            var result = _sut.Get(testId).Result as JsonResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.GetValueFromJsonResult<string>("Title"), test.Title);
            Assert.AreEqual(result.GetValueFromJsonResult<int>("Id"), test.Id);
            Assert.AreEqual(result.GetValueFromJsonResult<bool>("UserCanEdit"), true);
        }

        [Test]
        public void Get_WhenCorrectIdGivenButCurrentUserIsNotOwnerOfTest_ShouldReturnJsonViewModel()
        {
            var testId = 1;
            var test = new Test
            {
                Id = 1,
                Title = "title1",
                UserId = "user1"
            };

            _mockRepo.Setup(x => x.GetTest(testId)).Returns(Task.FromResult(test));

            var result = _sut.Get(testId).Result as JsonResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.GetValueFromJsonResult<string>("Title"), test.Title);
            Assert.AreEqual(result.GetValueFromJsonResult<int>("Id"), test.Id);
            Assert.AreEqual(result.GetValueFromJsonResult<bool>("UserCanEdit"), false);
        }

        [Test]
        public void Get_WhenInvalidIdGiven_ShouldReturnNotFound()
        {
            var testId = 2;
            _mockRepo.Setup(x => x.GetTest(testId)).Returns(Task.FromResult((Test)null));

            var result = _sut.Get(testId).Result;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public void Post_WhenCorrectViewModelGiven_ShouldReturnJsonViewModel()
        {
            var testId = 1;
            var test = new Test
            {
                Id = testId,
                Title = "title1"
            };

            _mockRepo.Setup(x => x.CreateTest(It.Is<Test>(t => t.Id == testId))).Returns(Task.FromResult(test));
            _mockHub.Setup(x => x.Clients.All.TestCreated()).Returns(Task.CompletedTask);

            var result = _sut.Post(test.Adapt<TestViewModel>()).Result as JsonResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.GetValueFromJsonResult<string>("Title"), test.Title);
            Assert.AreEqual(result.GetValueFromJsonResult<int>("Id"), test.Id);
        }

        [Test]
        public void Post_WhenInvalidViewModelGiven_ShouldReturnBadRequest()
        {
            var result = _sut.Post(null).Result as BadRequestResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, 400);
        }

        [Test]
        public void Post_WhenInvalidViewModelGiven_ShouldReturnStatusCode500()
        {
            var testId = 1;
            var test = new Test
            {
                Id = testId,
                Title = "title1"
            };

            _mockRepo.Setup(x => x.CreateTest(It.Is<Test>(t => t.Id == testId))).Throws(new Exception());

            var result = _sut.Post(test.Adapt<TestViewModel>()).Result as StatusCodeResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, 500);
        }

        [Test]
        public void Put_WhenCorrectViewModelGiven_ShouldReturnsJsonViewModel()
        {
            var testId = 1;
            var test = new Test
            {
                Id = 1,
                Title = "title1",
                UserId = _username
            };

            _mockRepo.Setup(x => x.GetTest(test.Id)).Returns(Task.FromResult(test));
            _mockRepo.Setup(x => x.UpdateTest(It.Is<Test>(t => t.Id == testId))).Returns(Task.FromResult(test));

            var result = _sut.Put(test.Adapt<TestViewModel>()).Result as JsonResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.GetValueFromJsonResult<string>("Title"), test.Title);
            Assert.AreEqual(result.GetValueFromJsonResult<int>("Id"), test.Id);
        }

        [Test]
        public void Put_WhenErrorDuringProcessing_ShouldReturnStatusCode500()
        {
            var testId = 1;
            var test = new Test
            {
                Id = 1,
                Title = "title1",
                UserId = _username
            };

            _mockRepo.Setup(x => x.GetTest(test.Id)).Returns(Task.FromResult(test));
            _mockRepo.Setup(x => x.UpdateTest(It.Is<Test>(t => t.Id == testId))).Throws(new Exception());

            var result = _sut.Put(test.Adapt<TestViewModel>()).Result as StatusCodeResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, 500);
        }

        [Test]
        public void Put_WhenInvalidViewModelGiven_ShouldReturnBadRequest()
        {
            var result = _sut.Put(null).Result as BadRequestResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, 400);
        }

        [Test]
        public void Put_WhenCorrectTestViewModelErrorDuringProcessing_ShouldReturnsNotFound()
        {
            var test = new Test
            {
                Id = 1,
                Title = "title1",
                UserId = _username
            };

            _mockRepo.Setup(x => x.GetTest(test.Id)).Returns(Task.FromResult(test));
            _mockRepo.Setup(x => x.UpdateTest(test)).Returns<TestViewModel>(null);

            var result = _sut.Put(test.Adapt<TestViewModel>()).Result;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public void Put_WhenCorrectTestViewModelButUserIsNotPermittedToEdit_ShouldReturnUnauthorized()
        {
            var test = new Test
            {
                Id = 1,
                Title = "title1",
                UserId = "user1"
            };

            _mockRepo.Setup(x => x.GetTest(test.Id)).Returns(Task.FromResult(test));

            var result = _sut.Put(test.Adapt<TestViewModel>()).Result;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }

        [Test]
        public void Put_WhenCorrectTestViewModelUserIsNotPermittedToDeleteButIsAdmin_ShouldReturnOkResult()
        {
            var testId = 2;
            var test = new Test
            {
                Id = testId,
                Title = "title1",
                UserId = "user1"
            };

            _mockRepo.Setup(x => x.UpdateTest(It.Is<Test>(t => t.Id == testId))).Returns(Task.FromResult(test));
            _mockRepo.Setup(x => x.GetTest(testId)).Returns(Task.FromResult(test));

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, _username),
                new Claim(ClaimTypes.Role, "Admin"),
            }, "mock"));

            var controller = new TestController(_mockRepo.Object, _mockHub.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user }
                }
            };

            var result = controller.Put(test.Adapt<TestViewModel>()).Result as JsonResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.GetValueFromJsonResult<string>("Title"), test.Title);
            Assert.AreEqual(result.GetValueFromJsonResult<int>("Id"), test.Id);
        }

        [Test]
        public void Delete_WhenCorrectViewModelGiven_ShouldReturnJsonViewModel()
        {
            var testId = 1;
            var test = new Test
            {
                Id = testId,
                Title = "title1",
                UserId = _username
            };

            _mockRepo.Setup(x => x.GetTest(testId)).Returns(Task.FromResult(test));
            _mockRepo.Setup(x => x.DeleteTest(testId)).Returns(Task.FromResult(true));
            _mockHub.Setup(x => x.Clients.All.TestRemoved(It.IsAny<int>())).Returns(Task.CompletedTask);

            var result = _sut.Delete(testId).Result;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public void Delete_CorrectTestIdButErrorDuringProcessing_ReturnsStatusCode500()
        {
            var testId = 2;
            var test = new Test
            {
                Id = testId,
                Title = "title1",
                UserId = _username
            };

            _mockRepo.Setup(x => x.DeleteTest(testId)).Returns(Task.FromResult(false));
            _mockRepo.Setup(x => x.GetTest(testId)).Returns(Task.FromResult(test));

            var result = _sut.Delete(testId).Result as StatusCodeResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, 500);
        }

        [Test]
        public void Delete_WhenCorrectTestIdButUserIsNotPermittedToDelete_ShouldReturnUnauthorized()
        {
            var testId = 2;
            var test = new Test
            {
                Id = testId,
                Title = "title1",
                UserId = "user1"
            };

            _mockRepo.Setup(x => x.GetTest(testId)).Returns(Task.FromResult(test));

            var result = _sut.Delete(testId).Result;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }

        [Test]
        public void Delete_WhenCorrectTestIdButUserIsNotPermittedToDeleteButIsAdmin_ShouldReturnOkResult()
        {
            var testId = 2;
            var test = new Test
            {
                Id = testId,
                Title = "title1",
                UserId = "user1"
            };

            _mockRepo.Setup(x => x.DeleteTest(testId)).Returns(Task.FromResult(true));
            _mockRepo.Setup(x => x.GetTest(testId)).Returns(Task.FromResult(test));
            _mockHub.Setup(x => x.Clients.All.TestRemoved(It.IsAny<int>())).Returns(Task.CompletedTask);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, _username),
                new Claim(ClaimTypes.Role, "Admin"), 
            }, "mock"));

            var controller = new TestController(_mockRepo.Object, _mockHub.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user }
                }
            };

            var result = controller.Delete(testId).Result;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public void Patch_WhenCorrectIdGiven_ShouldReturnOkResult()
        {
            var testId = 1;
            var viewModel = new UpdateTestViewCountViewModel
            {
                Id = testId
            };

            _mockRepo.Setup(x => x.IncrementTestViewCount(It.Is<int>(t => t == testId))).Returns(Task.FromResult(true));

            var result = _sut.UpdateTestViewCount(viewModel).Result as OkResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Patch_WhenCorrectIdAndErrorDuringProcessing_ShouldReturnStatusCode500()
        {
            var testId = 2;
            var viewModel = new UpdateTestViewCountViewModel
            {
                Id = testId
            };

            _mockRepo.Setup(x => x.IncrementTestViewCount(testId)).Returns(Task.FromResult(false));

            var result = _sut.UpdateTestViewCount(viewModel).Result as StatusCodeResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, 500);
        }
    }
}
