using FluentAssertions;

using Microsoft.AspNetCore.Identity;

using Moq;

using Plantica.Application.DTOs;
using Plantica.Core.Models;

namespace Plantica.Tests.Application.Services.UserTest
{
    /// <summary>
    /// Test class for UserService functionality.
    /// Inherits from ServiceTestBase which provides common test setup.
    /// </summary>
    public class UserServiceTests : ServiceTestBase
    {
        /// <summary>
        /// Initializes a new instance of the UserServiceTests class.
        /// </summary>
        public UserServiceTests() : base()
        {
        }

        #region GetUserByIdAsync Tests

        /// <summary>
        /// Tests that GetUserByIdAsync returns the correct user when provided with a valid user ID.
        /// </summary>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task GetUserByIdAsync_WithValidId_ReturnsUser()
        {
            // Arrange
            var user = CreateTestUser("testuser", "test@example.com", setupInRepository: true);

            // Act
            var result = await UserService.GetUserByIdAsync(user.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(user.Id);
            result.Name.Value.Should().Be("testuser");
            result.Email.Should().Be("test@example.com");

            MockUserRepository.Verify(repo => repo.GetUserByIdAsync(user.Id), Times.Once);
        }

        /// <summary>
        /// Tests that GetUserByIdAsync throws KeyNotFoundException when provided with an invalid (non-existent) user ID.
        /// </summary>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task GetUserByIdAsync_WithInvalidId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var invalidId = Ulid.NewUlid();
            SetupUserNotFound(invalidId, "nonexistent");

            // Act
            Func<Task> act = async () => await UserService.GetUserByIdAsync(invalidId);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage($"User with ID {invalidId} not found.");

            MockUserRepository.Verify(repo => repo.GetUserByIdAsync(invalidId), Times.Once);
        }

        /// <summary>
        /// Tests that GetUserByIdAsync throws ArgumentException when provided with an empty user ID.
        /// </summary>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task GetUserByIdAsync_WithEmptyId_ThrowsArgumentException()
        {
            // Arrange
            var emptyId = Ulid.Empty;
            MockUserRepository
                .Setup(repo => repo.GetUserByIdAsync(emptyId))
                .ThrowsAsync(new ArgumentException("User ID cannot be empty.", nameof(emptyId)));

            // Act
            Func<Task> act = async () => await UserService.GetUserByIdAsync(emptyId);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("User ID cannot be empty.*");

            MockUserRepository.Verify(repo => repo.GetUserByIdAsync(emptyId), Times.Once);
        }

        #endregion

        #region GetUserByUsernameAsync Tests

        /// <summary>
        /// Tests that GetUserByUsernameAsync returns the correct user when provided with a valid username.
        /// </summary>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task GetUserByUsernameAsync_WithValidUsername_ReturnsUser()
        {
            // Arrange
            var username = "testuser";
            var user = CreateTestUser(username, "test@example.com", setupInRepository: true);

            // Act
            var result = await UserService.GetUserByUsernameAsync(username);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(user.Id);
            result.Name.Value.Should().Be(username);
            result.Email.Should().Be("test@example.com");

            MockUserRepository.Verify(repo => repo.GetUserByUsernameAsync(username), Times.Once);
        }

        /// <summary>
        /// Tests that GetUserByUsernameAsync throws KeyNotFoundException when provided with a non-existent username.
        /// </summary>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task GetUserByUsernameAsync_WithNonExistentUsername_ThrowsKeyNotFoundException()
        {
            // Arrange
            var username = "nonexistentuser";
            SetupUserNotFound(Ulid.NewUlid(), username);

            // Act
            Func<Task> act = async () => await UserService.GetUserByUsernameAsync(username);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage($"User with username '{username}' not found.");

            MockUserRepository.Verify(repo => repo.GetUserByUsernameAsync(username), Times.Once);
        }

        /// <summary>
        /// Tests that GetUserByUsernameAsync throws ArgumentException when provided with a null or empty username.
        /// </summary>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task GetUserByUsernameAsync_WithNullOrEmptyUsername_ThrowsArgumentException()
        {
            // Arrange
            var username = string.Empty;
            MockUserRepository
                .Setup(repo => repo.GetUserByUsernameAsync(username))
                .ThrowsAsync(new ArgumentException("Username cannot be null or empty.", nameof(username)));

            // Act
            Func<Task> act = async () => await UserService.GetUserByUsernameAsync(username);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Username cannot be null or empty.*");

            MockUserRepository.Verify(repo => repo.GetUserByUsernameAsync(username), Times.Once);
        }

        #endregion

        #region RegisterUserAsync Tests

        /// <summary>
        /// Tests that RegisterUserAsync successfully registers a user with valid registration data
        /// and returns the registered user with proper hashed password.
        /// </summary>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task RegisterUserAsync_WithValidData_ReturnsRegisteredUser()
        {
            // Arrange
            var registrationDto = new UserRegistrationDto
            {
                Username = "newuser",
                Email = "newuser@example.com",
                Password = "Password123!"
            };

            // Setup mock to return null to indicate username is available
            MockUserRepository
                .Setup(repo => repo.GetUserByUsernameAsync(registrationDto.Username))
                .ThrowsAsync(new KeyNotFoundException($"User with username '{registrationDto.Username}' not found."));

            User? createdUser = null;
            MockUserRepository
                .Setup(repo => repo.RegisterUserAsync(It.IsAny<User>()))
                .Callback<User>(u => createdUser = u)
                .ReturnsAsync((User u) => u);

            // Act
            var result = await UserService.RegisterUserAsync(registrationDto);

            // Assert
            result.Should().NotBeNull();
            result.Name.Value.Should().Be(registrationDto.Username);
            result.Email.Should().Be(registrationDto.Email);

            // Verify password was hashed and set correctly
            MockPasswordHasher.Verify(
                hasher => hasher.HashPassword(It.IsAny<User>(), registrationDto.Password),
                Times.Once);
            createdUser.Should().NotBeNull();
            createdUser!.PasswordHash.Should().Be("HashedPassword123");

            // Verify repository methods were called
            MockUserRepository.Verify(repo => repo.GetUserByUsernameAsync(registrationDto.Username), Times.Once);
            MockUserRepository.Verify(repo => repo.RegisterUserAsync(It.IsAny<User>()), Times.Once);
        }

        /// <summary>
        /// Tests that RegisterUserAsync throws ApplicationException when trying to register
        /// a user with a username that already exists.
        /// </summary>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task RegisterUserAsync_WithExistingUsername_ThrowsApplicationException()
        {
            // Arrange
            var registrationDto = new UserRegistrationDto
            {
                Username = "existinguser",
                Email = "existing@example.com",
                Password = "Password123!"
            };

            var existingUser = CreateTestUser(registrationDto.Username, "another@example.com", setupInRepository: true);

            // Act
            Func<Task> act = async () => await UserService.RegisterUserAsync(registrationDto);

            // Assert
            await act.Should().ThrowAsync<ApplicationException>()
                .WithMessage("Username already exists");

            // Verify repository methods were called
            MockUserRepository.Verify(repo => repo.GetUserByUsernameAsync(registrationDto.Username), Times.Once);
            MockUserRepository.Verify(repo => repo.RegisterUserAsync(It.IsAny<User>()), Times.Never);
        }

        /// <summary>
        /// Tests that RegisterUserAsync throws ArgumentException when provided with invalid input data.
        /// Multiple test cases are verified using Theory and InlineData for different combinations
        /// of invalid username, email, and password.
        /// </summary>
        /// <param name="username">The username to test (can be null, empty, or whitespace).</param>
        /// <param name="email">The email to test (can be null, empty, or whitespace).</param>
        /// <param name="password">The password to test (can be null, empty, or whitespace).</param>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Theory]
        [InlineData(null, "valid@example.com", "ValidPassword123")]
        [InlineData("", "valid@example.com", "ValidPassword123")]
        [InlineData(" ", "valid@example.com", "ValidPassword123")]
        [InlineData("validusername", null, "ValidPassword123")]
        [InlineData("validusername", "", "ValidPassword123")]
        [InlineData("validusername", " ", "ValidPassword123")]
        [InlineData("validusername", "valid@example.com", null)]
        [InlineData("validusername", "valid@example.com", "")]
        [InlineData("validusername", "valid@example.com", " ")]
        public async Task RegisterUserAsync_WithInvalidInput_ThrowsArgumentException(
            string username, string email, string password)
        {
            // Arrange
            var registrationDto = new UserRegistrationDto
            {
                Username = username,
                Email = email,
                Password = password
            };

            // Act
            Func<Task> act = async () => await UserService.RegisterUserAsync(registrationDto);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>();
        }

        #endregion

        #region UpdateUserAsync Tests

        /// <summary>
        /// Tests that UpdateUserAsync successfully updates a user with valid data and returns
        /// the updated user with the new username and email.
        /// </summary>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task UpdateUserAsync_WithValidUser_ReturnsUpdatedUser()
        {
            // Arrange
            var userId = Ulid.NewUlid();
            var user = CreateTestUser("oldname", "old@example.com", setupInRepository: true);
            var updatedUserData = new UserUpdateDto
            {
                Name = "newname",
                Email = "new@example.com"
            };

            // Setup the repository to return user when GetUserByIdAsync is called
            MockUserRepository
                .Setup(repo => repo.GetUserByIdAsync(user.Id))
                .ReturnsAsync(user);

            // Setup the repository to throw KeyNotFoundException when GetUserByUsernameAsync is called with the new username
            MockUserRepository
                .Setup(repo => repo.GetUserByUsernameAsync(updatedUserData.Name))
                .ThrowsAsync(new KeyNotFoundException($"User with username '{updatedUserData.Name}' not found."));

            // Setup the repository to return the user when UpdateUserAsync is called
            MockUserRepository
                .Setup(repo => repo.UpdateUserAsync(It.IsAny<User>()))
                .ReturnsAsync((User u) => u);

            // Act
            var result = await UserService.UpdateUserAsync(user.Id, updatedUserData);

            // Assert
            result.Should().NotBeNull();
            result.Name.Value.Should().Be(updatedUserData.Name);
            result.Email.Should().Be(updatedUserData.Email);

            // Verify repository methods were called
            MockUserRepository.Verify(repo => repo.GetUserByIdAsync(user.Id), Times.Once);
            MockUserRepository.Verify(repo => repo.GetUserByUsernameAsync(updatedUserData.Name), Times.Once);
            MockUserRepository.Verify(repo => repo.UpdateUserAsync(It.IsAny<User>()), Times.Once);
        }

        /// <summary>
        /// Tests that UpdateUserAsync throws KeyNotFoundException when trying to update
        /// a user that doesn't exist.
        /// </summary>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task UpdateUserAsync_WithNonExistentUser_ThrowsKeyNotFoundException()
        {
            // Arrange
            var userId = Ulid.NewUlid();
            var updatedUserData = new UserUpdateDto
            {
                Name = "newname",
                Email = "new@example.com"
            };

            // Setup repository to throw when GetUserByIdAsync is called
            MockUserRepository
                .Setup(repo => repo.GetUserByIdAsync(userId))
                .ThrowsAsync(new KeyNotFoundException($"User with ID {userId} not found."));

            // Act
            Func<Task> act = async () => await UserService.UpdateUserAsync(userId, updatedUserData);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage($"User with ID {userId} not found.");

            // Verify repository methods were called
            MockUserRepository.Verify(repo => repo.GetUserByIdAsync(userId), Times.Once);
            MockUserRepository.Verify(repo => repo.UpdateUserAsync(It.IsAny<User>()), Times.Never);
        }

        /// <summary>
        /// Tests that UpdateUserAsync throws ApplicationException when trying to update
        /// a user's username to one that is already in use by another user.
        /// </summary>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task UpdateUserAsync_WithExistingUsername_ThrowsApplicationException()
        {
            // Arrange
            var userId = Ulid.NewUlid();
            var user = CreateTestUser("currentuser", "current@example.com", setupInRepository: true);
            var existingUser = CreateTestUser("existinguser", "existing@example.com", setupInRepository: true);

            var updatedUserData = new UserUpdateDto
            {
                Name = existingUser.Name.Value,
                Email = "new@example.com"
            };

            // Setup repository to return the user when GetUserByIdAsync is called
            MockUserRepository
                .Setup(repo => repo.GetUserByIdAsync(user.Id))
                .ReturnsAsync(user);

            // Setup repository to return the existing user when GetUserByUsernameAsync is called with the new username
            MockUserRepository
                .Setup(repo => repo.GetUserByUsernameAsync(updatedUserData.Name))
                .ReturnsAsync(existingUser);

            // Act
            Func<Task> act = async () => await UserService.UpdateUserAsync(user.Id, updatedUserData);

            // Assert
            await act.Should().ThrowAsync<ApplicationException>()
                .WithMessage("Username already exists");

            // Verify repository methods were called
            MockUserRepository.Verify(repo => repo.GetUserByIdAsync(user.Id), Times.Once);
            MockUserRepository.Verify(repo => repo.GetUserByUsernameAsync(updatedUserData.Name), Times.Once);
            MockUserRepository.Verify(repo => repo.UpdateUserAsync(It.IsAny<User>()), Times.Never);
        }

        /// <summary>
        /// Tests that UpdateUserAsync throws ArgumentException when provided with an empty user ID.
        /// </summary>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task UpdateUserAsync_WithEmptyId_ThrowsArgumentException()
        {
            // Arrange
            var emptyId = Ulid.Empty;
            var updatedUserData = new UserUpdateDto
            {
                Name = "newname",
                Email = "new@example.com"
            };

            // Act
            Func<Task> act = async () => await UserService.UpdateUserAsync(emptyId, updatedUserData);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("User ID cannot be empty.*");
        }

        #endregion

        #region DeleteUserAsync Tests

        /// <summary>
        /// Tests that DeleteUserAsync successfully deletes a user with a valid ID
        /// and returns true to indicate successful deletion.
        /// </summary>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task DeleteUserAsync_WithValidId_ReturnsTrue()
        {
            // Arrange
            var user = CreateTestUser("userToDelete", "delete@example.com", setupInRepository: true);

            // Setup repository to return true when DeleteUserAsync is called
            MockUserRepository
                .Setup(repo => repo.DeleteUserAsync(user.Id))
                .ReturnsAsync(true);

            // Act
            var result = await UserService.DeleteUserAsync(user.Id);

            // Assert
            result.Should().BeTrue();

            // Verify repository methods were called
            MockUserRepository.Verify(repo => repo.GetUserByIdAsync(user.Id), Times.Once);
            MockUserRepository.Verify(repo => repo.DeleteUserAsync(user.Id), Times.Once);
        }

        /// <summary>
        /// Tests that DeleteUserAsync throws KeyNotFoundException when trying to delete
        /// a user with a non-existent ID.
        /// </summary>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task DeleteUserAsync_WithNonExistentId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var nonExistentId = Ulid.NewUlid();

            // Setup repository to throw when GetUserByIdAsync is called
            MockUserRepository
                .Setup(repo => repo.GetUserByIdAsync(nonExistentId))
                .ThrowsAsync(new KeyNotFoundException($"User with ID {nonExistentId} not found."));

            // Act
            Func<Task> act = async () => await UserService.DeleteUserAsync(nonExistentId);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage($"User with ID {nonExistentId} not found.");

            // Verify repository methods were called
            MockUserRepository.Verify(repo => repo.GetUserByIdAsync(nonExistentId), Times.Once);
            MockUserRepository.Verify(repo => repo.DeleteUserAsync(nonExistentId), Times.Never);
        }

        /// <summary>
        /// Tests that DeleteUserAsync throws ArgumentException when provided with an empty user ID.
        /// </summary>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task DeleteUserAsync_WithEmptyId_ThrowsArgumentException()
        {
            // Arrange
            var emptyId = Ulid.Empty;

            // Act
            Func<Task> act = async () => await UserService.DeleteUserAsync(emptyId);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("User ID cannot be empty.*");

            // Verify repository methods were called
            MockUserRepository.Verify(repo => repo.DeleteUserAsync(emptyId), Times.Never);
        }

        #endregion

        #region AuthenticateUserAsync Tests

        /// <summary>
        /// Tests that AuthenticateUserAsync successfully authenticates a user with valid credentials
        /// and returns the authenticated user.
        /// </summary>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task AuthenticateUserAsync_WithValidCredentials_ReturnsUser()
        {
            // Arrange
            var loginDto = new UserLoginDto
            {
                Username = "testuser",
                Password = "Password123!"
            };

            var user = CreateTestUser(loginDto.Username, "test@example.com", setupInRepository: true);
            user.UpdatePassword("HashedPassword123");

            // Setup password hasher to return Success when VerifyHashedPassword is called
            MockPasswordHasher
                .Setup(hasher => hasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    loginDto.Password))
                .Returns(PasswordVerificationResult.Success);

            // Act
            var result = await UserService.AuthenticateUserAsync(loginDto);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeSameAs(user);

            // Verify methods were called
            MockUserRepository.Verify(repo => repo.GetUserByUsernameAsync(loginDto.Username), Times.Once);
            MockPasswordHasher.Verify(hasher =>
                hasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    loginDto.Password),
                Times.Once);
        }

        /// <summary>
        /// Tests that AuthenticateUserAsync throws InvalidOperationException when a user
        /// attempts to authenticate with an invalid password.
        /// </summary>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task AuthenticateUserAsync_WithInvalidPassword_ThrowsInvalidOperationException()
        {
            // Arrange
            var loginDto = new UserLoginDto
            {
                Username = "testuser",
                Password = "InvalidPassword123"
            };

            var user = CreateTestUser(loginDto.Username, "test@example.com", setupInRepository: true);
            user.UpdatePassword("HashedPassword123");

            // Setup password hasher to return Failed when VerifyHashedPassword is called
            MockPasswordHasher
                .Setup(hasher => hasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    loginDto.Password))
                .Returns(PasswordVerificationResult.Failed);

            // Act
            Func<Task> act = async () => await UserService.AuthenticateUserAsync(loginDto);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Invalid username or password.");

            // Verify methods were called
            MockUserRepository.Verify(repo => repo.GetUserByUsernameAsync(loginDto.Username), Times.Once);
            MockPasswordHasher.Verify(hasher =>
                hasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    loginDto.Password),
                Times.Once);
        }

        /// <summary>
        /// Tests that AuthenticateUserAsync throws KeyNotFoundException when attempting
        /// to authenticate with a username that doesn't exist.
        /// </summary>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task AuthenticateUserAsync_WithNonExistentUsername_ThrowsKeyNotFoundException()
        {
            // Arrange
            var loginDto = new UserLoginDto
            {
                Username = "nonexistentuser",
                Password = "Password123!"
            };

            // Setup repository to throw when GetUserByUsernameAsync is called
            MockUserRepository
                .Setup(repo => repo.GetUserByUsernameAsync(loginDto.Username))
                .ThrowsAsync(new KeyNotFoundException($"User with username '{loginDto.Username}' not found."));

            // Act
            Func<Task> act = async () => await UserService.AuthenticateUserAsync(loginDto);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage($"User with username '{loginDto.Username}' not found.");

            // Verify methods were called
            MockUserRepository.Verify(repo => repo.GetUserByUsernameAsync(loginDto.Username), Times.Once);
            MockPasswordHasher.Verify(hasher =>
                hasher.VerifyHashedPassword(
                    It.IsAny<User>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
        }

        /// <summary>
        /// Tests that AuthenticateUserAsync throws ArgumentException when provided with invalid input.
        /// Multiple test cases are verified using Theory and InlineData for different combinations
        /// of invalid username and password.
        /// </summary>
        /// <param name="username">The username to test (can be null, empty, or whitespace).</param>
        /// <param name="password">The password to test (can be null, empty, or whitespace).</param>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Theory]
        [InlineData(null, "Password123!")]
        [InlineData("", "Password123!")]
        [InlineData(" ", "Password123!")]
        [InlineData("username", null)]
        [InlineData("username", "")]
        [InlineData("username", " ")]
        public async Task AuthenticateUserAsync_WithInvalidInput_ThrowsArgumentException(
            string username, string password)
        {
            // Arrange
            var loginDto = new UserLoginDto
            {
                Username = username,
                Password = password
            };

            // Act
            Func<Task> act = async () => await UserService.AuthenticateUserAsync(loginDto);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>();
        }

        #endregion
    }
}
