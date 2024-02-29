using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security;
using System.Text.RegularExpressions;
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

            if (!Guid.TryParse(courseId, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            if (!Guid.TryParse(studentId, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            if (!DateTime.TryParse(day, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _atenndancesService.GetAttendancesAsync(courseId, studentId);

            DateTime date = DateTime.Parse(day);

            if (result is not null) foreach (var attendance in result)
                {
                    if (attendance is not null && attendance.Date.Date == date.Date) return Ok(new DataResponse { Data = attendance.Dictionary, ErrorMessage = null });

                    break;
                }



            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
        }


        [HttpPost]
        [Route("create")]
        [Authorize]

        public async Task<IActionResult> CreateAttendanceAsync([FromBody] AttendanceSchema attendanceSchema) {

            

            var attendance = new Attendance { 

                Id = Guid.NewGuid().ToString(),
                CourseId = attendanceSchema.CourseId,
                StudentId = attendanceSchema.StudentId,
                AttendanceClass = attendanceSchema.AttendanceClass,
                Date = attendanceSchema.Date,

            };

            return null;
        }
       

          

    }
}
