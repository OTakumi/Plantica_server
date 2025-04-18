using FluentAssertions;

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

        [Fact]
        public async Task GetUserByIdAsync_WithValidId_ReturnUser()
        {
            // Arrange
            var user = await AddUserAsync("testuser", "test@example.com");
            _output.WriteLine($"User ID: {user.Id}");
            _output.WriteLine($"User Name: {user.Name}");
            _output.WriteLine($"User Email: {user.Email}");

            // Act
            var result = await _repository.GetUserByIdAsync(user.Id);

            // Assert
            result.Should().NotBeNull();
            result.Name.Value.Should().Be("testuser");
            result.Email.Should().Be("test@example.com");
        }

        //[Fact]
        //public async Task RegisterUserAsync_WithValidUser_ShouldSaveUserAndReturnUser()
        //{
        //    // Arrange
        //    var dbName = $"UserDb_{Guid.NewGuid()}";
        //    using var context = CreateContext(dbName);
        //    var repository = new UserRepository(context);

        //    var user = new User("testuser", "test@example.com");
        //    user.UpdatePassword("hashedpassword");

        //    // Act
        //    var result = await repository.RegisterUserAsync(user);

        //    // Assert
        //    result.Should().NotBeNull();
        //    result.Name.Value.Should().Be("testuser");
        //    result.Email.Should().Be("test@example.com");

        //    // 追加確認: データベースに保存されているか
        //    using var verifyContext = CreateContext(dbName);
        //    var savedUser = await verifyContext.Users.FindAsync(user.Id);
        //    savedUser.Should().NotBeNull();
        //    savedUser!.Name.Value.Should().Be("testuser");
        //}
    }
}