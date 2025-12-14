using Microsoft.EntityFrameworkCore;
using TextAdventureApi.Models;
using TextAdventureApi.Security;

namespace TextAdventureApi.Data
{
    public class TextAdventureDbContext : DbContext
    {
        public TextAdventureDbContext(DbContextOptions<TextAdventureDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = default!;
        public DbSet<KeyShare> KeyShares { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed ONE keyshare for simple-mode
            modelBuilder.Entity<KeyShare>().HasData(
                new KeyShare
                {
                    Id = Guid.NewGuid(),
                    RoomId = "main",
                    Share = "ABC-EFG-HIJK", /// WOW
                    MinRole = "Player"
                }
            );

            // Seed a single admin user (username: admin, password: admin)
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "admin",
                    PasswordHash = Sha256Hasher.Hash("admin"),
                    Role = Role.Admin,
                    FailedLoginAttempts = 0,
                    IsLockedOut = false
                }
            );
        }
    }
}
