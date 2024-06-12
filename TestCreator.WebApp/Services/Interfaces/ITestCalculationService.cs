using TestCreator.WebApp.ViewModels;

namespace TestCreator.WebApp.Services.Interfaces
{
    public interface ITestCalculationService
    {
        TestAttemptResultViewModel CalculateResult(TestAttemptViewModel viewModel);
    }
}
