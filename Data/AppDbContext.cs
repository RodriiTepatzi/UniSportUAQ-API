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

			modelBuilder.Entity<Attendance>().ToTable("Attendances")
				.HasOne(a => a.Student)
				.WithMany(s => s.Attendances)
				.HasForeignKey(a => a.StudentId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Inscription>()
				.HasOne(i => i.Student)
				.WithMany(s => s.Inscriptions)
				.HasForeignKey(i => i.StudentId)
				.OnDelete(DeleteBehavior.Restrict);


    

            base.OnModelCreating(modelBuilder);

            
        }

		public DbSet<ApplicationUser> ApplicationUsers { get; set; }
		
		public DbSet<Student> Students { get; set; }

        public DbSet<Instructor> Instructors { get; set; }

		public DbSet<Admin> Admins { get; set; }

		public DbSet<Course> Courses { get; set; }

		public DbSet<Inscription> Inscriptions { get; set; }

		public DbSet<CourseClass> CourseClasses { get; set; }

		public DbSet<Attendance> Attendances { get; set; }
    }
}