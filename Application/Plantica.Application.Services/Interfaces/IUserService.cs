using Plantica.Core.Models;

namespace Plantica.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(Ulid userId);
        Task<User> GetUserByUsernameAsync(string username);
    }
}
