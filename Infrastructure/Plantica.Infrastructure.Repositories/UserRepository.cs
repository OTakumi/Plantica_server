using Plantica.Core.Models;
using Plantica.Infrastructure.Data;
using Plantica.Infrastructure.Repositories.Interfaces;

namespace Plantica.Infrastructure.Repositories
{
    public class UserRepository(ApplicationDbContext context) : IUserRepository
    {
        private readonly ApplicationDbContext _context = context;

        /// <summary>  
        /// Get user information by user ID  
        /// </summary>  
        /// <param name="userId"></param>  
        /// <returns>user</returns>  
        /// <exception cref="ArgumentException"></exception>  
        /// <exception cref="KeyNotFoundException"></exception>  
        public async Task<User> GetUserByIdAsync(Ulid userId)
        {
            if (userId == Ulid.Empty)
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));

            // Returns an error if the user is not found  
            var user = await _context.Users.FindAsync(userId);

            return user ?? throw new KeyNotFoundException($"User with ID {userId} not found.");
        }

        public Task<User> GetUserByUsernameAsync(string username)
        {
            throw new NotImplementedException();
        }

        public async Task<User> RegisterUserAsync(User user)
        {
            // Check the type of the user
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User cannot be null.");

            // Check if the user already exists
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }
    }
}
