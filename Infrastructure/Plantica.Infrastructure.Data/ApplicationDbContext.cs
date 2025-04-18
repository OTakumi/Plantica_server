using Microsoft.EntityFrameworkCore;

using Plantica.Core.Models;

namespace Plantica.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext // Fixed constructor declaration
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // added to ensure the primary key is set correctly
            modelBuilder.Entity<User>().HasKey(u => u.Id);

            // added to ensure the Ulid is converted correctly
            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .HasConversion(
                    id => id.ToString(),
                    str => Ulid.Parse(str));

            // configures UserName as an owned entity
            modelBuilder.Entity<User>().OwnsOne(u => u.Name, nameBuilder =>
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
}
