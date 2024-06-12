using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestCreator.Data.Context;
using TestCreator.Data.Models;
using TestCreator.Data.Repositories.Interfaces;
using TestCreator.Data.Repositories.Params;

namespace TestCreator.Data.Repositories
{
    public class TestRepository : ITestRepository
    {
        private readonly ApplicationDbContext _context;

        public TestRepository(ApplicationDbContext context)
        {
            this._context = context;
        }

        public async Task<Test> GetTest(int id)
        {
            return await _context.Tests.FirstOrDefaultAsync(t => t.Id.Equals(id));
        }

        public async Task<Test> GetTestWithInclude(int id)
        {
            return await _context.Tests.Where(t => t.Id.Equals(id))
                .Include(t => t.Questions)
                .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Test>> GetTestsByParam(int number, TestsOrder order)
        {
            IQueryable<Test> tests = null;

            switch (order)
            {
                case TestsOrder.ByTitle:
                    tests = _context.Tests.OrderBy(t => t.Title).Take(number).AsNoTracking();
                    break;
                case TestsOrder.Latest:
                    tests = _context.Tests.OrderByDescending(t => t.CreationDate).Take(number).AsNoTracking();
                    break;
                case TestsOrder.Random:
                    tests = _context.Tests.OrderBy(t => Guid.NewGuid()).Take(number).AsNoTracking();
                    break;
            }

            return await tests!.ToListAsync();
        }

        public async Task<List<Test>> Search(string text, int number)
        {
            return await _context.Tests.Where(t => t.Title.Contains(text))
                .Take(number)
                .ToListAsync();
        }

        public async Task<Test> CreateTest(Test test)
        {
            test.CreationDate = DateTime.Now;
            test.LastModificationDate = DateTime.Now;
            test.UserId = _context.Users.FirstOrDefault(u => u.UserName.Equals("Admin"))?.Id;

            await _context.Tests.AddAsync(test);
            await _context.SaveChangesAsync();

            return test;
        }

        public async Task<Test> UpdateTest(Test test)
        {
            var testToUpdate = await _context.Tests.FirstOrDefaultAsync(t => t.Id.Equals(test.Id));

            if (testToUpdate == null)
            {
                return null;
            }

            testToUpdate.Title = test.Title;
            testToUpdate.Description = test.Description;
            testToUpdate.Text = test.Text;
            testToUpdate.Notes = test.Notes;
            testToUpdate.ViewCount = test.ViewCount;
            testToUpdate.LastModificationDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return testToUpdate;
        }

        public async Task<bool> IncrementTestViewCount(int id)
        {
            var testToUpdate = await _context.Tests.FirstOrDefaultAsync(t => t.Id.Equals(id));

            if (testToUpdate == null)
            {
                throw new Exception($"Test with id: {id} not found.");
            }

            testToUpdate.ViewCount++;

            testToUpdate.LastModificationDate = DateTime.Now;
            
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteTest(int id)
        {
            var test = await _context.Tests.FirstOrDefaultAsync(t => t.Id.Equals(id));

            if (test == null)
            {
                throw new Exception($"Test with id: {id} not found.");
            }

            _context.Tests.Remove(test);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
