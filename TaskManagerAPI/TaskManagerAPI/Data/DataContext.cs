using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Domain.Models;

namespace TaskManagerApi.Data
{
	public class DataContext : DbContext
	{
		public DataContext(DbContextOptions<DataContext> options) : base(options) {}

		public DbSet<Project> Projects { get; set; }
		public DbSet<User> Users { get; set; }
	}
}
