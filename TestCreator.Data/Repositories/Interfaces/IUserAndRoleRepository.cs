using System.Threading.Tasks;
using TestCreator.Data.Models;

namespace TestCreator.Data.Repositories.Interfaces
{
    public interface IUserAndRoleRepository
    {
        Task<ApplicationUser> GetUserByNameAsync(string userName);
        Task<ApplicationUser> GetUserByEmailAsync(string email);
        Task<ApplicationUser> GetUserById(string userId);
        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
        Task<ApplicationUser> CreateUserAndAddToRolesAsync(ApplicationUser user, string[] roles);
    }
}
