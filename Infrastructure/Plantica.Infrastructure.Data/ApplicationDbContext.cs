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
            // Define entity structure (relationships, constraints, etc.)
            //modelBuilder.Entity<User>()
            //    .HasIndex(u => u.Name)
            //    .IsUnique();

            //modelBuilder.Entity<User>().OwnsOne(u => u.Name);

            base.OnModelCreating(modelBuilder);

            // UserNameを所有エンティティとして構成
            modelBuilder.Entity<User>().OwnsOne(u => u.Name, nameBuilder =>
            {
                // ValueプロパティにインデックスをBuilder方式で設定
                nameBuilder.Property(n => n.Value)
                    .HasMaxLength(50)
                    .IsRequired();

                // インデックスは所有エンティティのプロパティに対して設定
                nameBuilder.HasIndex("Value").IsUnique();
            });

            // 主キーの設定を追加
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            // Ulidのコンバーターを追加
            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .HasConversion(
                    id => id.ToString(),
                    str => Ulid.Parse(str));
        }
    }
}
