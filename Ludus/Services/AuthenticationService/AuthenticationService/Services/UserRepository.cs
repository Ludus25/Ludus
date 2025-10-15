using AuthenticationService.Entities;
using Microsoft.EntityFrameworkCore;
using AuthenticationService.Data;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationService.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<User> _userManager;

        public UserRepository(AppDbContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager= userManager;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                return null;

            return await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            return await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == id.ToString());
        }
        public async Task<List<User>> GetAllUsers()
        {
            var users = await _userManager.GetUsersInRoleAsync("User");
            return users.ToList();
        }
        public async Task UpdateAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
        }
    }
}
