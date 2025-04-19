using Plantica.Infrastructure.Repositories;
using Plantica.Tests.TestBase;

using Xunit.Abstractions;

namespace Plantica.Tests.Infrastructure.Repositories.UserTest
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
    }
}
