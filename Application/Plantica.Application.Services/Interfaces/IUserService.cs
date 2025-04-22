using Plantica.Application.DTOs;
using Plantica.Core.Models;

namespace Plantica.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(Ulid userId);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User> RegisterUserAsync(UserRegistrationDto registrationDto);
        Task<User> UpdateUserAsync(Ulid userId, UserUpdateDto updateDto);
        Task<User> DeleteUserAsync(Ulid userId);
        Task<User> AuthenticateUserAsync(UserLoginDto loginDto);
    }
}
