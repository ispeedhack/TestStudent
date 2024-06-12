using System.Collections.Generic;
using System.Threading.Tasks;
using TestCreator.Data.Models;
using TestCreator.Data.Repositories.Params;

namespace TestCreator.Data.Repositories.Interfaces
{
    public interface ITestRepository
    {
        Task<Test> GetTest(int id);
        Task<Test> GetTestWithInclude(int id);
        Task<List<Test>> GetTestsByParam(int number, TestsOrder order);
        Task<List<Test>> Search(string text, int number);
        Task<Test> CreateTest(Test test);
        Task<Test> UpdateTest(Test test);
        Task<bool> DeleteTest(int id);
        Task<bool> IncrementTestViewCount(int id);
    }
}
