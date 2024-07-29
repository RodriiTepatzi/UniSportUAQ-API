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
                    IsStudent = true,
                    IsInstructor = true,
                    IsActive = true
                };
                await userManager.CreateAsync(instructor, "Passw0rd$69");

                var student = new ApplicationUser
                {
                    Email = "student@hotmail.com",
                    UserName = "307000",
                    Name = "Jasiel",
                    LastName = "Salmeron",
                    Expediente = "307000",
                    IsStudent = true,
                    IsActive = true
                };
                await userManager.CreateAsync(student, "JasielGei14$");





                var student2 = new ApplicationUser
                {
                    Email = "student2@hotmail.com",
                    UserName = "3070002",
                    Name = "Juan",
                    LastName = "Pérez",
                    Expediente = "3070002",
                    IsStudent = true,
                    IsActive = true
                };

                await userManager.CreateAsync(student2, "JuanGei14$");



                var student3 = new ApplicationUser
                {
                    Email = "student3@hotmail.com",
                    UserName = "3070003",
                    Name = "María",
                    LastName = "González",
                    Expediente = "3070003",
                    IsStudent = true,
                    IsActive = true
                };

                await userManager.CreateAsync(student3, "MariaGei14$");



                var student4 = new ApplicationUser
                {
                    Email = "student4@hotmail.com",
                    UserName = "3070004",
                    Name = "Pedro",
                    LastName = "Martínez",
                    Expediente = "3070004",
                    IsStudent = true,
                    IsActive = true
                };

                await userManager.CreateAsync(student4, "PedroGei14$");


                var student5 = new ApplicationUser
                {
                    Email = "student5@hotmail.com",
                    UserName = "3070005",
                    Name = "Ana",
                    LastName = "López",
                    Expediente = "3070005",
                    IsStudent = true,
                    IsActive = true
                };

                await userManager.CreateAsync(student5, "AnaGei14$");

            }
        }

        public async static void FeedDatabase(IApplicationBuilder applicationBuilder)
        {

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
                        new() {
                            CourseName = "Curso de C#",
                            InstructorId = instructorFilled.Id,
                            Description = "Este curso proporciona a los participantes una sólida introducción al lenguaje de programación C#. Diseñado para principiantes",
                            MaxUsers = 30,
                            CurrentUsers = 0,
                            Day = "Lunes",
                            StartHour = "8:00",
                            EndHour = "11:00",
                            IsActive = true
                        },
                        new()
                        {
                            CourseName = "Curso de Python",
                            InstructorId = instructorFilled.Id,
                            Description = "Este curso ofrece a los participantes una introducción completa al lenguaje de programación Python",
                            MaxUsers = 30,
                            CurrentUsers = 0,
                            Day = "Martes",
                            StartHour = "12:00",
                            EndHour = "14:00",
                            IsActive = true
                        },

                    };

                    context.Courses.AddRange(courses);

                }



                context.SaveChanges();
            }
        }

        public async static void FeedInscriptions(IApplicationBuilder applicationBuilder)
        {



            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {

                var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                if (context!.Inscriptions.Any()) return;

                var courseCSharp = context.Courses.FirstOrDefault(c => c.CourseName == "Curso de C#");
                var coursePython = context.Courses.FirstOrDefault(c => c.CourseName == "Curso de Python");

                List<ApplicationUser> studentsList = await context.ApplicationUsers.Where(st => st.IsStudent).ToListAsync();





                foreach (ApplicationUser student in studentsList)
                {


                    var studentUser = await userManager.FindByEmailAsync(student.Email!);


                    if (studentUser != null && courseCSharp != null)
                    {
                        var inscription = new Inscription
                        {
                            StudentId = studentUser.Id,
                            CourseId = courseCSharp.Id,
                            DateInscription = DateTime.Now.Date.AddDays(-10),

                        };

                        context.Inscriptions.Add(inscription);
                    }






                }

                context.SaveChanges();

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

                List<ApplicationUser> studentsList = await context.ApplicationUsers.Where(st => st.IsStudent).ToListAsync();

                // Busca a los usuarios que van a asistir a los cursos

                // Añade más estudiantes si es necesario

                // Busca los cursos a los que van a asistir los estudiantes
                var courseCSharp = context.Courses.FirstOrDefault(c => c.CourseName == "Curso de C#");
                var coursePython = context.Courses.FirstOrDefault(c => c.CourseName == "Curso de Python");
                // Añade más cursos si es necesario


                foreach (var student in studentsList)
                {

                    var studentUser = await userManager.FindByEmailAsync(student.Email!);

                    for (int i = 1; i < 10; i++)
                    {

                        if (studentUser != null && courseCSharp != null)
                        {
                            var attendance = new Attendance
                            {
                                CourseId = courseCSharp.Id,
                                StudentId = studentUser.Id,
                                Date = DateTime.Now.Date.AddDays(-i),
                                AttendanceClass = true
                                // Añade más campos si es necesario
                            };

                            context.Attendances.Add(attendance);
                        }

                    }



                }


                context.SaveChanges();
            }
        }

    }
}