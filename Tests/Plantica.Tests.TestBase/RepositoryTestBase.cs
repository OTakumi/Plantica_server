using Microsoft.EntityFrameworkCore;

using Plantica.Core.Models;
using Plantica.Infrastructure.Data;

namespace Plantica.Tests.TestBase
{
    /// <summary>
    /// Base class for repository tests providing common functionality.
    /// </summary>
    public abstract class RepositoryTestBase : IDisposable
    {
        protected readonly ApplicationDbContext DbContext;
        protected readonly string DatabaseName;

        /// <summary>
        /// Initializes a new instance of the RepositoryTestBase class.
        /// </summary>
        protected RepositoryTestBase()
        {
            DatabaseName = $"TestDb_{Guid.NewGuid()}";
            DbContext = CreateContext();
        }

        /// <summary>
        /// Creates a new in-memory DbContext for testing.
        /// </summary>
        /// <returns>A new ApplicationDbContext instance.</returns>
        protected ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(DatabaseName)
                .Options;

            var context = new ApplicationDbContext(options);
            context.Database.EnsureCreated();

            return context;
        }

        /// <summary>
        /// Adds a user to the database for testing.
        /// </summary>
        /// <param name="name">The username.</param>
        /// <param name="email">The user's email.</param>
        /// <returns>The created User entity.</returns>
        protected async Task<User> AddUserAsync(string name, string email)
        {
            var user = new User(name, email);
            await DbContext.Users.AddAsync(user);
            await DbContext.SaveChangesAsync();

            return user;
        }

        /// <summary>
        /// Disposes the database context.
        /// </summary>
        public void Dispose()
        {
            DbContext.Database.EnsureDeleted();
            DbContext.Dispose();
        }
    }
}