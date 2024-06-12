using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestCreator.Data.Context;
using TestCreator.Data.Models;
using TestCreator.Data.Repositories.Interfaces;

namespace TestCreator.Data.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly ApplicationDbContext _context;

        public TokenRepository(ApplicationDbContext context)
        {
            this._context = context;
        }

        public async Task<Token> CheckRefreshTokenForClient(string clientId, string refreshToken)
        {
            return await _context.Tokens.FirstOrDefaultAsync(t => t.ClientId == clientId && t.Value == refreshToken);
        }

        public async Task RemoveRefreshToken(Token refreshToken)
        {
            _context.Tokens.Remove(refreshToken); 
            await _context.SaveChangesAsync();
        }


        public async Task AddRefreshToken(Token refreshToken)
        {
            await _context.Tokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
        }
    }
}
