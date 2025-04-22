using Microsoft.AspNetCore.Identity;

using Plantica.Application.DTOs;
using Plantica.Application.Services.Interfaces;
using Plantica.Core.Models;
using Plantica.Infrastructure.Repositories.Interfaces;

namespace Plantica.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(IUserRepository userRepository, IPasswordHasher<User> passwordHaser)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHaser;
        }

        public async Task<User> GetUserByIdAsync(Ulid userId)
        {
            return await _userRepository.GetUserByIdAsync(userId);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _userRepository.GetUserByUsernameAsync(username);
        }

        public async Task<User> RegisterUserAsync(UserRegistrationDto registrationDto)
        {
            // Check arguments
            if (registrationDto == null)
                throw new ArgumentNullException(nameof(registrationDto), "Registration DTO cannot be null.");

            if (string.IsNullOrWhiteSpace(registrationDto.Username))
                throw new ArgumentException("Username cannot be null or empty.", nameof(registrationDto.Username));
            if (string.IsNullOrWhiteSpace(registrationDto.Email))
                throw new ArgumentException("Email cannot be null or empty.", nameof(registrationDto.Email));
            if (string.IsNullOrWhiteSpace(registrationDto.Password))
                throw new ArgumentException("Password cannot be null or empty.", nameof(registrationDto.Password));

            // Validate username is unique
            var existingUser = await _userRepository.GetUserByUsernameAsync(registrationDto.Username);
            if (existingUser != null)
            {
                throw new ApplicationException("Username already exists");
            }

            // Create new user
            var user = new User(registrationDto.Username, registrationDto.Email);

            // Hash password
            user.UpdatePassword(_passwordHasher.HashPassword(user, registrationDto.Password));

            // Save user to database
            return await _userRepository.RegisterUserAsync(user);
        }

        public async Task<User> UpdateUserAsync(Ulid userId, UserUpdateDto updateDto)
        {
            if (userId == Ulid.Empty)
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));

            // Get existing user
            var user = await _userRepository.GetUserByIdAsync(userId);

            // Update user properties if provided
            if (!string.IsNullOrWhiteSpace(updateDto.Name))
            {
                // Check if new username is already taken (only if username is being changed)
                if (updateDto.Name != user.Name.Value)
                {
                    try
                    {
                        var existingUser = await _userRepository.GetUserByUsernameAsync(updateDto.Name);
                        if (existingUser != null && existingUser.Id != userId)
                        {
                            throw new ApplicationException("Username already exists");
                        }
                    }
                    catch (KeyNotFoundException)
                    {
                        // Username is available
                    }

                    user.UpdateName(updateDto.Name);
                }
            }

            if (!string.IsNullOrWhiteSpace(updateDto.Email))
            {
                user.UpdateEmail(updateDto.Email);
            }

            // Save changes
            return await _userRepository.UpdateUserAsync(user);
        }

        public async Task<bool> DeleteUserAsync(Ulid userId)
        {
            if (userId == Ulid.Empty)
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));

            // Verify user exists
            await _userRepository.GetUserByIdAsync(userId);

            // Delete user and convert result to boolean
            var deletedUser = await _userRepository.DeleteUserAsync(userId);

            // Delete user
            return deletedUser != null;
        }

        public async Task<User> AuthenticateUserAsync(UserLoginDto loginDto)
        {
            // Get user by username
            var user = await _userRepository.GetUserByUsernameAsync(loginDto.Username);
            if (user == null)
            {
                throw new ApplicationException("Invalid username or password");
            }
            // Verify password
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                throw new ApplicationException("Invalid username or password");
            }
            return user;
        }
    }
}
