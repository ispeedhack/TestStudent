using System.Collections.Generic;
using Mapster;
using TestCreator.Data.Models;
using TestCreator.WebApp.Converters.Interfaces;
using TestCreator.WebApp.ViewModels;

namespace TestCreator.WebApp.Converters
{
    public class TestAttemptViewModelConverter : ITestAttemptViewModelConverter
    {
        public TestAttemptViewModel Convert(Test test)
        {
            if (test == null)
            {
                return null;
            }

            var viewModel = new TestAttemptViewModel
            {
                TestId = test.Id,
                Title = test.Title,
                TestAttemptEntries = new List<TestAttemptEntryViewModel>()
            };

            foreach (var question in test.Questions)
            {
                viewModel.TestAttemptEntries.Add(new TestAttemptEntryViewModel
                {
                    Question = question.Adapt<QuestionViewModel>(),
                    Answers = question.Answers.Adapt<List<TestAttemptAnswerViewModel>>()
                });
            }

            return viewModel;
        }
    }
}
