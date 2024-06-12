using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestCreator.Data.Context;
using TestCreator.Data.Models;
using TestCreator.Data.Repositories.Interfaces;

namespace TestCreator.Data.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly ApplicationDbContext _context;

        public QuestionRepository(ApplicationDbContext context)
        {
            this._context = context;
        }

        public async Task<Question> GetQuestion(int id)
        {
            return await _context.Questions.FirstOrDefaultAsync(t => t.Id.Equals(id));
        }

        public async Task<List<Question>> GetQuestions(int testId)
        {
            return await _context.Questions.Where(t => t.TestId == testId).ToListAsync();
        }

        public async Task<Question> CreateQuestion(Question question)
        {
            question.CreationDate = DateTime.Now;
            question.LastModificationDate = DateTime.Now;

            await _context.Questions.AddAsync(question);
            await _context.SaveChangesAsync();

            return question;
        }

        public async Task<Question> UpdateQuestion(Question question)
        {
            var questionToUpdate = await _context.Questions.FirstOrDefaultAsync(t => t.Id.Equals(question.Id));

            if (questionToUpdate == null)
            {
                return null;
            }

            questionToUpdate.TestId = question.TestId;
            questionToUpdate.Text = question.Text;
            questionToUpdate.Notes = question.Notes;

            questionToUpdate.LastModificationDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return questionToUpdate;
        }

        public async Task<bool> DeleteQuestion(int id)
        {
            var question = await _context.Questions.FirstOrDefaultAsync(t => t.Id.Equals(id));

            if (question == null)
            {
                return false;
            }

            _context.Questions.Remove(question);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
