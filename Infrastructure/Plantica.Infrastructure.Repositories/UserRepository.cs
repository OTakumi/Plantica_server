using Microsoft.EntityFrameworkCore;

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

        /// <summary>
        /// Get user information by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be null or empty.", nameof(username));

            // Search for the user by username
            // UserName is a value object, so we need to search from own entity of EFCore
            var user = await _context.Users
                .Where(u => u.IsDeleted == false)
                .FirstOrDefaultAsync(u => u.Name.Value == username);

            // Return null if the user is not found
            return user;
        }

        /// <summary>
        /// Registers a new user in the database.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<User> RegisterUserAsync(User user)
        {
            // Check the type of the user
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User cannot be null.");

            try
            {
                // Check if the user already exists
                var existingUser = await GetUserByUsernameAsync(user.Name.Value.ToString());

                // If exsistingUser is null and you reach this point, it means that the user has been found.
                if (existingUser != null)
                    throw new InvalidOperationException($"User with username {user.Name.Value} already exists.");
            }
            catch (KeyNotFoundException)
            {
                // User does not exist, continue with registration
            }

            // Register the user
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        /// <summary>
        /// Updates the user information.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<User> UpdateUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User cannot be null.");

            // Check if the user exists
            var existingUser = await GetUserByIdAsync(user.Id);

            // Update the user
            _context.Entry(existingUser).CurrentValues.SetValues(user);
            await _context.SaveChangesAsync();

            return existingUser;
        }

        public async Task<User> DeleteUserAsync(Ulid userId)
        {
            if (userId == Ulid.Empty)
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));

            // Check if the user exists
            var existingUser = await GetUserByIdAsync(userId);

            // Mark the user as deleted
            existingUser.IsDeleted = true;
            _context.Entry(existingUser).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return existingUser;
        }
    }
}
