using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using UniSportUAQ_API.Data;
using UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Data
{
    public class DatabaseInitializer
    {

        public async static void FeedDatabase(IApplicationBuilder applicationBuilder) {

            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AppDbContext>();

				var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

				if (context!.Courses.Any()) return;
				if (context!.CourseClasses.Any()) return;

				var instructorUser = await userManager.FindByEmailAsync("rodrif19@hotmail.com");



				var instructorFilled = context.Instructors.Where(
					i => i.Email == instructorUser!.Email
				)
				.ToList();

				var courses = new List<Course>
				{
					new Course
					{
						CourseName = "Curso de C#",
						InstructorId = instructorFilled[0].Id,
					},
				};

				context.Courses.AddRange(courses);

				context.SaveChanges();
			}
        }

		public async static void FeedUsersAndRoles(IApplicationBuilder applicationBuilder)
		{
			using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
			{
				var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

				var instructorUser = await userManager.FindByEmailAsync("rodrif19@hotmail.com");
				var studentUser = await userManager.FindByEmailAsync("student@hotmail.com");

				if (instructorUser != null) return;
				if (studentUser != null) return;

				var instructor = new Instructor
				{
					Email = "rodrif19@hotmail.com",
					UserName = "307041",
					Name = "Marco Rodrigo",
					LastName = "Flores Tepatzi",
					Expediente = "307041"
				};

				var student = new Student
				{
					Email = "student@hotmail.com",
					UserName = "307000",
					Name = "Jasiel",
					LastName = "Salmeron",
					Expediente = "307000"
				};

				await userManager.CreateAsync(instructor, "Passw0rd$69");
				await userManager.CreateAsync(student, "JasielGei14$");
			}
		}
    }
}
