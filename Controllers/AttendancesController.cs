using IronPython.Runtime.Operations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using UniSportUAQ_API.Data.Consts;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;
using UniSportUAQ_API.Data.Services;

namespace UniSportUAQ_API.Controllers
{
    [ApiController]
    [Route("api/attendances")]
    public class AttendancesController : Controller
    {

        private readonly IAttendancesService _atenndancesService;
        private readonly ICoursesService _coursesService;
        private readonly IStudentsService _studentsService;

        public AttendancesController(IAttendancesService attendancesService, ICoursesService coursesService, IStudentsService studentsService)
        {

            _atenndancesService = attendancesService;
            _coursesService = coursesService;
            _studentsService = studentsService;

        }


        [HttpGet]
        [Route("id/{id}")]
        [Authorize]

        public async Task<IActionResult> GetAttendanceByIdAsync(string id)
        {
            if (!Guid.TryParse(id, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _atenndancesService.GetAttendanceByIdAsync(id);

            if (result is not null) return Ok(new DataResponse { Data = result.Dictionary, ErrorMessage = null });

            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
        }

        [HttpGet]
        [Route("course/{courseid}")]
        [Authorize]

        public async Task<IActionResult> GetAttendancesByCourseIdAsync(string courseid) {

            if (!Guid.TryParse(courseid, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _atenndancesService.GetAttendancesByCourseIdAsync(courseid);

            var data = new List<Dictionary<string, object>>();

            foreach (var item in result) data.Add(item.Dictionary);

            if (result.Count > 0) return Ok(new DataResponse { Data = data, ErrorMessage = null });

            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
        }

        [HttpGet]
        [Route("student/{studentid}")]
        [Authorize]
        public async Task<IActionResult> GetAttendancesByStudentIdAsync(string studentid) {

            if (!Guid.TryParse(studentid, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _atenndancesService.GetAttendancesByStudentIdAsync(studentid);

            var Data = new List<Dictionary<string, object>>();

            foreach (var item in result) Data.Add(item.Dictionary);

            if (result.Count > 0) return Ok(new DataResponse { Data = Data, ErrorMessage = null });

            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

        }
        [HttpGet]
        [Route("course/{courseid}/day/{day}")]
        [Authorize]
        public async Task<IActionResult> GetAttendancesByCourseIdByDayAsync(string courseid, string day)
        {
            if (!Guid.TryParse(courseid, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            if (!DateTime.TryParse(day, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _atenndancesService.GetAttendancesByCourseIdAsync(courseid);

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

            var result = await _atenndancesService.GetAttendancesAsync(courseid, studentid);

            var Data = new List<Dictionary<string, object>>();

            foreach (var item in result) Data.Add(item.Dictionary);

            if (Data.Count > 0) return Ok(new DataResponse { Data = Data, ErrorMessage = null });

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

            var result = await _atenndancesService.GetAttendancesAsync(courseId, studentId);

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

            if(!Guid.TryParse(attendanceSchema.StudentId, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST});
            if (!Guid.TryParse(attendanceSchema.CourseId, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            if (await _studentsService.GetStudentByIdAsync(attendanceSchema.StudentId) is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
            if (await _coursesService.GetCourseByIdAsync(attendanceSchema.CourseId) is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            //get the course
            var course = await _coursesService.GetCourseByIdAsync(attendanceSchema.CourseId);

            //asign asist date to today

            attendanceSchema.Date = DateTime.Now.Date;

            //review if coincide with course day of week

            string newDateDayOfWeek = attendanceSchema.Date.ToString("dddd", new CultureInfo("es-ES")).ToLower();

            string[]? courseDay = course!.Day?.Split(",");

            foreach (string dayElment in courseDay!) {

                dayElment.ToLower();

                if(dayElment != newDateDayOfWeek) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_COURSE_DAY });

            }


            var result = await _atenndancesService.GetAttendancesAsync(attendanceSchema.CourseId, attendanceSchema.StudentId);

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

            if(attendanceRegister is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.ATTENDANCE_ENTITY_EXISTS});

            return Ok(new DataResponse { Data = attendanceRegister.Dictionary, ErrorMessage = null });
        }


        [HttpPut]
        [Route("update")]
        [Authorize]
        public async Task<IActionResult> UpdateAttendanceAsync([FromBody] AttendanceSchema attendanceSchema) {

            if (attendanceSchema.Id is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (attendanceSchema.CourseId is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (attendanceSchema.StudentId is null) return  BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (attendanceSchema.Date == DateTime.MinValue) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });


            var attendance = new Attendance
            {
                Id = attendanceSchema.Id,
                CourseId = attendanceSchema.CourseId,
                StudentId = attendanceSchema.StudentId,
                Date = attendanceSchema.Date,
                AttendanceClass = attendanceSchema.AttendanceClass

            };

            var result = await _atenndancesService.UpDateAttedanceAsync(attendance);

            if (result is not null) return Ok(new DataResponse { Data = result.Dictionary, ErrorMessage = null });


            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND }); ;
        }


       





    }
}
