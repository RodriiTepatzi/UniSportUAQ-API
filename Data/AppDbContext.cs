using Microsoft.EntityFrameworkCore;
using UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{

		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Student>().ToTable("Students");

			modelBuilder.Entity<Student>()
				.Property(e => e.Id);

			base.OnModelCreating(modelBuilder);
		}

		public DbSet<ApplicationUser> ApplicationUsers { get; set; }
		public DbSet<Student> Students { get; set; }
	}
}