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
    public class QuestionControllerTests
    {
        private Mock<IQuestionRepository> _mockRepo;

        private QuestionController _sut;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mockRepo = new Mock<IQuestionRepository>();
            _sut = new QuestionController(_mockRepo.Object);
        }

        [Test]
        public void Get_WhenCorrectIdGiven_ShouldReturnJsonViewModel()
        {
            var questionId = 1;
            var question = new Question
            {
                Id = questionId,
                Text = "Text1"
            };

            _mockRepo.Setup(x => x.GetQuestion(questionId)).Returns(Task.FromResult(question));

            var result = _sut.Get(questionId).Result as JsonResult;

            Assert.IsNotNull(result);
            var viewModel = result.GetObjectFromJsonResult<QuestionViewModel>();
            Assert.AreEqual(viewModel.Text, question.Text);
            Assert.AreEqual(viewModel.Id, question.Id);
        }

        [Test]
        public void Get_WhenInvalidIdGiven_ShouldReturnNotFound()
        {
            var questionId = 2;

            _mockRepo.Setup(x => x.GetQuestion(questionId)).Returns(Task.FromResult((Question)null));
            var result = _sut.Get(questionId).Result;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public void GetByTestId_WhenCorrectIdGiven_ShouldReturnJsonViewModel()
        {
            var testId = 1;
            var questions = new List<Question>
            {
                new Question
                {
                    Id = 1,
                    Text = "Text1",
                    TestId = testId
                },
                new Question
                {
                    Id = 2,
                    Text = "Text2",
                    TestId = testId
                }
            };

            _mockRepo.Setup(x => x.GetQuestions(testId)).Returns(Task.FromResult(questions));

            var result = _sut.GetByTestId(testId).Result as JsonResult;

            Assert.IsNotNull(result);

            var viewModelsCollection = result.GetIEnumberableFromJsonResult<QuestionViewModel>().ToList();
            foreach (var question in questions)
            {
                Assert.IsTrue(viewModelsCollection.Any(x => x.Text == question.Text));
                Assert.IsTrue(viewModelsCollection.Any(x => x.Id == question.Id));
                Assert.IsTrue(viewModelsCollection.Any(x => x.TestId == testId));
            }
        }

        [Test]
        public void GetByTestId_WhenInvalidIdGiven_ShouldReturnNotFound()
        {
            var testId = 2;
            _mockRepo.Setup(x => x.GetQuestions(testId)).Returns(Task.FromResult((List<Question>)null));

            var result = _sut.GetByTestId(testId).Result;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public void Post_WhenCorrectViewModelGiven_ShouldReturnJsonViewModel()
        {
            var questionId = 1;
            var question = new Question
            {
                Id = questionId,
                Text = "Text1"
            };

            _mockRepo.Setup(x => x.CreateQuestion(It.Is<Question>(q => q.Id == questionId))).Returns(Task.FromResult(question));

            var result = _sut.Post(question.Adapt<QuestionViewModel>()).Result as JsonResult;

            Assert.IsNotNull(result);
            var viewModel = result.GetObjectFromJsonResult<QuestionViewModel>();
            Assert.AreEqual(viewModel.Text, question.Text);
            Assert.AreEqual(viewModel.Id, question.Id);
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
            var questionId = 1;
            var question = new Question
            {
                Id = questionId,
                Text = "Text1"
            };

            _mockRepo.Setup(x => x.CreateQuestion(It.Is<Question>(q => q.Id == questionId))).Throws(new Exception());

            var result = _sut.Post(question.Adapt<QuestionViewModel>()).Result as StatusCodeResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, 500);
        }

        [Test]
        public void Put_WhenCorrectViewModelGiven_ShouldReturnJsonViewModel()
        {
            var questionId = 1;
            var question = new Question
            {
                Id = 1,
                Text = "Text1"
            };

            _mockRepo.Setup(x => x.UpdateQuestion(It.Is<Question>(q => q.Id == questionId))).Returns(Task.FromResult(question));

            var result = _sut.Put(question.Adapt<QuestionViewModel>()).Result as JsonResult;

            Assert.IsNotNull(result);
            var viewModel = result.GetObjectFromJsonResult<QuestionViewModel>();
            Assert.AreEqual(viewModel.Text, question.Text);
            Assert.AreEqual(viewModel.Id, question.Id);
        }

        [Test]
        public void Put_WhenErrorDuringProcessing_ShouldReturnStatusCode500()
        {
            var questionId = 1;
            var question = new Question
            {
                Id = 1,
                Text = "Text1"
            };

            _mockRepo.Setup(x => x.UpdateQuestion(It.Is<Question>(q => q.Id == questionId))).Throws(new Exception());

            var result = _sut.Put(question.Adapt<QuestionViewModel>()).Result as StatusCodeResult;

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
        public void Put_CorrectViewModelErrorDuringProcessing_ShouldReturnNotFound()
        {
            var question = new Question
            {
                Id = 1,
                Text = "Text1"
            };

            _mockRepo.Setup(x => x.UpdateQuestion(question)).Returns<Question>(null);

            var result = _sut.Put(question.Adapt<QuestionViewModel>()).Result;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public void Delete_WhenCorrectViewModelGiven_ShouldReturnJsonViewModel()
        {
            var questionId = 1;

            _mockRepo.Setup(x => x.DeleteQuestion(questionId)).Returns(Task.FromResult(true));

            var result = _sut.Delete(questionId).Result;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public void Delete_WhenCorrectViewModelErrorDuringProcessing_ShouldReturnNotFound()
        {
            var questionId = 2;
            _mockRepo.Setup(x => x.DeleteQuestion(questionId)).Returns(Task.FromResult(false));

            var result = _sut.Delete(questionId).Result;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }
    }
}
