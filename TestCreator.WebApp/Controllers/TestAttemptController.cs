using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TestCreator.Data.Repositories.Interfaces;
using TestCreator.WebApp.Converters.Interfaces;
using TestCreator.WebApp.Services.Interfaces;
using TestCreator.WebApp.ViewModels;

namespace TestCreator.WebApp.Controllers
{
    [ApiController]
    public class TestAttemptController : BaseApiController
    {
        private readonly ITestCalculationService _service;
        private readonly ITestRepository _repository;
        private readonly ITestAttemptViewModelConverter _converter;

        public TestAttemptController(ITestCalculationService service, ITestRepository repository, ITestAttemptViewModelConverter converter)
        {
            _service = service;
            _repository = repository;
            _converter = converter;
        }

        /// <summary>
        /// GET: api/testAttempt/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Single TestAttemptViewModel with given {id}</returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var test = await _repository.GetTestWithInclude(id);
                var viewModel = _converter.Convert(test);

                if (viewModel == null)
                {
                    return NotFound(new
                    {
                        Error = $"Test with identifier {id} was not found"
                    });
                }
                return new JsonResult(viewModel, JsonSettings);
            }
            catch (Exception e)
            {
                return new StatusCodeResult(500);
            }
        }


        /// <summary>
        /// POST: api/testAttempt
        /// </summary>
        /// <param name="viewModel">TestAttemptViewModel with data</param>
        /// <returns>Calculate result and return TestAttemptResultViewModel for given {viewModel}</returns>
        [HttpPost]
        public IActionResult CalculateResult([FromBody] TestAttemptViewModel viewModel)
        {
            if (viewModel == null)
            {
                return new BadRequestResult();
            }

            try
            {
                var resultViewModel = _service.CalculateResult(viewModel);

                if (resultViewModel == null)
                {
                    return new StatusCodeResult(500);
                }

                return new JsonResult(resultViewModel, JsonSettings);
            }
            catch (Exception e)
            {
                return new StatusCodeResult(500);
            }
        }
    }
}
