using AuthenticationService.Entities;

namespace AuthenticationService.Services
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByIdAsync(Guid id);
        Task UpdateAsync(User user);
        Task<List<User>> GetAllUsers(); 
    }
}
