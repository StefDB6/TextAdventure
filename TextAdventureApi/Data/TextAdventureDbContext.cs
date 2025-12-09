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
    }
}
