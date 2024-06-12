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
    public class ResultRepository : IResultRepository
    {
        private readonly ApplicationDbContext _context;

        public ResultRepository(ApplicationDbContext context)
        {
            this._context = context;
        }

        public async Task<Result> GetResult(int id)
        {
            return await _context.Results.FirstOrDefaultAsync(t => t.Id.Equals(id));
        }

        public async Task<List<Result>> GetResults(int testId)
        {
            return await _context.Results.Where(t => t.TestId == testId).ToListAsync();
        }

        public async Task<Result> CreateResult(Result result)
        {
            result.CreationDate = DateTime.Now;
            result.LastModificationDate = DateTime.Now;

            await _context.Results.AddAsync(result);
            await _context.SaveChangesAsync();

            return result;
        }

        public async Task<Result> UpdateResult(Result result)
        {
            var resultToUpdate = await _context.Results.FirstOrDefaultAsync(t => t.Id.Equals(result.Id));

            if (resultToUpdate == null)
            {
                return null;
            }

            resultToUpdate.TestId = result.TestId;
            resultToUpdate.Text = result.Text;
            resultToUpdate.Notes = result.Notes;
            resultToUpdate.MinValue = result.MinValue;
            resultToUpdate.MaxValue = result.MaxValue;

            resultToUpdate.LastModificationDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return result;
        }

        public async Task<bool> DeleteResult(int id)
        {
            var result = await _context.Results.FirstOrDefaultAsync(t => t.Id.Equals(id));

            if (result == null)
            {
                return false;
            }

            _context.Results.Remove(result);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
