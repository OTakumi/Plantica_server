using Plantica.Core.Models;

namespace Plantica.Infrastructure.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(Ulid userId);

        /// <summary>
        /// Get user information by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns>Return null if no user name is found, as it may not be registered</returns>
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User> RegisterUserAsync(User user);
        Task<User> UpdateUserAsync(User user);
        Task<User> DeleteUserAsync(Ulid userId);
    }
}
