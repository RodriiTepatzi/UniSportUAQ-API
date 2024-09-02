using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Data
{
    public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{

		}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
            // Habilitamos el registro de datos sensibles
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);   
        }

		public DbSet<ApplicationUser> ApplicationUsers { get; set; }

		public DbSet<Course> Courses { get; set; }

		public DbSet<Inscription> Inscriptions { get; set; }

		public DbSet<CourseClass> CourseClasses { get; set; }

		public DbSet<Attendance> Attendances { get; set; }

		public DbSet<CartaLiberacion> CartasLiberacion { get; set; }

		public DbSet<TimePeriod> TimePeriods { get; set; }
    }
}