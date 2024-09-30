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
