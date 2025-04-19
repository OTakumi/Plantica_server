using FluentAssertions;

using Plantica.Infrastructure.Repositories;
using Plantica.Tests.TestBase;

using Xunit.Abstractions;

namespace Plantica.Tests.Infrastructure.Repositories.UserTest
{
    public class GetUserTests : RepositoryTestBase
    {
        private readonly UserRepository _repository;
        private readonly ITestOutputHelper _output;

        public GetUserTests(ITestOutputHelper output) : base()
        {
            _repository = new UserRepository(DbContext);
            _output = output;
        }

        /// <summary>
        /// Tests the GetUserByIdAsync method with a valid user ID.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetUserByIdAsync_WithValidId_ReturnUser()
        {
            // Arrange
            var user = await AddUserAsync("testuser", "test@example.com");

            // Act
            var result = await _repository.GetUserByIdAsync(user.Id);

            // Assert
            result.Should().NotBeNull();
            result.Name.Value.Should().Be("testuser");
            result.Email.Should().Be("test@example.com");
        }

        /// <summary>
        /// Tests the GetUserByIdAsync method with an invalid user ID.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetUserByIdAsync_WithInvalidId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var invalidId = Ulid.NewUlid();

            // Act
            Func<Task> act = () => _repository.GetUserByIdAsync(invalidId);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage($"User with ID {invalidId} not found.");
        }

        /// <summary>
        /// Tests the GetUserByIdAsync method with an empty user ID.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetUserByIdAsync_WithEmptyId_ThrowsArgumentException()
        {
            // Arrange
            var emptyId = Ulid.Empty;

            // Act
            Func<Task> act = async () => await _repository.GetUserByIdAsync(emptyId);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("User ID cannot be empty.*")
                .WithParameterName("userId");
        }

        [Fact]
        public async Task GetUserByUsernameAsync_WithValidUsername_ReturnsUser()
        {
            // Arrange
            // Act
            // Assert
        }

        [Fact]
        public async Task GetUserByUsernameAsync_WithNonExistentUsername_ThrowsKeyNotFoundException()
        {
            // Arrange
            // Act
            // Assert
        }

        [Fact]
        public async Task GetUserByUsernameAsync_WithNullOrEmptyUsername_ThrowsArgumentException()
        {
            // Arrange
            // Act
            // Assert
        }

        [Fact]
        public async Task UpdateUserAsync_WithValidUser_ReturnsUpdatedUser()
        {
            // Arrange
            // Act
            // Assert
        }

        [Fact]
        public async Task UpdateUserAsync_WithNonExistentUser_ThrowsKeyNotFoundException()
        {
            // Arrange
            // Act
            // Assert
        }

        [Fact]
        public async Task UpdateUserAsync_WithNullUser_ThrowsArgumentNullException()
        {
            // Arrange
            // Act
            // Assert
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsAllActiveUsers()
        {
            // Arrange
            // Act
            // Assert
        }

        [Fact]
        public async Task GetAllUsersAsync_WithIncludeDeleted_ReturnsAllUsers()
        {
            // Arrange
            // Act
            // Assert
        }
    }
}