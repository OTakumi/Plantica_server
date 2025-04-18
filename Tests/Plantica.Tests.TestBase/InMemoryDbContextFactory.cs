using Microsoft.EntityFrameworkCore;

using Plantica.Infrastructure.Data;

namespace Plantica.Tests.TestBase
{
    /// <summary>
    /// Provides factory methods for creating in-memory database contexts for testing.
    /// </summary>
    public static class InMemoryDbContextFactory
    {
        /// <summary>
        /// Creates a new ApplicationDbContext with an in-memory database for testing.
        /// </summary>
        /// <param name="databaseName">Optional name for the in-memory database.
        /// If not provided, a unique name will be generated.</param>
        /// <returns>A new instance of ApplicationDbContext configured to use an in-memory database.</returns>
        public static ApplicationDbContext Create(string? databaseName = null)
        {
            databaseName ??= $"InMemoryDb_{Guid.NewGuid()}";

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;

            var context = new ApplicationDbContext(options);

            context.Model.GetEntityTypes();

            return new ApplicationDbContext(options);
        }
    }
}
