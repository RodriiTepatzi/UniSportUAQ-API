using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System;
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

				if (instructorUser != null)
				{
					var instructorFilled = context.ApplicationUsers.Single(
						i => i.Email == instructorUser!.Email && i.IsInstructor == true
					);

					var courses = new List<Course>
					{
						new Course
						{
							CourseName = "Curso de C#",
							InstructorId = instructorFilled.Id,
							MaxUsers = 30,
							CurrentUsers = 0,
							Day = "Lunes",
							Hour = "4",
							IsActive = true
						},
						new Course
						{
                            CourseName = "Curso de Python",
                            InstructorId = instructorFilled.Id,
                            MaxUsers = 30,
                            CurrentUsers = 0,
                            Day = "Martes",
                            Hour = "4",
                            IsActive = true
                        },
						
					};

					context.Courses.AddRange(courses);
				}

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

				var instructor = new ApplicationUser
				{
					Email = "rodrif19@hotmail.com",
					UserName = "307041",
					Name = "Marco Rodrigo",
					LastName = "Flores Tepatzi",
					Expediente = "307041",
					IsStudent = true
				};
                await userManager.CreateAsync(instructor, "Passw0rd$69");

                var student = new ApplicationUser
				{
					Email = "student@hotmail.com",
					UserName = "307000",
					Name = "Jasiel",
					LastName = "Salmeron",
					Expediente = "307000",
					IsStudent = true
				};
                await userManager.CreateAsync(student, "JasielGei14$");

                var student1 = new ApplicationUser
				{
                    Email = "student1@hotmail.com",
                    UserName = "3070001",
                    Name = "Jasiel",
                    LastName = "Salmeron",
                    Expediente = "3070001",
					IsStudent = true
				};

                await userManager.CreateAsync(student1, "JasielGei14$");

                var student2 = new ApplicationUser
				{
                    Email = "student2@hotmail.com",
                    UserName = "3070002",
                    Name = "Juan",
                    LastName = "Pérez",
                    Expediente = "3070002",
					IsStudent = true
				};

                await userManager.CreateAsync(student2, "JuanGei14$");

                var student3 = new ApplicationUser
				{
                    Email = "student3@hotmail.com",
                    UserName = "3070003",
                    Name = "María",
                    LastName = "González",
                    Expediente = "3070003",
					IsStudent = true
				};

                await userManager.CreateAsync(student3, "MariaGei14$");

                var student4 = new ApplicationUser
				{
                    Email = "student4@hotmail.com",
                    UserName = "3070004",
                    Name = "Pedro",
                    LastName = "Martínez",
                    Expediente = "3070004",
					IsStudent = true
				};

                await userManager.CreateAsync(student4, "PedroGei14$");

                var student5 = new ApplicationUser
				{
                    Email = "student5@hotmail.com",
                    UserName = "3070005",
                    Name = "Ana",
                    LastName = "López",
                    Expediente = "3070005",
					IsStudent = true
				};

                await userManager.CreateAsync(student5, "AnaGei14$");

                





            }


        }
        public async static void FeedAttendances(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                // Comprueba si ya existen asistencias en la base de datos
                if (context!.Attendances.Any()) return;

                // Busca a los usuarios que van a asistir a los cursos
                var studentUser = await userManager.FindByEmailAsync("student2@hotmail.com");
                var studentUser1 = await userManager.FindByEmailAsync("student4@hotmail.com");
                // Añade más estudiantes si es necesario

                // Busca los cursos a los que van a asistir los estudiantes
                var courseCSharp = context.Courses.FirstOrDefault(c => c.CourseName == "Curso de C#");
                var coursePython = context.Courses.FirstOrDefault(c => c.CourseName == "Curso de C#");
                // Añade más cursos si es necesario

                if (studentUser != null && courseCSharp != null)
                {
                    var attendance = new Attendance
                    {
                        CourseId = courseCSharp.Id,
                        StudentId = studentUser.Id,
                        Date = DateTime.Now.Date
                        // Añade más campos si es necesario
                    };

                    context.Attendances.Add(attendance);
                }

                if (studentUser1 != null && coursePython != null)
                {
                    var attendance = new Attendance
                    {
                        CourseId = coursePython.Id,
                        StudentId = studentUser1.Id,
                        Date = DateTime.Now.Date
                        // Añade más campos si es necesario
                    };

                    context.Attendances.Add(attendance);
                }

                if (studentUser1 != null && coursePython != null)
                {
                    var attendance = new Attendance
                    {
                        CourseId = coursePython.Id,
                        StudentId = studentUser1.Id,
                        Date = DateTime.Now.Date.AddDays(-1),
                        // Añade más campos si es necesario
                    };

                    context.Attendances.Add(attendance);
                }
                if (studentUser1 != null && coursePython != null)
                {
                    var attendance = new Attendance
                    {
                        CourseId = coursePython.Id,
                        StudentId = studentUser1.Id,
                        Date = DateTime.Now.Date.AddDays(-2),
                        // Añade más campos si es necesario
                    };

                    context.Attendances.Add(attendance);
                }
                if (studentUser1 != null && coursePython != null)
                {
                    var attendance = new Attendance
                    {
                        CourseId = coursePython.Id,
                        StudentId = studentUser1.Id,
                        Date = DateTime.Now.Date.AddDays(-3),
                        // Añade más campos si es necesario
                    };

                    context.Attendances.Add(attendance);
                }

                if (studentUser != null && coursePython != null)
                {
                    var attendance = new Attendance
                    {
                        CourseId = coursePython.Id,
                        StudentId = studentUser.Id,
                        Date = DateTime.Now.Date.AddDays(-1),
                        // Añade más campos si es necesario
                    };

                    context.Attendances.Add(attendance);
                }
                if (studentUser != null && coursePython != null)
                {
                    var attendance = new Attendance
                    {
                        CourseId = coursePython.Id,
                        StudentId = studentUser.Id,
                        Date = DateTime.Now.Date.AddDays(-2),
                        // Añade más campos si es necesario
                    };

                    context.Attendances.Add(attendance);
                }
                if (studentUser != null && coursePython != null)
                {
                    var attendance = new Attendance
                    {
                        CourseId = coursePython.Id,
                        StudentId = studentUser.Id,
                        Date = DateTime.Now.Date.AddDays(-3),
                        // Añade más campos si es necesario
                    };

                    context.Attendances.Add(attendance);
                }

                // Añade más asistencias si es necesario

                context.SaveChanges();
            }
        }

    }
}
