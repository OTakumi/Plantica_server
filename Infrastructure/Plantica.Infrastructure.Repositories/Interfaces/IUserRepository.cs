using Plantica.Core.Models;

namespace Plantica.Infrastructure.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(Ulid userId);
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> RegisterUserAsync(User user);
        Task<User> UpdateUserAsync(User user);
        Task<User> DeleteUserAsync(Ulid userId);
    }
}
