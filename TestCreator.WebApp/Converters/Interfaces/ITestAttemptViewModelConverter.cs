using TestCreator.Data.Models;
using TestCreator.WebApp.ViewModels;

namespace TestCreator.WebApp.Converters.Interfaces
{
    public interface ITestAttemptViewModelConverter
    {
        TestAttemptViewModel Convert(Test test);
    }
}
