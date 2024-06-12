using System.Collections.Generic;
using System.Threading.Tasks;
using TestCreator.Data.Models;

namespace TestCreator.Data.Repositories.Interfaces
{
    public interface IQuestionRepository
    {
        Task<Question> GetQuestion(int id);
        Task<List<Question>> GetQuestions(int testId);
        Task<Question> CreateQuestion(Question question);
        Task<Question> UpdateQuestion(Question question);
        Task<bool> DeleteQuestion(int id);
    }
}
