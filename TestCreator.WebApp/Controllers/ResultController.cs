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
    public class ResultController : BaseApiController
    {
        private readonly IResultRepository _repository;

        public ResultController(IResultRepository repository)
        {
            this._repository = repository;
        }

        /// <summary>
        /// GET: api/result/get/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns>ResultViewModel with given {id}</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var result = await _repository.GetResult(id);

                if (result == null)
                {
                    return NotFound(new
                    {
                        Error = $"Result with identifier {id} was not found"
                    });
                }

                return new JsonResult(result.Adapt<ResultViewModel>(), JsonSettings);
            }
            catch (Exception e)
            {
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        /// PUT: api/result/put
        /// </summary>
        /// <param name="viewModel">ResultViewModel with data</param>
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Put([FromBody]ResultViewModel viewModel)
        {
            if (viewModel == null)
            {
                return new BadRequestResult();
            }

            try
            {
                var updatedResult = await _repository.UpdateResult(viewModel.Adapt<Result>());
                if (updatedResult == null)
                {
                    return NotFound(new
                    {
                        Error = $"Error during updating result with identifier {viewModel.Id}"
                    });
                }
                return new JsonResult(updatedResult.Adapt<ResultViewModel>(), JsonSettings);
            }
            catch (Exception e)
            {
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        /// POST: api/result/post
        /// </summary>
        /// <param name="viewModel">ResultViewModel with data</param>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody]ResultViewModel viewModel)
        {
            if (viewModel == null)
            {
                return new BadRequestResult();
            }

            try
            {
                var createdResult = await _repository.CreateResult(viewModel.Adapt<Result>());
                return new JsonResult(createdResult.Adapt<ResultViewModel>(), JsonSettings);
            }
            catch (Exception e)
            {
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        /// DELETE: api/result/delete
        /// </summary>
        /// <param name="id">Identifier of ResultViewModel</param>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (await _repository.DeleteResult(id))
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
        /// GET: api/result
        /// </summary>
        /// <param name="testId"></param>
        /// <returns>All ResultViewModel for given {testId}</returns>
        [HttpGet]
        public async Task<IActionResult> GetByTestId([Required][FromQuery(Name = "testId")] int testId)
        {
            try
            {
                var results = await _repository.GetResults(testId);

                if (results == null)
                {
                    return NotFound(new
                    {
                        Error = $"Results for test with identifier {testId} were not found"
                    });
                }

                return new JsonResult(results.Adapt<List<ResultViewModel>>(), JsonSettings);
            }
            catch (Exception e)
            {
                return new StatusCodeResult(500);
            }
        }
    }
}
