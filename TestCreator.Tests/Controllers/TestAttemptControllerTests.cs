using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestCreator.Data.Models;
using TestCreator.Data.Repositories.Interfaces;
using TestCreator.Tests.Helpers;
using TestCreator.WebApp.Controllers;
using TestCreator.WebApp.Converters;
using TestCreator.WebApp.Services.Interfaces;
using TestCreator.WebApp.ViewModels;

namespace TestCreator.Tests.Controllers
{
    [TestFixture]
    public class TestAttemptControllerTests
    {
        private Mock<ITestRepository> _mockRepo;
        private Mock<ITestCalculationService> _mockService;

        private TestAttemptController _sut;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mockRepo = new Mock<ITestRepository>();
            _mockService = new Mock<ITestCalculationService>();
            _sut = new TestAttemptController(_mockService.Object, _mockRepo.Object, new TestAttemptViewModelConverter());
        }

        [Test]
        public void Get_WhenCorrectIdGiven_ShouldReturnJsonViewModel()
        {
            var testId = 1;
            var test = new Test
            {
                Id = 1,
                Title = "title1",
                Questions = new List<Question>
                {
                    new Question
                    {
                        Id = 1,
                        Answers = new List<Answer>
                        {
                            new Answer
                            {
                                Id = 1
                            }
                        }
                    }
                }
            };

            _mockRepo.Setup(x => x.GetTestWithInclude(testId)).Returns(Task.FromResult(test));

            var result = _sut.Get(testId).Result as JsonResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.GetValueFromJsonResult<string>("Title"), test.Title);
            Assert.AreEqual(result.GetValueFromJsonResult<int>("TestId"), test.Id);
            Assert.IsNotNull(result.GetValueFromJsonResult<List<TestAttemptEntryViewModel>>("TestAttemptEntries"));
        }

        [Test]
        public void Get_WhenInvalidIdGiven_ShouldReturnNotFound()
        {
            var testId = 2;
            _mockRepo.Setup(x => x.GetTestWithInclude(testId)).Returns(Task.FromResult((Test)null));

            var result = _sut.Get(testId).Result;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public void CalculateResult_WhenCorrectTestAttemptViewModel_ShouldReturnJsonViewModel()
        {
            var viewModel = new TestAttemptViewModel()
            {
                TestId = 1,
                Title = "title1",
                TestAttemptEntries = new List<TestAttemptEntryViewModel>
                {
                    new TestAttemptEntryViewModel()
                }
            };

            var returnedViewModel = new TestAttemptResultViewModel()
            {
                TestId = 1,
                Title = "title1",
                Score = 10,
                MaximalPossibleScore = 15
            };

            _mockService.Setup(x => x.CalculateResult(It.IsAny<TestAttemptViewModel>())).Returns(returnedViewModel);

            var result = _sut.CalculateResult(viewModel) as JsonResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.GetValueFromJsonResult<string>("Title"), returnedViewModel.Title);
            Assert.AreEqual(result.GetValueFromJsonResult<int>("TestId"), returnedViewModel.TestId);
            Assert.AreEqual(result.GetValueFromJsonResult<int>("Score"), returnedViewModel.Score);
            Assert.AreEqual(result.GetValueFromJsonResult<int>("MaximalPossibleScore"), returnedViewModel.MaximalPossibleScore);
        }

        [Test]
        public void CalculateResult_WhenNullTestAttemptViewModel_ShouldReturnBadRequest()
        {
            var viewModel = new TestAttemptViewModel()
            {
                TestId = 1,
                Title = "title1",
                TestAttemptEntries = new List<TestAttemptEntryViewModel>
                {
                    new TestAttemptEntryViewModel()
                }
            };

            _mockService.Setup(x => x.CalculateResult(It.IsAny<TestAttemptViewModel>()))
                .Throws(new Exception());

            var result = _sut.CalculateResult(viewModel) as StatusCodeResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, 500);
        }

        [Test]
        public void CalculateResult_WhenErrorDuringProcessing_ShouldReturnBadRequest()
        {
            var result = _sut.CalculateResult(null);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public void CalculateResult_WhenCorrectTestAttemptViewModelErrorDuringCalculating_ThenReturnNotFound()
        {
            var viewModel = new TestAttemptViewModel()
            {
                TestId = 1,
                Title = "title1",
                TestAttemptEntries = new List<TestAttemptEntryViewModel>
                {
                    new TestAttemptEntryViewModel()
                }
            };

            _mockService.Setup(x => x.CalculateResult(It.IsAny<TestAttemptViewModel>())).Returns<TestAttemptResultViewModel>(null);

            var result = _sut.CalculateResult(viewModel) as StatusCodeResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, 500);
        }
    }
}
