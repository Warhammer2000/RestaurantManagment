using Microsoft.EntityFrameworkCore;
using RestaurantOrderManagement.Models;

namespace RestaurantOrderManagement.DB
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		public DbSet<MenuItem> MenuItems { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<User> Users { get; set; }
	}
}
