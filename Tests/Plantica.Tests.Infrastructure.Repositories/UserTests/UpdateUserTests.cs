using FluentAssertions;

using Plantica.Core.Models;
using Plantica.Infrastructure.Repositories;
using Plantica.Tests.TestBase;

using Xunit.Abstractions;

namespace Plantica.Tests.Infrastructure.Repositories.UserTests
{
    public class UpdateUserTests : RepositoryTestBase
    {
        private readonly UserRepository _repository;
        private readonly ITestOutputHelper _output;

        public UpdateUserTests(ITestOutputHelper output) : base()
        {
            _repository = new UserRepository(DbContext);
            _output = output;
        }

        [Fact]
        public async Task UpdateUserAsync_WithValidUser_ReturnsUpdatedUser()
        {
            // Arrange
            var user = await AddUserAsync("testuser", "test@example.com");
            _output.WriteLine($"User ID: {user.Id}");               // Log the user ID for debugging
            _output.WriteLine($"User Name: {user.Name.Value}");     // Log the user name for debugging
            _output.WriteLine($"User Email: {user.Email}");         // Log the user email for debugging

            // Update the user's properties
            user.UpdateName("updateduser");
            user.UpdateEmail("updated@example.com");

            // Act
            var result = await _repository.UpdateUserAsync(user);
            _output.WriteLine($"Updated User ID: {result.Id}");             // Log the updated user ID for debugging
            _output.WriteLine($"Updated User Name: {result.Name.Value}");   // Log the updated user name for debugging
            _output.WriteLine($"Updated User Email: {result.Email}");       // Log the updated user email for debugging

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(user.Id);
            result.Name.Value.Should().Be("updateduser");
            result.Email.Should().Be("updated@example.com");

            // Verify changes are saved to database
            var updatedUser = await DbContext.Users.FindAsync(user.Id);
            updatedUser.Should().NotBeNull();
            updatedUser!.Name.Value.Should().Be("updateduser");
            updatedUser.Email.Should().Be("updated@example.com");
        }

        [Fact]
        public async Task UpdateUserAsync_WithNonExistentUser_ThrowsKeyNotFoundException()
        {
            // Arrange
            var nonExistentUser = new User("nonexistent", "nonexistent@example.com");

            // Act & Assert
            Func<Task> act = async () => await _repository.UpdateUserAsync(nonExistentUser);

            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage($"User with ID {nonExistentUser.Id} not found.");
        }

        [Fact]
        public async Task UpdateUserAsync_WithNullUser_ThrowsArgumentNullException()
        {
            // Arrange
            User? nullUser = null;

            // Act & Assert
            Func<Task> act = async () => await _repository.UpdateUserAsync(nullUser!);

            await act.Should().ThrowAsync<ArgumentNullException>()
                .WithMessage("User cannot be null.*")
                .WithParameterName("user");
        }
    }
}
