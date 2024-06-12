using System.Collections.Generic;
using System.Threading.Tasks;
using TestCreator.Data.Models;

namespace TestCreator.Data.Repositories.Interfaces
{
    public interface IResultRepository
    {
        Task<Result> GetResult(int id);
        Task<List<Result>> GetResults(int testId);
        Task<Result> CreateResult(Result result);
        Task<Result> UpdateResult(Result result);
        Task<bool> DeleteResult(int id);
    }
}
