using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using Plantica.Core.Models;
using Plantica.Infrastructure.Repositories;
using Plantica.Tests.TestBase;

using Xunit.Abstractions;

namespace Plantica.Tests.Infrastructure.Repositories.UserTest
{
    public class RegisterUserTests : RepositoryTestBase
    {
        private readonly UserRepository _repository;
        private readonly ITestOutputHelper _output;

        public RegisterUserTests(ITestOutputHelper output) : base()
        {
            _repository = new UserRepository(DbContext);
            _output = output;
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
            User? nullUser = null;

            // Act & Assert
            Func<Task> act = async () => await _repository.RegisterUserAsync(nullUser);

            await act.Should().ThrowAsync<ArgumentNullException>()
                .WithMessage("User cannot be null.*")
                .WithParameterName("user");
        }

        /// <summary>
        /// Tests the RegisterUserAsync method with a duplicate user.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task RegisterUserAsync_DuplicateUser_ThrowsException()
        {
            // Arrange
            var firstUser = new User("TestUser", "test1@example.com");
            await _repository.RegisterUserAsync(firstUser);

            // DBに保存されたことを確認
            var savedUser = await DbContext.Users.FindAsync(firstUser.Id);
            savedUser.Should().NotBeNull();

            // 同じユーザー名で2人目のユーザーを作成
            var duplicateUser = new User("TestUser", "test2@example.com");

            // Act & Assert
            Func<Task> act = async () => await _repository.RegisterUserAsync(duplicateUser);

            // 何らかの例外がスローされることを検証
            // 注: 具体的な例外の種類は実装によって異なるため、
            // ここでは一般的なExceptionをキャッチしています
            await act.Should().ThrowAsync<Exception>();

            // DBに保存されたユーザーが1人だけであることを確認
            var userCount = await DbContext.Users.CountAsync();
            userCount.Should().Be(1);
        }
    }
}
