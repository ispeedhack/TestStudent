using System.Threading.Tasks;
using TestCreator.Data.Models;

namespace TestCreator.Data.Repositories.Interfaces
{
    public interface ITokenRepository
    {
        Task<Token> CheckRefreshTokenForClient(string clientId, string refreshToken);
        Task RemoveRefreshToken(Token refreshToken);
        Task AddRefreshToken(Token refreshToken);
        
    }
}
