using Microsoft.EntityFrameworkCore;
using TextAdventureApi.Models;

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
        }
    }
}
