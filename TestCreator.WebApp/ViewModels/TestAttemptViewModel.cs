using System.Collections.Generic;

namespace TestCreator.WebApp.ViewModels
{
    public class TestAttemptViewModel
    {
        public int TestId { get; set; }
        public string Title { get; set; }
        public string UserId { get; set; }

        public List<TestAttemptEntryViewModel> TestAttemptEntries { get; set; }
    }
}
