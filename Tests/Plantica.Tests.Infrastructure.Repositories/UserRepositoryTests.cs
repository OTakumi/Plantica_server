using FluentAssertions;

using Plantica.Core.Models;
using Plantica.Tests.TestBase;

using Xunit.Abstractions;

namespace Plantica.Infrastructure.Repositories.Tests
{
    public class UserRepositoryTests : RepositoryTestBase
    {
        private readonly UserRepository _repository;
        private readonly ITestOutputHelper _output;

        public UserRepositoryTests(ITestOutputHelper output) : base()
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

        /// <summary>
        /// Tests the RegisterUserAsync method with a valid user.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task RegisterUserAsync_WithValidUser_ReturnsRegisteredUser()
        {
            // Arrange
            var user = new User("TestUser", "test@example.com");

            // Act
            var result = await _repository.RegisterUserAsync(user);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(user.Id);
            result.Name.Value.Should().Be("TestUser");
            result.Email.Should().Be("test@example.com");

            // check the user is saved in the database
            var savedUser = await DbContext.Users.FindAsync(user.Id);
            savedUser.Should().NotBeNull();
            savedUser.Name.Value.Should().Be("TestUser");
            savedUser.Email.Should().Be("test@example.com");
        }

        /// <summary>
        /// Tests the RegisterUserAsync method with a null user.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task RegisterUserAsync_WithNullUser_ThrowsArgumentNullException()
        {
            // Arrange
            // Act
            // Assert
        }

        /// <summary>
        /// Tests the RegisterUserAsync method with a duplicate user.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task RegisterUserAsync_DuplicateUser_ThrowsException()
        {
            // Arrange
            // Act
            // Assert
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
        public async Task DeleteUserAsync_WithValidId_MarksUserAsDeleted()
        {
            // Arrange
            // Act
            // Assert
        }

        [Fact]
        public async Task DeleteUserAsync_WithNonExistentId_ThrowsKeyNotFoundException()
        {
            // Arrange
            // Act
            // Assert
        }

        [Fact]
        public async Task DeleteUserAsync_WithEmptyId_ThrowsArgumentException()
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