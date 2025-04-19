using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Plantica.Core.Models;

namespace Plantica.Infrastructure.Data
{
    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // added to ensure the primary key is set correctly
            builder.HasKey(u => u.Id);

            // added to ensure the Ulid is converted correctly
            builder.Property(u => u.Id)
                .HasConversion(
                    id => id.ToString(),
                    str => Ulid.Parse(str));

            // configures UserName as an owned entity
            builder.OwnsOne(u => u.Name, nameBuilder =>
            {
                // set index to Value property by uilder method
                nameBuilder.Property(n => n.Value)
                    .HasMaxLength(50)
                    .IsRequired();

                // Indexes are set for the properties of the owning entity
                nameBuilder.HasIndex("Value").IsUnique();
            });
        }
    }
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            new UserEntityTypeConfiguration().Configure(modelBuilder.Entity<User>());
        }
    }
}
