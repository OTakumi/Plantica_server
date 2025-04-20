using Microsoft.AspNetCore.Identity;

using Moq;

using Plantica.Application.Services;
using Plantica.Core.Models;
using Plantica.Infrastructure.Repositories.Interfaces;

namespace Plantica.Tests.Application.Services
{
    /// <summary>
    /// Base class for service tests providing common functionality.
    /// This class sets up common mock objects and provides helper methods
    /// to reduce code duplication in test classes.
    /// </summary>
    public abstract class ServiceTestBase : IDisposable
    {
        // Common mocks that will be used by derived test classes
        /// <summary>
        /// Mock of the user repository to be used by test classes.
        /// </summary>
        protected readonly Mock<IUserRepository> MockUserRepository;

        /// <summary>
        /// Mock of the password hasher to be used by test classes.
        /// </summary>
        protected readonly Mock<IPasswordHasher<User>> MockPasswordHasher;

        // Services to be tested
        /// <summary>
        /// Instance of UserService initialized with mocked dependencies.
        /// </summary>
        protected readonly UserService UserService;

        /// <summary>
        /// Initializes a new instance of the ServiceTestBase class.
        /// Sets up common mock objects and initializes services with these mocks.
        /// </summary>
        protected ServiceTestBase()
        {
            // Initialize mocks
            MockUserRepository = new Mock<IUserRepository>();
            MockPasswordHasher = new Mock<IPasswordHasher<User>>();

            // Initialize services with mocks
            UserService = new UserService(MockUserRepository.Object, MockPasswordHasher.Object);

            // Set up common mock behaviors
            SetupCommonMockBehaviors();
        }

        /// <summary>
        /// Sets up common mock behaviors that will be used by most tests.
        /// This includes default implementations for common operations like password hashing.
        /// </summary>
        private void SetupCommonMockBehaviors()
        {
            // Setup password hasher to return a predictable hashed value
            MockPasswordHasher
                .Setup(h => h.HashPassword(It.IsAny<User>(), It.IsAny<string>()))
                .Returns("HashedPassword123");
        }

        /// <summary>
        /// Creates a User object for testing purposes.
        /// </summary>
        /// <param name="name">The username for the test user.</param>
        /// <param name="email">The email for the test user.</param>
        /// <param name="setupInRepository">
        /// Whether to setup the mock repository to return this user when queried.
        /// If true, the repository will be configured to return this user for both GetUserByIdAsync and GetUserByUsernameAsync.
        /// </param>
        /// <returns>The created User entity.</returns>
        protected User CreateTestUser(string name, string email, bool setupInRepository = false)
        {
            var user = new User(name, email);

            if (setupInRepository)
            {
                MockUserRepository
                    .Setup(repo => repo.GetUserByIdAsync(user.Id))
                    .ReturnsAsync(user);

                MockUserRepository
                    .Setup(repo => repo.GetUserByUsernameAsync(name))
                    .ReturnsAsync(user);
            }

            return user;
        }

        /// <summary>
        /// Sets up the mock repository to simulate a user not found scenario.
        /// This configures both GetUserByIdAsync and GetUserByUsernameAsync to throw KeyNotFoundException.
        /// </summary>
        /// <param name="userId">The user ID that should trigger a not found exception.</param>
        /// <param name="username">The username that should trigger a not found exception.</param>
        protected void SetupUserNotFound(Ulid userId, string username)
        {
            MockUserRepository
                .Setup(repo => repo.GetUserByIdAsync(userId))
                .ThrowsAsync(new KeyNotFoundException($"User with ID {userId} not found."));

            MockUserRepository
                .Setup(repo => repo.GetUserByUsernameAsync(username))
                .ThrowsAsync(new KeyNotFoundException($"User with username '{username}' not found."));
        }

        /// <summary>
        /// Verifies that all expected mock calls were made.
        /// This method can be called at the end of a test to ensure that
        /// all the expected repository and service methods were called.
        /// </summary>
        protected void VerifyAllMocks()
        {
            MockUserRepository.VerifyAll();
            MockPasswordHasher.VerifyAll();
        }

        /// <summary>
        /// Cleans up resources used by the test.
        /// This method is called automatically after each test is run.
        /// </summary>
        public virtual void Dispose()
        {
            // Nothing to dispose in this base class, but derived classes might need to override this
        }
    }
}
