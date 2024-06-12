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
    public class AnswerRepository : IAnswerRepository
    {
        private readonly ApplicationDbContext _context;

        public AnswerRepository(ApplicationDbContext context)
        {
            this._context = context;
        }

        public async Task<Answer> GetAnswer(int id)
        {
            return await _context.Answers.FirstOrDefaultAsync(t => t.Id.Equals(id));
        }

        public async Task<List<Answer>> GetAnswers(int questionId)
        {
            return await _context.Answers.Where(t => t.QuestionId == questionId).ToListAsync();
        }

        public async Task<Answer> CreateAnswer(Answer answer)
        {
            answer.CreationDate = DateTime.Now;
            answer.LastModificationDate = DateTime.Now;

            await _context.Answers.AddAsync(answer);
            await _context.SaveChangesAsync();

            return answer;
        }

        public async Task<Answer> UpdateAnswer(Answer answer)
        {
            var answerToUpdate = await _context.Answers.FirstOrDefaultAsync(t => t.Id.Equals(answer.Id));

            if (answerToUpdate == null)
            {
                return null;
            }

            answerToUpdate.QuestionId = answer.QuestionId;
            answerToUpdate.Text = answer.Text;
            answerToUpdate.Notes = answer.Notes;
            answerToUpdate.Value = answer.Value;

            answerToUpdate.LastModificationDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return answerToUpdate;
        }

        public async Task<bool> DeleteAnswer(int id)
        {
            var answer = await _context.Answers.FirstOrDefaultAsync(t => t.Id.Equals(id));

            if (answer == null)
            {
                return false;
            }

            _context.Answers.Remove(answer);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
