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

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _userRepository.GetUserByUsernameAsync(username);
        }

        public async Task<User> RegisterUserAsync(UserRegistrationDto registrationDto)
        {
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
    }
}
