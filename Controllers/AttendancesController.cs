using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using UniSportUAQ_API.Data.Consts;
using UniSportUAQ_API.Data.Interfaces;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;
using UniSportUAQ_API.Data.Services;
using UniSportUAQ_API.Data.Base;
using UniSportUAQ_API.Data;
using Microsoft.EntityFrameworkCore;
using UniSportUAQ_API.Data.DTO;

namespace UniSportUAQ_API.Controllers
{
    [ApiController]
    [Route("api/v1/attendances")]
    public class AttendancesController : Controller
    {

        private readonly IAttendancesService _atenndancesService;
        private readonly ICoursesService _coursesService;
        private readonly IStudentsService _studentsService;
        private readonly IUtilsService _utilsService;
        private readonly IHorariosService _horariosService;
        private readonly IInscriptionsService _inscriptionsService;
        private readonly AppDbContext _context;

        public AttendancesController(AppDbContext context, IInscriptionsService inscriptionsService, IHorariosService horariosService, IAttendancesService attendancesService, ICoursesService coursesService, IStudentsService studentsService, IUtilsService utilsService)
        {
            _inscriptionsService = inscriptionsService;
            _atenndancesService = attendancesService;
            _coursesService = coursesService;
            _studentsService = studentsService;
            _utilsService = utilsService;
            _horariosService = horariosService;
            _context = context;
        }


        [HttpGet]
        [Route("id/{id}")]
        [Authorize]

        public async Task<IActionResult> GetAttendanceByIdAsync(string id)
        {
            if (!Guid.TryParse(id, out _)) return BadRequest(new BaseResponse<AttendanceDTO> { Error = ResponseErrors.AttributeIdInvalidlFormat });

            var result = await _atenndancesService.GetByIdAsync(id,
                a => a.Course!,
                a => a.Student!,
                a => a.Course!.Instructor!
            );

            if (result is not null)
            {
                var attendanceDTO = new AttendanceDTO
                {
                    Id = result.Id,
                    StudentId = result.StudentId,
                    CourseId = result.CourseId,
                    Date = result.Date,
                    AttendanceClass = result.AttendanceClass
                };

                return Ok(new BaseResponse<AttendanceDTO> { Data = attendanceDTO });
            }

            return Ok(new BaseResponse<AttendanceDTO> { Data = null, Error = ResponseErrors.DataNotFound });
        }


        [HttpGet]
        [Route("course/{courseid}")]
        [Authorize]

        public async Task<IActionResult> GetAttendancesByCourseIdAsync(string courseid)
        {

            if (!Guid.TryParse(courseid, out _)) return BadRequest(new BaseResponse<List<AttendanceDTO>> { Error = ResponseErrors.AttributeIdInvalidlFormat });

            var result = await _atenndancesService.GetAllAsync(a => a.CourseId == courseid,
                a => a.Course!,
                a => a.Student!,
                a => a.Course!.Instructor!
            );

            var data = new List<AttendanceDTO>();

            foreach (var item in result)
            {
                var attendanceDTO = new AttendanceDTO
                {
                    Id = item.Id,
                    StudentId = item.StudentId,
                    CourseId = courseid,
                    Date = item.Date,
                    AttendanceClass = item.AttendanceClass
                };
                data.Add(attendanceDTO);
            }

            return Ok(new BaseResponse<List<AttendanceDTO>> { Data = data });
        }

        [HttpGet]
        [Route("student/{studentid}")]
        [Authorize]
        public async Task<IActionResult> GetAttendancesByStudentIdAsync(string studentid)
        {

            if (!Guid.TryParse(studentid, out _)) return BadRequest(new BaseResponse<List<AttendanceDTO>> { Error = ResponseErrors.AttributeIdInvalidlFormat });

            var result = await _atenndancesService.GetAllAsync(a => a.StudentId == studentid,
                a => a.Course!,
                a => a.Student!,
                a => a.Course!.Instructor!
            );

            var data = new List<AttendanceDTO>();

            foreach (var item in result)
            {
                var attendanceDTO = new AttendanceDTO
                {
                    Id = item.Id,
                    StudentId = item.StudentId,
                    CourseId = item.CourseId,
                    Date = item.Date,
                    AttendanceClass = item.AttendanceClass
                };
                data.Add(attendanceDTO);
            }

            return Ok(new BaseResponse<List<AttendanceDTO>> { Data = data, Error = null });
        }


        [Route("create")]
		[HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateAttendanceAsync([FromBody] AttendanceSchema attendanceSchema)
        {

            if (!Guid.TryParse(attendanceSchema.StudentId, out _)) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeIdInvalidlFormat });
            if (!Guid.TryParse(attendanceSchema.CourseId, out _)) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeIdInvalidlFormat });

            if (await _studentsService.GetByIdAsync(attendanceSchema.StudentId) is null) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AuthUserNotFound });
            if (await _coursesService.GetByIdAsync(attendanceSchema.CourseId) is null) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AuthUserNotFound });

            //check if student is in course
            var studentEnrolled = await _inscriptionsService.GetAllAsync(i =>
            i.CourseId == attendanceSchema.CourseId &&
            i.StudentId == attendanceSchema.StudentId);

            if (!studentEnrolled.Any()) return Ok(new BaseResponse<bool> { Error = ResponseErrors.CourseNotFoundInscription });

            //get the course and 
            var course = await _coursesService.GetByIdAsync(attendanceSchema.CourseId);

            if (course == null) return Ok(new BaseResponse<bool> { Error = ResponseErrors.DataNotFound });


            var dateServ = _utilsService.GetServerDateAsync();

            attendanceSchema.Date = dateServ.Date;

            //review if coincide with course day of week

            string newDateDayOfWeek = attendanceSchema.Date.ToString("dddd", new CultureInfo("en-US")).ToLower();
            string newDateDayOfWeekSpanish = attendanceSchema.Date.ToString("dddd", new CultureInfo("es-ES")).ToLower();

            var horarios = await _horariosService.GetAllAsync(i => i.CourseId! == course!.Id);

            bool CorrectDay = false;

            foreach (var horario in horarios!)
            {

                var day = horario.Day!.ToLower();

                Console.WriteLine($"day = {day} newDateDayOfWeek = {newDateDayOfWeek}");


                if (day == newDateDayOfWeek) CorrectDay = true;
                if (day == newDateDayOfWeekSpanish) CorrectDay = true;


            }

            if (CorrectDay is false) return Ok(new BaseResponse<bool> { Error = ResponseErrors.CourseWrongScheduleAttendance });


            var result = await _atenndancesService.GetAllAsync(a => a.CourseId == attendanceSchema.CourseId && a.StudentId == attendanceSchema.StudentId);

            if (result is not null) foreach (var att in result)
                {
                    if (att.Date == attendanceSchema.Date) return Ok(new BaseResponse<bool> { Error = ResponseErrors.EntityExist });

                }


            var attendance = new Attendance
            {

                Id = Guid.NewGuid().ToString(),
                CourseId = attendanceSchema.CourseId,
                StudentId = attendanceSchema.StudentId,
                AttendanceClass = attendanceSchema.AttendanceClass,
                Date = attendanceSchema.Date,

            };


            var attendanceRegister = await _atenndancesService.CreateAttendanceAsync(attendance);

            if (attendanceRegister is null) return Ok(new BaseResponse<bool> { Error = ResponseErrors.EntityNotExist });

            return Ok(new BaseResponse<bool> { Data = true, Error = null });


        }

		[HttpPost]
		[Route("register/all/{courseId}")]
		[Authorize]
		public async Task<IActionResult> RegisterAllAttendances([FromBody] List<AttendanceSchema> Attendances, string courseId)
		{
			if (string.IsNullOrEmpty(courseId) || string.IsNullOrWhiteSpace(courseId))
				return BadRequest(new BaseResponse<bool> { Data = false, Error = ResponseErrors.AttributeIdInvalidlFormat });

			if (!Attendances.Any())
				return BadRequest(new BaseResponse<bool> { Data = false, Error = ResponseErrors.AttributeSchemaEmpty });

			var course = await _coursesService.GetByIdAsync(courseId);
			var schedules = await _horariosService.GetAllAsync(i => i.CourseId == courseId);

			if (course == null || schedules.Count() < 0)
				return Ok(new BaseResponse<bool> { Data = false, Error = ResponseErrors.CourseNotFound });

			
			var serverTime = DateTime.UtcNow;

			var mexicoCityTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
			var today = TimeZoneInfo.ConvertTimeFromUtc(serverTime, mexicoCityTimeZone);

			var todayName = today.ToString("dddd", new CultureInfo("en-US")).ToLower();
			var currentTimeSpan = new TimeSpan(today.Hour, today.Minute, today.Second);

			List<Attendance> newAttendances = new List<Attendance>();

			bool isWithinSchedule = false;

			foreach (var schedule in schedules)
			{
				var scheduleDay = schedule.Day!.ToLower();

				if (scheduleDay == todayName && currentTimeSpan >= schedule.StartHour && currentTimeSpan <= schedule.EndHour)
				{
					isWithinSchedule = true;
					break;
				}
			}

			if (!isWithinSchedule)
			{
				return Ok(new BaseResponse<bool> { Data = false, Error = ResponseErrors.CourseWrongScheduleAttendance });
			}

			foreach (var attendance in Attendances)
			{
				if (string.IsNullOrEmpty(attendance.CourseId) || string.IsNullOrWhiteSpace(attendance.CourseId))
					return Ok(new BaseResponse<bool> { Data = false, Error = ResponseErrors.AttributeEmptyOrNull });

				if (string.IsNullOrEmpty(attendance.StudentId) || string.IsNullOrWhiteSpace(attendance.StudentId))
					return Ok(new BaseResponse<bool> { Data = false, Error = ResponseErrors.AttributeEmptyOrNull });

				var NewAttendance = new Attendance
				{
					Id = Guid.NewGuid().ToString(),
					CourseId = courseId,
					StudentId = attendance.StudentId,
					Date = attendance.Date,
					AttendanceClass = attendance.AttendanceClass,
				};

				newAttendances.Add(NewAttendance);
			}

			try
			{
				await _context.AddRangeAsync(newAttendances);
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException)
			{
				return Ok(new BaseResponse<bool> { Data = false, Error = ResponseErrors.ServerDataBaseError });
			}

			return Ok(new BaseResponse<bool> { Data = true });
		}


		[Authorize(Roles = UserRoles.Instructor)]
		[HttpGet]
		[Route("available/course/{courseId}")]
		public async Task<IActionResult> CheckCourseAviableAttendance(string courseId)
		{
			if (string.IsNullOrEmpty(courseId) || string.IsNullOrWhiteSpace(courseId))
				return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeIdInvalidlFormat });

			var course = await _coursesService.GetByIdAsync(courseId, i => i.Horarios!);
			if (course == null)
				return Ok(new BaseResponse<bool> { Error = ResponseErrors.EntityNotExist });

			var schedules = await _horariosService.GetAllAsync(i => i.CourseId == course.Id);
			if (schedules == null || !schedules.Any())
				return Ok(new BaseResponse<bool> { Error = ResponseErrors.CourseNoSchedule });

			var serverTime = DateTime.UtcNow;

			var mexicoCityTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
			var today = TimeZoneInfo.ConvertTimeFromUtc(serverTime, mexicoCityTimeZone);

			var todayName = today.ToString("dddd", new CultureInfo("en-US")).ToLower();
			var currentTimeSpan = new TimeSpan(today.Hour, today.Minute, today.Second);

			foreach (var schedule in schedules)
			{
				var scheduleDay = schedule.Day!.ToLower();
				if (scheduleDay == todayName && currentTimeSpan >= schedule.StartHour && currentTimeSpan <= schedule.EndHour)
				{
					var attendances = await _atenndancesService.GetAllAsync(a =>
						a.CourseId == course.Id &&
						a.Date.Date == today.Date);

					if (attendances.Any())
					{
						return Ok(new BaseResponse<bool>
						{
							Data = false,
							Error = ResponseErrors.CourseAlreadyPassedAttendnaces
						});
					}
					else
					{
						return Ok(new BaseResponse<bool> { Data = true, });
					}
				}
			}

			return Ok(new BaseResponse<bool> { Data = false, Error = ResponseErrors.CourseWrongScheduleAttendance });
		}


		[HttpPut]
        [Route("update")]
        [Authorize]
        public async Task<IActionResult> UpdateAttendanceAsync([FromBody] AttendanceSchema attendanceSchema)
        {

            if (attendanceSchema.Id is null) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeIdInvalidlFormat });
            if (attendanceSchema.CourseId is null) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeIdInvalidlFormat });
            if (attendanceSchema.StudentId is null) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeIdInvalidlFormat });
            if (attendanceSchema.Date == DateTime.MinValue) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AuthInvalidData });

            var oldAttendance = await _atenndancesService.GetByIdAsync(attendanceSchema.Id);

            if (oldAttendance == null) return Ok(new BaseResponse<bool> { Error = ResponseErrors.EntityNotExist });

            oldAttendance.AttendanceClass = attendanceSchema.AttendanceClass;

            var result = await _atenndancesService.UpdateAsync(oldAttendance);

            if (result is not null) return Ok(new BaseResponse<bool> { Data = true, Error = null });

            return Ok(new BaseResponse<bool> { Data = false, Error = ResponseErrors.ServerDataBaseErrorUpdating });


        }
    }
}
