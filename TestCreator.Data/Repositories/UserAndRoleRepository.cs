using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TestCreator.Data.Context;
using TestCreator.Data.Models;
using TestCreator.Data.Repositories.Interfaces;

namespace TestCreator.Data.Repositories
{
    public class UserAndRoleRepository : IUserAndRoleRepository
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public UserAndRoleRepository(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }

        public async Task<ApplicationUser> GetUserByNameAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<ApplicationUser> GetUserById(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<ApplicationUser> CreateUserAndAddToRolesAsync(ApplicationUser user, string[] roles)
        {
            var newUser = new ApplicationUser
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = user.UserName,
                Email = user.Email,
                DisplayName = user.DisplayName,
                CreationDate = DateTime.Now,
                LastModificationDate = DateTime.Now
            };

            var createResult = await _userManager.CreateAsync(newUser);

            var addToRoleResult = await _userManager.AddToRolesAsync(newUser, roles);

            if (createResult.Succeeded && addToRoleResult.Succeeded)
            {
                newUser.EmailConfirmed = true;
                newUser.LockoutEnabled = false;

                await _context.SaveChangesAsync();

                return newUser;
            }
            else
            {
                throw new Exception("Error during creating user.");
            }
        }
    }
}
