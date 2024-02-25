using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using UniSportUAQ_API.Data.Consts;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;
using UniSportUAQ_API.Data.Services;

namespace UniSportUAQ_API.Controllers
{

    [ApiController]
    [Route("api/courses")]
    public class CoursesController: Controller
    {

        private readonly ICoursesService _coursesService;
        private readonly IInscriptionsService _inscriptionsService;
            
        public CoursesController(ICoursesService coursesService, IInscriptionsService inscriptionsService)
        {
            _coursesService = coursesService;
            _inscriptionsService = inscriptionsService;
        }

        [HttpGet]
        [Route("id/{id}")]
        [Authorize]
        public async Task<IActionResult> GetCourseById(string Id)
        {
            if (!Guid.TryParse(Id, out _)) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _coursesService.GetCourseByIdAsync(Id);

            if (result is not null) return Ok(new DataResponse { Data = result.ToDictionaryForIdRequest(), ErrorMessage = null });

            return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND});
        }

        [HttpGet]
        [Route("name/{name}")]
        [Authorize]

        public async Task<IActionResult> GetCourseByName(string name) {

            var result = await _coursesService.GetCourseByNameAsync(name);

            if (result.Count > 0) return Ok(new DataResponse { Data = result[0].ToDictionaryForIdRequest(), ErrorMessage = null });

            return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND }); 
        }


        [HttpGet]
        [Route("instructorid/{instructorid}")]
        [Authorize]
        public async Task<IActionResult> GetCourseByInstructorId(string instructorid) 
        {

            if (!Guid.TryParse(instructorid, out _)) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _coursesService.GetCourseByIdInstructor(instructorid);

            if (result.Count > 0) return Ok(new DataResponse { Data = result[0].ToDictionaryForIdRequest(), ErrorMessage = null });

            return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
            
        }

        [HttpPost]
        [Route("inscription/{courseId}")]
        [Authorize]
        public async Task<IActionResult> AddToCourse(string courseId, [FromBody]StudentSchema student)
        {
            if (string.IsNullOrEmpty(student.Id)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.STUDENT_ID_NEEDED });

            var checkIfInCourse = await _inscriptionsService.CheckInscriptionByCourseIdAndStudentIdAsync(courseId, student.Id);

            if (checkIfInCourse) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.ALREADY_IN_COURSE });

            
                var course = await _coursesService.GetCourseByIdAsync(courseId);

            if (course is not null)
            {
                course.CurrentUsers++;
                // course.PendingUsers--; // TODO: check for this line in case to implement a way to reserve places in the course.

                await _coursesService.UpdateCourseAsync(course);
            }


            var inscriptionResult = await _inscriptionsService.CreateInscriptionAsync(courseId, student.Id);

            return Ok(new DataResponse { Data = inscriptionResult, ErrorMessage = null});
        }
    }
}
