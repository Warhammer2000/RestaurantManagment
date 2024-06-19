using Microsoft.EntityFrameworkCore;
using MenuService.Models;

namespace MenuService.DB
{
    public class MenuContext : DbContext
    {
        public MenuContext(DbContextOptions<MenuContext> options) : base(options) { }

        public DbSet<MenuItem> MenuItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MenuItem>()
                .Property(m => m.Price)
                .HasPrecision(18, 4); 
        }
    }
}
