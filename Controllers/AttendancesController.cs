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
            if (!Guid.TryParse(id, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _atenndancesService.GetByIdAsync(id,
                a => a.Course!,
                a => a.Student!,
                a => a.Course!.Instructor!
            );

            if (result is not null) return Ok(new DataResponse { Data = result.Dictionary, ErrorMessage = null });

            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
        }

        [HttpGet]
        [Route("course/{courseid}")]
        [Authorize]

        public async Task<IActionResult> GetAttendancesByCourseIdAsync(string courseid) {

            if (!Guid.TryParse(courseid, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _atenndancesService.GetAllAsync(a => a.CourseId == courseid,
                a => a.Course!,
                a => a.Student!,
                a => a.Course!.Instructor!
            );

            var data = new List<Dictionary<string, object>>();

            foreach (var item in result) data.Add(item.Dictionary);

            if (result.Any()) return Ok(new DataResponse { Data = data, ErrorMessage = null });

            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
        }

        [HttpGet]
        [Route("student/{studentid}")]
        [Authorize]
        public async Task<IActionResult> GetAttendancesByStudentIdAsync(string studentid) {

            if (!Guid.TryParse(studentid, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _atenndancesService.GetAllAsync(a => a.StudentId == studentid,
                a => a.Course!,
                a => a.Student!,
                a => a.Course!.Instructor!
            );

            var Data = new List<Dictionary<string, object>>();

            foreach (var item in result) Data.Add(item.Dictionary);

            if (result.Any()) return Ok(new DataResponse { Data = Data, ErrorMessage = null });

            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

        }

        [HttpGet]
        [Route("course/{courseid}/day/{day}")]
        [Authorize]
        public async Task<IActionResult> GetAttendancesByCourseIdByDayAsync(string courseid, string day)
        {
            if (!Guid.TryParse(courseid, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            if (!DateTime.TryParse(day, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _atenndancesService.GetAllAsync(a => a.CourseId == courseid,
                a => a.Course!,
                a => a.Student!,
                a => a.Course!.Instructor!
            );

            DateTime dayDate = DateTime.Parse(day);

            var Data = new List<Dictionary<string, object>>();

            foreach (var item in result) {

                if (item.Date.Date == dayDate.Date) Data.Add(item.Dictionary);
            }

            if (Data.Count > 0) return Ok(new DataResponse { Data = Data, ErrorMessage = null });

            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
        }

        [HttpGet]
        [Route("course/{courseid}/student/{studentid}")]
        [Authorize]
        public async Task<IActionResult> GetAttendancesByCourseIdStudentIdAsync(string courseid, string studentid) {

            if (!Guid.TryParse(courseid, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            if (!Guid.TryParse(studentid, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _atenndancesService.GetAllAsync(a => a.CourseId == courseid && a.StudentId == studentid,
                a => a.Course!,
                a => a.Student!,
                a => a.Course!.Instructor!
            );

            var Data = new List<Dictionary<string, object>>();

            foreach (var item in result) Data.Add(item.Dictionary);

            if (Data.Count > 0) return Ok(new DataResponse { Data = Data, ErrorMessage = null });

            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
        }

        [HttpGet]
        [Route("course/{courseid}/start/{start}/end/{end}")]
        [Authorize]
        public async Task<IActionResult> GetAttendancesByCourseTimeLapse(string courseId, string start, string end) {

            if (!Guid.TryParse(courseId, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = "error en curso" + ResponseMessages.BAD_REQUEST });

            if (!DateTime.TryParse(start, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = "error en fecha inicio" + ResponseMessages.BAD_REQUEST });

            if (!DateTime.TryParse(end, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = "error en fecha final" + ResponseMessages.BAD_REQUEST });

            DateTime dateStart = DateTime.Parse(start).Date;
            DateTime dateEnd = DateTime.Parse(end).Date;

            var result = await _atenndancesService.GetAllAsync(a => a.CourseId == courseId && a.Date >= dateStart && a.Date <= dateEnd,
                a => a.Course!,
                a => a.Student!,
                a => a.Course!.Instructor!
            );

            if (result == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            var data = new List<Dictionary<string, object>>();

            foreach (var element in result) data.Add(element.Dictionary);

            if (data.Count > 0) return Ok(new DataResponse { Data = data, ErrorMessage = null });

            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
        }



        [HttpGet]
        [Route("course/{courseid}/student/{studentid}/start/{start}/end/{end}")]
        [Authorize]
        public async Task<IActionResult> GetAttendancesByCourseStudentTimeLapse(string courseId, string studentId, string start, string end)
        {

            if (!Guid.TryParse(courseId, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = "error en curso" + ResponseMessages.BAD_REQUEST });

            if (!Guid.TryParse(studentId, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = "error en student" + ResponseMessages.BAD_REQUEST });

            if (!DateTime.TryParse(start, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = "error en fecha inicio" + ResponseMessages.BAD_REQUEST });

            if (!DateTime.TryParse(end, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = "error en fecha final" + ResponseMessages.BAD_REQUEST });

            DateTime dateStart = DateTime.Parse(start).Date;
            DateTime dateEnd = DateTime.Parse(end).Date;

            var result = await _atenndancesService.GetAllAsync(a => a.CourseId == courseId && a.StudentId == studentId && a.Date >= dateStart && a.Date <= dateEnd,
                a => a.Course!,
                a => a.Student!,
                a => a.Course!.Instructor!
            );

            if (result == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            var data = new List<Dictionary<string, object>>();

            foreach (var element in result) data.Add(element.Dictionary);

            if (data.Count > 0) return Ok(new DataResponse { Data = data, ErrorMessage = null });

            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
        }



        [HttpGet]
        [Route("course/{courseid}/student/{studentid}/day/{day}")]
        [Authorize]
        public async Task<IActionResult> GetAttendanceByDayAsync(string courseId, string studentId, string day)
        {

            if (!Guid.TryParse(courseId, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = "error en curso" + ResponseMessages.BAD_REQUEST });

            if (!Guid.TryParse(studentId, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = "error en student" + ResponseMessages.BAD_REQUEST });

            if (!DateTime.TryParse(day, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = "error en fecha" + ResponseMessages.BAD_REQUEST });

            var result = await _atenndancesService.GetAllAsync(a => a.CourseId == courseId && a.StudentId == studentId,
                a => a.Course!,
                a => a.Student!,
                a => a.Course!.Instructor!
            );

            DateTime date = DateTime.Parse(day).Date;

            if (result is not null) foreach (var attendance in result)
                {
                    if (attendance.Date == date) return Ok(new DataResponse { Data = attendance.Dictionary, ErrorMessage = null });


                }



            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
        }




        [HttpPost]
        [Route("create")]
        [Authorize]

        public async Task<IActionResult> CreateAttendanceAsync([FromBody] AttendanceSchema attendanceSchema) {

            if (!Guid.TryParse(attendanceSchema.StudentId, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (!Guid.TryParse(attendanceSchema.CourseId, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            if (await _studentsService.GetByIdAsync(attendanceSchema.StudentId) is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
            if (await _coursesService.GetByIdAsync(attendanceSchema.CourseId) is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            //check if student is in course
            var studentEnrolled = await _inscriptionsService.GetAllAsync(i =>
            i.CourseId == attendanceSchema.CourseId &&
            i.StudentId == attendanceSchema.StudentId);

            if (!studentEnrolled.Any()) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.NOT_FOUND_IN_COURSE });

            //get the course and 
            var course = await _coursesService.GetByIdAsync(attendanceSchema.CourseId);

            if (course == null) return NotFound(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });


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

            if (CorrectDay is false) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_COURSE_DAY });


            var result = await _atenndancesService.GetAllAsync(a => a.CourseId == attendanceSchema.CourseId && a.StudentId == attendanceSchema.StudentId);

            if (result is not null) foreach (var att in result)
                {
                    if (att.Date == attendanceSchema.Date) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.ATTENDANCE_ENTITY_EXISTS });

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

            if (attendanceRegister is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.ATTENDANCE_ENTITY_EXISTS });

            return Ok(new DataResponse { Data = attendanceRegister.Dictionary, ErrorMessage = null });
        }

        [HttpPost]
        [Route("register/all/{courseId}")]
        [Authorize]
        public async Task<IActionResult> RegisterAllAttendances([FromBody] List<AttendanceSchema> Attendances, string courseId) {


            if (string.IsNullOrEmpty(courseId) || string.IsNullOrWhiteSpace(courseId)) return BadRequest(new BaseResponse<bool> { Data = false, Error = ResponseErrors.AttributeIdInvalidlFormat });
            if (!Attendances.Any()) return BadRequest(new BaseResponse<bool> { Data = false, Error = ResponseErrors.AttributeSchemaEmpty });

            var course = await _coursesService.GetByIdAsync(courseId);
            var schedules = await _horariosService.GetAllAsync(i => i.CourseId == courseId);

            if (course == null || schedules.Count() < 0) return Ok(new BaseResponse<bool> { Data = false, Error = ResponseErrors.CourseNotFound });

            var today = _utilsService.GetServerDateAsync();
            var todayName = today.ToString("dddd", new CultureInfo("en-En")).ToLower();
            var todayNameSpa = today.ToString("dddd", new CultureInfo("es-Es")).ToLower();
            var currentTimeSpan = new TimeSpan(today.Hour, today.Minute, today.Second);

            List<Attendance> newAttendances = new List<Attendance>();


            //check date course
            foreach (var schedule in schedules) {

                var scheduleDay = schedule.Day!.ToLower();
                var scheduleStart = schedule.StartHour;
                var scheduleEnd = schedule.EndHour;

                if (todayName != scheduleDay)
                {
                    if (todayNameSpa != scheduleDay) return Ok(new BaseResponse<bool> { Data = false, Error = ResponseErrors.CourseWrongScheduleAttendance });
                }
                if (currentTimeSpan < scheduleStart || currentTimeSpan > scheduleEnd) return Ok(new BaseResponse<bool> { Data = false, Error = ResponseErrors.CourseWrongScheduleAttendance });

            }

            foreach (var attendance in Attendances) {

                if (string.IsNullOrEmpty(attendance.CourseId) || string.IsNullOrWhiteSpace(attendance.CourseId)) return Ok(new BaseResponse<bool> { Data = false, Error = ResponseErrors.AttributeEmptyOrNull });
                if (string.IsNullOrEmpty(attendance.StudentId) || string.IsNullOrWhiteSpace(attendance.CourseId)) return Ok(new BaseResponse<bool> { Data = false, Error = ResponseErrors.AttributeEmptyOrNull });

                var NewAttendance = new Attendance {

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

        [HttpGet]
        [Route("available/course/{courseId}")]
        [Authorize]
        public async Task<IActionResult> CheckCourseAviableAttendance(string courseId) {

            //validate course id
            if (string.IsNullOrEmpty(courseId) || string.IsNullOrWhiteSpace(courseId)) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeIdInvalidlFormat });
            //check course existence
            var course = await _coursesService.GetByIdAsync(courseId, i => i.Horarios!);

            if(course == null) return Ok(new BaseResponse<bool> { Error = ResponseErrors.EntityNotExist });

            //check schedules to check if is class and hour day
            var today = _utilsService.GetServerDateAsync();
            var todayName = today.ToString("dddd", new CultureInfo("en-Us")).ToLower();
            var todayNameSpa = today.ToString("dddd", new CultureInfo("es-Es")).ToLower();
            var currentTimeSpan = new TimeSpan(today.Hour, today.Minute, today.Second);

            //check date course
            foreach (var schedule in course.Horarios!)
            {

                var scheduleDay = schedule.Day!.ToLower();
                var scheduleStart = schedule.StartHour;
                var scheduleEnd = schedule.EndHour;

                if (todayName != scheduleDay) 
                {
                    if (todayNameSpa != scheduleDay) return Ok(new BaseResponse<bool> { Data = false, Error = ResponseErrors.CourseWrongScheduleAttendance });
                }
                if (currentTimeSpan < scheduleStart || currentTimeSpan > scheduleEnd) return Ok(new BaseResponse<bool> { Data = false, Error = ResponseErrors.CourseWrongScheduleAttendance });

            }

            //return true or false

            return Ok(new BaseResponse<bool> { Data = true });
        }



        [HttpPut]
        [Route("update")]
        [Authorize]
        public async Task<IActionResult> UpdateAttendanceAsync([FromBody] AttendanceSchema attendanceSchema) {

            if (attendanceSchema.Id is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (attendanceSchema.CourseId is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (attendanceSchema.StudentId is null) return  BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (attendanceSchema.Date == DateTime.MinValue) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var oldAttendance = await _atenndancesService.GetByIdAsync(attendanceSchema.Id);

            if (oldAttendance == null) return  BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.ENTITY_EXISTS });


            oldAttendance.AttendanceClass = attendanceSchema.AttendanceClass;
            

            var result = await _atenndancesService.UpdateAsync(oldAttendance);

            if (result is not null) return Ok(new DataResponse { Data = result.Dictionary, ErrorMessage = null });


            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
        }
    }
}
