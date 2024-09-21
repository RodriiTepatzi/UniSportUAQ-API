using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniSportUAQ_API.Data;

using UniSportUAQ_API.Data.Interfaces;
using UniSportUAQ_API.Data.Models;
using System.Globalization;
namespace UniSportUAQ_API.Controllers
{
    [Route("api/v1/hangfire")]
    [ApiController]
    public class JobsController : Controller
    {
        private readonly IAttendancesService _attendancesService;
        private readonly ICoursesService _coursesService;
        private readonly IInscriptionsService _inscriptionsService;

        public JobsController(IAttendancesService attendancesService, ICoursesService coursesService, IInscriptionsService inscriptionsService) 
        {
            _attendancesService = attendancesService;
            _coursesService = coursesService;
            _inscriptionsService = inscriptionsService;
        }




        [HttpPost]
        [Route("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyAssistences()
        {
            try
            {
                // Obtener la zona horaria de Ciudad de México
                TimeZoneInfo cdmxTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");

                // Obtener la fecha y hora actual en la zona horaria de Ciudad de México
                DateTime cdmxDateTime = TimeZoneInfo.ConvertTime(DateTimeOffset.Now.Date, cdmxTimeZone);

                var ActiveCourses = await _coursesService.GetAllAsync(i => i.IsActive);

                if (!ActiveCourses.Any()) return NotFound();

                foreach (var course in ActiveCourses)
                {
                    var day = course.Day;
                    TimeOnly checkHour = TimeOnly.Parse(course.EndHour!);
                    checkHour = checkHour.AddHours(1);

                    string? cronExpresion = GetCronExpressionForDayAndTime(day!, checkHour.ToString("HH:mm"));

                     Console.WriteLine($"cdmxDateTime {cdmxDateTime}");

                    if (cronExpresion != null)
                    {
                        var options = new RecurringJobOptions
                        {
                            TimeZone = cdmxTimeZone
                        };

                        RecurringJob.AddOrUpdate(
                            $"job-{day}",
                            () => CheckTodayAttendance(cdmxDateTime, course.Id!, day!.ToLower()),
                            cronExpresion,
                            options
                        );
                    }
                    else
                    {
                         Console.WriteLine($"Not valid day: {day!.ToLower()} or var CronExpression null");
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                 Console.WriteLine($"Error dates {ex}");
                return BadRequest();
            }
        }

		// Método sincrónico que llama al método asincrónico
		[ApiExplorerSettings(IgnoreApi = true)]
        public void CheckTodayAttendance(DateTime date, string courseId, string day)
        {
            CheckTodayAttendanceAsync(date, courseId, day).GetAwaiter().GetResult();
        }

		[ApiExplorerSettings(IgnoreApi = true)]
        public async Task CheckTodayAttendanceAsync(DateTime date, string courseId, string day)
        {
             Console.WriteLine($"Executing VerifyAssistence to day: {day}");

            var Attendances = await _attendancesService.GetAllAsync(i =>
                i.CourseId == courseId &&
                i.Date == date);

            if (Attendances.Count() > 0)
            {
                 Console.WriteLine($"Intructor for course with {courseId} has taken attendances for day:{day}");
            }
            else
            { 

                 Console.WriteLine($"Intructor for course with {courseId} has NOT TAKEN attendances for day:{day}");

                // Obtener inscripciones para el curso
                var inscriptions = await _inscriptionsService.GetAllAsync(i =>
                    i.CourseId == courseId);

                if (!inscriptions.Any())
                {
                     Console.WriteLine($"this course {courseId} has NOT inscriptions");
                }

                // Generar asistencias
                foreach (var inscription in inscriptions)
                {
                    var attendance = new Attendance
                    {
                        Id = Guid.NewGuid().ToString(),
                        CourseId = inscription.CourseId,
                        StudentId = inscription.StudentId,
                        Date = date,
                        AttendanceClass = true,
                    };


                    var newattendance = await _attendancesService.AddAsync(attendance);

                    if (newattendance is not null)
                    {
                        if (inscription.Student != null && inscription.Course != null)
                        {
                             Console.WriteLine($"Generated attendance for student with exp {inscription.Student.Expediente} for course with id:{inscription.Course.Id}");
                        }
                        else
                        {
                             Console.WriteLine($"Student or Course information is missing for inscription with CourseId: {inscription.CourseId}");
                        }
                    }
                    else
                    {
                         Console.WriteLine($"Can not generate attendance for student with exp {inscription.Student!.Expediente} for course with id:{inscription.Course!.Id}");
                    }
                }
            }
        }




        private string? GetCronExpressionForDay(string day)
        {
            return day.ToLower() switch
            {
                "monday" => Cron.Hourly(),
                "lunes" => Cron.Weekly(DayOfWeek.Monday),
                "tuesday" => Cron.Weekly(DayOfWeek.Tuesday),
                "martes" => Cron.Weekly(DayOfWeek.Tuesday),
                "wednesday" => Cron.Weekly(DayOfWeek.Wednesday),
                "miercoles" => Cron.Weekly(DayOfWeek.Wednesday),
                "thursday" => Cron.Weekly(DayOfWeek.Thursday),
                "jueves" => Cron.Weekly(DayOfWeek.Thursday),
                "friday" => Cron.Weekly(DayOfWeek.Friday),
                "viernes" => Cron.Weekly(DayOfWeek.Friday),
                "saturday" => Cron.Weekly(DayOfWeek.Saturday),
                "sabado" => Cron.Weekly(DayOfWeek.Saturday),
                "sunday" => Cron.Weekly(DayOfWeek.Sunday),
                "domingo" => Cron.Weekly(DayOfWeek.Sunday),
                _ => null 
            };
        }

        private string? GetCronExpressionForDayAndTime(string day, string time)
        {
            var timeParts = time.Split(':');
            if (timeParts.Length != 2) return null;

            var minute = timeParts[1];
            var hour = timeParts[0];

             Console.WriteLine($"day {day}, hour {hour}, minute{minute}");

            return day.ToLower() switch
            {
                "monday" => $"{minute} {hour} * * 1",
                "lunes" => $"{minute} {hour} * * 1",
                "tuesday" => $"{minute} {hour} * * 2",
                "martes" => $"{minute} {hour} * * 2",
                "wednesday" => $"{minute} {hour} * * 3",
                "miercoles" => $"{minute} {hour} * * 3",
                "miércoles" => $"{minute} {hour} * * 3",
                "thursday" => $"{minute} {hour} * * 4",
                "jueves" => $"{minute} {hour} * * 4",
                "friday" => $"{minute} {hour} * * 5",
                "viernes" => $"{minute} {hour} * * 5",
                "saturday" => $"{minute} {hour} * * 6",
                "sabado" => $"{minute} {hour} * * 6",
                "sábado" => $"{minute} {hour} * * 6",
                "sunday" => $"{minute} {hour} * * 0",
                "domingo" => $"{minute} {hour} * * 0",
                _ => null
            };
        }
    }
}
