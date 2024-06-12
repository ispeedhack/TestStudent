using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ResultControllerTests
    {
        private Mock<IResultRepository> _mockRepo;

        private ResultController _sut;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mockRepo = new Mock<IResultRepository>();
            _sut = new ResultController(_mockRepo.Object);
        }

        [Test]
        public void Get_WhenCorrectIdGiven_ShouldReturnJsonViewModel()
        {
            var resultId = 1;
            var resultModel = new Result
            {
                Id = resultId,
                Text = "Text1"
            };

            _mockRepo.Setup(x => x.GetResult(resultId)).Returns(Task.FromResult(resultModel));

            var result = _sut.Get(resultId).Result as JsonResult;

            Assert.IsNotNull(result);
            var viewModel = result.GetObjectFromJsonResult<ResultViewModel>();
            Assert.AreEqual(viewModel.Text, resultModel.Text);
            Assert.AreEqual(viewModel.Id, resultModel.Id);
        }

        [Test]
        public void Get_WhenInvalidIdGiven_ShouldReturnNotFound()
        {
            var resultId = 2;
            _mockRepo.Setup(x => x.GetResult(resultId)).Returns(Task.FromResult((Result)null));

            var result = _sut.Get(resultId).Result;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public void GetByTestId_WhenCorrectIdGiven_ShouldReturnJsonViewModel()
        {
            var testId = 1;
            var results = new List<Result>
            {
                new Result
                {
                    Id = 1,
                    Text = "Text1",
                    TestId = testId
                },
                new Result
                {
                    Id = 2,
                    Text = "Text2",
                    TestId = testId
                }
            };

            _mockRepo.Setup(x => x.GetResults(testId)).Returns(Task.FromResult(results));

            var result = _sut.GetByTestId(testId).Result as JsonResult;

            Assert.IsNotNull(result);

            var viewModelsCollection = result.GetIEnumberableFromJsonResult<ResultViewModel>().ToList();
            foreach (var resultModel in results)
            {
                Assert.IsTrue(viewModelsCollection.Any(x => x.Text == resultModel.Text));
                Assert.IsTrue(viewModelsCollection.Any(x => x.Id == resultModel.Id));
                Assert.IsTrue(viewModelsCollection.Any(x => x.TestId == testId));
            }
        }

        [Test]
        public void GetByTestId_WhenInvalidIdGiven_ShouldReturnNotFound()
        {
            var testId = 2;
            _mockRepo.Setup(x => x.GetResults(testId)).Returns(Task.FromResult((List<Result>)null));

            var result = _sut.GetByTestId(testId).Result;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public void Post_WhenCorrectViewModelGiven_ShouldReturnJsonViewModel()
        {
            var resultId = 1;
            var resultModel = new Result
            {
                Id = resultId,
                Text = "Text1"
            };

            _mockRepo.Setup(x => x.CreateResult(It.Is<Result>(r => r.Id == resultId))).Returns(Task.FromResult(resultModel));

            var result = _sut.Post(resultModel.Adapt<ResultViewModel>()).Result as JsonResult;

            Assert.IsNotNull(result);
            var viewModel = result.GetObjectFromJsonResult<ResultViewModel>();
            Assert.AreEqual(viewModel.Text, resultModel.Text);
            Assert.AreEqual(viewModel.Id, resultModel.Id);
        }

        [Test]
        public void Post_WhenInvalidViewModelGiven_ShouldReturnBadRequest()
        {
            var result = _sut.Post(null).Result as BadRequestResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, 400);
        }

        [Test]
        public void Post_WhenErrorDuringProcessing_ShouldReturnStatusCode500()
        {
            var resultId = 1;
            var resultModel = new Result
            {
                Id = resultId,
                Text = "Text1"
            };

            _mockRepo.Setup(x => x.CreateResult(It.Is<Result>(r => r.Id == resultId))).Throws(new Exception());

            var result = _sut.Post(resultModel.Adapt<ResultViewModel>()).Result as StatusCodeResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, 500);
        }

        [Test]
        public void Put_WhenCorrectViewModelGiven_ShouldReturnJsonViewModel()
        {
            var resultId = 1;
            var resultModel = new Result
            {
                Id = resultId,
                Text = "Text1"
            };

            _mockRepo.Setup(x => x.UpdateResult(It.Is<Result>(r => r.Id == resultId))).Returns(Task.FromResult(resultModel));

            var result = _sut.Put(resultModel.Adapt<ResultViewModel>()).Result as JsonResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.GetObjectFromJsonResult<ResultViewModel>().Text, resultModel.Text);
            Assert.AreEqual(result.GetObjectFromJsonResult<ResultViewModel>().Id, resultModel.Id);
        }

        [Test]
        public void Put_WhenErrorDuringProcessing_ShouldReturnStatusCode500()
        {
            var resultId = 1;
            var resultModel = new Result
            {
                Id = resultId,
                Text = "Text1"
            };

            _mockRepo.Setup(x => x.UpdateResult(It.Is<Result>(r => r.Id == resultId))).Throws(new Exception());

            var result = _sut.Put(resultModel.Adapt<ResultViewModel>()).Result as StatusCodeResult;

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
        public void Put_WhenCorrectViewModelErrorDuringProcessing_ShouldReturnNotFound()
        {
            var viewModel = new ResultViewModel
            {
                Id = 1,
                Text = "Text1"
            };

            _mockRepo.Setup(x => x.UpdateResult(viewModel.Adapt<Result>())).Returns<ResultViewModel>(null);

            var result = _sut.Put(viewModel).Result;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public void Delete_WhenCorrectViewModelGiven_ShouldReturnJsonViewModel()
        {
            var resultId = 1;

            _mockRepo.Setup(x => x.DeleteResult(resultId)).Returns(Task.FromResult(true));

            var result = _sut.Delete(resultId).Result;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public void Delete_WhenCorrectViewModelErrorDuringProcessing_ShouldReturnNotFound()
        {
            var resultId = 2;

            _mockRepo.Setup(x => x.DeleteResult(resultId)).Returns(Task.FromResult(false));

            var result = _sut.Delete(resultId).Result;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }
    }
}
