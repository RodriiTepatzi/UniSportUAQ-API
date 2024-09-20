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
			modelBuilder.Entity<Horario>()
				.HasOne(c => c.Course)
				.WithMany(c => c.Horarios)
				.HasForeignKey(c => c.CourseId)
				.OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<Course>()
				.HasOne(c => c.Subject)
				.WithMany(s => s.Courses)
				.HasForeignKey(c => c.SubjectId)
				.OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<CartaLiberacion>()
				.HasOne(c => c.Course)
				.WithMany(s => s.CartaLiberacions)
				.HasForeignKey(c => c.CourseId)
				.OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<Attendance>()
				.HasOne(c => c.Course)
				.WithMany(s => s.Attendances)
				.HasForeignKey(c => c.CourseId)
				.OnDelete(DeleteBehavior.NoAction);


			modelBuilder.Entity<Inscription>()
				.HasOne(c => c.Course)
				.WithMany(s => s.Inscriptions)
				.HasForeignKey(c => c.CourseId)
				.OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<UserPreferences>()
				.HasOne(Us => Us.User)
				.WithOne(U => U.UserPreferences)
                .HasForeignKey<UserPreferences>(up => up.UserId)
				.OnDelete(DeleteBehavior.NoAction);





            base.OnModelCreating(modelBuilder);





		}

		public DbSet<ApplicationUser> ApplicationUsers { get; set; }

		public DbSet<Course> Courses { get; set; }

		public DbSet<Inscription> Inscriptions { get; set; }

		public DbSet<Subject> CourseClasses { get; set; }

		public DbSet<Attendance> Attendances { get; set; }

		public DbSet<CartaLiberacion> CartasLiberacion { get; set; }

		public DbSet<Subject> Subjects { get; set; }

		public DbSet<TimePeriod> TimePeriods { get; set; }

		public DbSet<UserPreferences> UserPreferences { get; set; }
		public DbSet<Horario> Horarios { get; set; }
    }
}