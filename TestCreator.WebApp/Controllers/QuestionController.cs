using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestCreator.Data.Models;
using TestCreator.Data.Repositories.Interfaces;
using TestCreator.WebApp.Converters.Interfaces;
using TestCreator.WebApp.ViewModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TestCreator.WebApp.Controllers
{
    public class QuestionController : BaseApiController
    {
        private readonly IQuestionRepository _repository;

        public QuestionController(IQuestionRepository repository)
        {
            this._repository = repository;
        }

        /// <summary>
        /// GET: api/question/get/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns>QuestionViewModel with given {id}</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var question = await _repository.GetQuestion(id);

                if (question == null)
                {
                    return NotFound(new
                    {
                        Error = $"Question with identifier {id} was not found"
                    });
                }
                
                return new JsonResult(question.Adapt<QuestionViewModel>(), JsonSettings);
            }
            catch (Exception e)
            {
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        /// PUT: api/question/put
        /// </summary>
        /// <param name="viewModel">QuestionViewModel with data</param>
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Put([FromBody]QuestionViewModel viewModel)
        {
            if (viewModel == null)
            {
                return new BadRequestResult();
            }

            try
            {
                var updatedQuestion = await _repository.UpdateQuestion(viewModel.Adapt<Question>());
                if (updatedQuestion == null)
                {
                    return NotFound(new
                    {
                        Error = $"Error during updating question with identifier {viewModel.Id}"
                    });
                }
                return new JsonResult(updatedQuestion.Adapt<QuestionViewModel>(), JsonSettings);
            }
            catch (Exception e)
            {
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        /// POST: api/question/post
        /// </summary>
        /// <param name="viewModel">QuestionViewModel with data</param>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody]QuestionViewModel viewModel)
        {
            if (viewModel == null)
            {
                return new BadRequestResult();
            }

            try
            {
                var createdQuestion = await _repository.CreateQuestion(viewModel.Adapt<Question>());
                return new JsonResult(createdQuestion.Adapt<QuestionViewModel>(), JsonSettings);
            }
            catch (Exception e)
            {
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        /// DELETE: api/question/delete
        /// </summary>
        /// <param name="id">Identifier of QuestionViewModel</param>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (await _repository.DeleteQuestion(id))
                {
                    return new NoContentResult();
                }
                return NotFound(new
                {
                    Error = $"Error during deletion question with identifier {id}"
                });
            }
            catch (Exception e)
            {
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        /// GET: api/question
        /// </summary>
        /// <param name="testId"></param>
        /// <returns>All QuestionViewModel for given {testId}</returns>
        [HttpGet]
        public async Task<IActionResult> GetByTestId([Required][FromQuery(Name = "testId")] int testId)
        {
            try
            {
                var questions = await _repository.GetQuestions(testId);

                if (questions == null)
                {
                    return NotFound(new
                    {
                        Error = $"Questions for test with identifier {testId} were not found"
                    });
                }

                return new JsonResult(questions.Adapt<List<QuestionViewModel>>(), JsonSettings);
            }
            catch (Exception e)
            {
                return new StatusCodeResult(500);
            }
        }
    }
}
