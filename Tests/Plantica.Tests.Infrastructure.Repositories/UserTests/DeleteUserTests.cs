using FluentAssertions;

using Plantica.Infrastructure.Repositories;
using Plantica.Tests.TestBase;

using Xunit.Abstractions;

namespace Plantica.Tests.Infrastructure.Repositories.UserTests
{
    public class DeleteUserTests : RepositoryTestBase
    {
        private readonly UserRepository _repository;
        private readonly ITestOutputHelper _output;

        public DeleteUserTests(ITestOutputHelper output) : base()
        {
            _repository = new UserRepository(DbContext);
            _output = output;
        }

        [Fact]
        public async Task DeleteUserAsync_WithValidId_MarksUserAsDeleted()
        {
            // Arrange
            var user = await AddUserAsync("testuser", "test@example.com");

            // Act
            var result = await _repository.DeleteUserAsync(user.Id);

            // Assert
            result.Should().NotBeNull();
            result.IsDeleted.Should().BeTrue();

            // Verify user is marked as deleted in the database
            var deletedUser = await DbContext.Users.FindAsync(user.Id);
            deletedUser.Should().NotBeNull();
            deletedUser!.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteUserAsync_WithNonExistentId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var nonExistentId = Ulid.NewUlid();

            // Act
            Func<Task> act = async () => await _repository.DeleteUserAsync(nonExistentId);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage($"User with ID {nonExistentId} not found.");
        }

        [Fact]
        public async Task DeleteUserAsync_WithEmptyId_ThrowsArgumentException()
        {
            // Arrange
            var emptyId = Ulid.Empty;

            // Act
            Func<Task> act = async () => await _repository.DeleteUserAsync(emptyId);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("User ID cannot be empty.*")
                .WithParameterName("userId");
        }
    }
}
