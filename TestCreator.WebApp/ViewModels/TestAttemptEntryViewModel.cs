using System.Collections.Generic;
using System.Linq;

namespace TestCreator.WebApp.ViewModels
{
    public class TestAttemptEntryViewModel
    {
        public QuestionViewModel Question { get; set; }
        public List<TestAttemptAnswerViewModel> Answers { get; set; }
        public bool IsMultitipleChoise => Answers.Count(a => a.Value > 0) > 1;
    }
}
