using System.Collections.Generic;
using System.Threading.Tasks;
using TestCreator.Data.Models;

namespace TestCreator.Data.Repositories.Interfaces
{
    public interface IAnswerRepository
    {
        Task<Answer> GetAnswer(int id);
        Task<List<Answer>> GetAnswers(int questionId);
        Task<Answer> CreateAnswer(Answer answer);
        Task<Answer> UpdateAnswer(Answer answer);
        Task<bool> DeleteAnswer(int id);
    }
}
