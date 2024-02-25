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

            if (string.IsNullOrEmpty(attendance.CourseId) && string.IsNullOrEmpty(attendance.StudentId)) return BadRequest(new DataResponse { Data = null, ErrorMessage = "error en el isnullorempty" });

            attendance.Date=DateTime.Now.Date;

            var attendanceEntity = await _atenndancesService.GetAttendanceForValidationAsync(attendance.CourseId, attendance.StudentId, attendance.Date);

            if (attendanceEntity.Count > 0) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.ENTITY_EXISTS });

            Guid guid = Guid.NewGuid();
            DateTime dateTime = DateTime.Now.Date;
            attendance.Id = guid.ToString();
            attendance.Date = dateTime;

            var result = await _atenndancesService.CreateAttendanceAsync(attendance);

            return Ok(new DataResponse { Data = result, ErrorMessage = null });
        }
    }
}
