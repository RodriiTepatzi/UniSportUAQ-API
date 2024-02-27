using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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


        [HttpPost]
        [Route("create")]
        [Authorize]

        public async Task<IActionResult> CreateAttendanceAsync([FromBody] AttendanceSchema attendance)
        {

            if (string.IsNullOrEmpty(attendance.CourseId) && string.IsNullOrEmpty(attendance.StudentId)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            

            var attendanceEntity = await _atenndancesService.GetAttendancesAsync(attendance.CourseId, attendance.StudentId);

            if (attendanceEntity is not null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.ENTITY_EXISTS });

            Guid guid = Guid.NewGuid();
            DateTime dateTime = DateTime.Now.Date;
            attendance.Id = guid.ToString();
            attendance.Date = dateTime;

            var result = await _atenndancesService.CreateAttendanceAsync(attendance);

            return Ok(new DataResponse { Data = result, ErrorMessage = null });
        }


        [HttpGet]
        [Route("id/{id}")]
        [Authorize]
        public async Task<IActionResult> GetAttendanceByIdAsync(string id) {
            //validate id
            if (!Guid.TryParse(id, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            //asigate result
            var result = await _atenndancesService.GetAttendanceByIdAsync(id);
            //check if result is not null
            if (result is not null) return Ok(new DataResponse { Data = result.ToDictionary(), ErrorMessage = null });
            //return null result
            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

        }

        [HttpGet]
        [Route("courseid/{courseid}")]
        [Authorize]
        public async Task<IActionResult> GetAttendanceByCourseIdAsync(string courseid)
        {
            if(!Guid.TryParse(courseid, out _))return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _atenndancesService.GetAttendanceByCourseIdAsync(courseid);

            if (result is not null) return Ok(new DataResponse { Data = result.ToDictionary(), ErrorMessage = null });

            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
        }

        [HttpGet]
        [Route("studentid/{studentid}")]
        [Authorize]
        public async Task<IActionResult> GetAttendanceByStudentIdAsync(string studentid) {

            if (!Guid.TryParse(studentid, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _atenndancesService.GetAttendanceByStudentIdAsync(studentid);

            if (result is not null) return Ok(new DataResponse { Data = result.ToDictionary(), ErrorMessage = null });

            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
        }
        //ignore in production
        [HttpGet]
        [Route("day/{day}")]
        [Authorize]
        public async Task<IActionResult> GetAttendanceByDateAsync(string day)
        {
            if (!DateTime.TryParse(day, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _atenndancesService.GetAttendanceByDateAsync(DateTime.Parse(day));

            if (result is not null) return Ok(new DataResponse { Data = result.ToDictionary(), ErrorMessage = null });

            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
        }

        [HttpGet]
        [Route("attendance/{courseid}/{studentid}")]
        [Authorize]
        public async Task<IActionResult> GetAttendancesAsync(string courseid, string studentid, string day)
        {
            if(!Guid.TryParse(studentid, out _) && !Guid.TryParse(courseid, out _) && !DateTime.TryParse(day, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _atenndancesService.GetAttendancesAsync(courseid, studentid);

            if (result is not null) return Ok(new DataResponse { Data = result.ToDictionary(), ErrorMessage = null });

            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
        }
    }
}
