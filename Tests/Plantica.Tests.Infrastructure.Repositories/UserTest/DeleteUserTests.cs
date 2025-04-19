using Plantica.Infrastructure.Repositories;
using Plantica.Tests.TestBase;

using Xunit.Abstractions;

namespace Plantica.Tests.Infrastructure.Repositories.UserTest
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
    }
}
