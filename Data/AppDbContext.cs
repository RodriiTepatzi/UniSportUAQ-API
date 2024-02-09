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
            //inheritance from users
            modelBuilder.Entity<Student>().ToTable("Students");

			modelBuilder.Entity<Student>()
				.Property(e => e.Id);

			//
			modelBuilder.Entity<Instructor>().ToTable("Instructors");

			modelBuilder.Entity<Instructor>()
				.Property(e => e.Id);

			//
			modelBuilder.Entity<Admin>().ToTable("Admins");

			modelBuilder.Entity<Admin>()
				.Property(e => e.Id);

			base.OnModelCreating(modelBuilder);
		}

		public DbSet<ApplicationUser> ApplicationUsers { get; set; }
		
		public DbSet<Student> Students { get; set; }

        public DbSet<Instructor> Instructors { get; set; }

		public DbSet<Admin> Admins { get; set; }

		public DbSet<Course> Courses { get; set; }

		public DbSet<Inscription> Inscriptions { get; set; }

		public DbSet<CourseClass> CourseClasses { get; set; }



    }
}