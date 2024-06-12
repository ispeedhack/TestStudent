using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestCreator.Data.Models;
using TestCreator.Data.Repositories.Interfaces;
using TestCreator.WebApp.ViewModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TestCreator.WebApp.Controllers
{
    public class AnswerController : BaseApiController
    {
        private readonly IAnswerRepository _repository;

        public AnswerController(IAnswerRepository repository)
        {
            this._repository = repository;
        }

        /// <summary>
        /// GET: api/answer/
        /// </summary>
        /// <returns>All AnswerViewModel for given {questionId}</returns>
        [HttpGet]
        public async Task<IActionResult> GetByQuestionId([Required][FromQuery(Name = "questionId")] int questionId)
        {
            try
            {
                var answers = await _repository.GetAnswers(questionId);

                if (answers == null)
                {
                    return NotFound(new
                    {
                        Error = $"Answers for question with identifier {questionId} were not found"
                    });
                }

                return new JsonResult(answers.Adapt<List<AnswerViewModel>>(), JsonSettings);
            }
            catch (Exception e)
            {
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        /// GET: api/answer/get/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns>AnswerViewModel with given {id}</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var answer = await _repository.GetAnswer(id);

                if (answer == null)
                {
                    return NotFound(new
                    {
                        Error = $"Answer with identifier {id} was not found"
                    });
                }

                return new JsonResult(answer.Adapt<AnswerViewModel>(), JsonSettings);
            }
            catch (Exception e)
            {
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        /// PUT: api/answer/put
        /// </summary>
        /// <param name="viewModel">AnswerViewModel with data</param>
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Put([FromBody]AnswerViewModel viewModel)
        {
            if (viewModel == null)
            {
                return new BadRequestResult();
            }

            try
            {
                var updatedAnswer = await _repository.UpdateAnswer(viewModel.Adapt<Answer>());
                if (updatedAnswer == null)
                {
                    return NotFound(new
                    {
                        Error = $"Error during updating answer with identifier {viewModel.Id}"
                    });
                }
                return new JsonResult(updatedAnswer.Adapt<AnswerViewModel>(), JsonSettings);
            }
            catch (Exception e)
            {
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        /// POST: api/answer/post
        /// </summary>
        /// <param name="viewModel">AnswerViewModel with data</param>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody]AnswerViewModel viewModel)
        {
            if (viewModel == null)
            {
                return new BadRequestResult();
            }

            try
            {
                var createdAnswer = await _repository.CreateAnswer(viewModel.Adapt<Answer>());
                return new JsonResult(createdAnswer.Adapt<AnswerViewModel>(), JsonSettings);
            }
            catch (Exception e)
            {
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        /// DELETE: api/answer/delete
        /// </summary>
        /// <param name="id">Identifier of AnswerViewModel</param>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (await _repository.DeleteAnswer(id))
                {
                    return new NoContentResult();
                }
                return NotFound(new
                {
                    Error = $"Error during deletion answer with identifier {id}"
                });
            }
            catch (Exception e)
            {
                return new StatusCodeResult(500);
            }
        }
    }
}
