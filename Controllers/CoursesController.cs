using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniSportUAQ_API.Data.Consts;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Services;

namespace UniSportUAQ_API.Controllers
{

    [ApiController]
    [Route("api/courses")]
    public class CoursesController: Controller
    {

        private readonly ICoursesService _coursesService;
            
        public CoursesController(ICoursesService coursesService) {

            _coursesService = coursesService;

        }


        [HttpGet]
        [Route("id/{id}")]
        [Authorize]
        public async Task<IActionResult> GetCourseById(string Id)
        {
            if (!Guid.TryParse(Id, out _)) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _coursesService.GetCourseByIdAsync(Id);

            if (result.Count > 0) return Ok(new DataResponse { Data = result[0].ToDictionaryForIdRequest(), ErrorMessage = null });

            return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND});
        }

        [HttpGet]
        [Route("name/{name}")]
        [Authorize]

        public async Task<IActionResult> GetCourseByName(string name) {

            var result = await _coursesService.GetCourseByNameAsync(name);

            if (result.Count > 0) return Ok(new DataResponse { Data = result[0].ToDictionaryForCourseNameRequest(), ErrorMessage = null });

            return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND }); 
        }


        [HttpGet]
        [Route("instructorid/{instructorid}")]
        [Authorize]
        public async Task<IActionResult> GetCourseByInstructorId(string instructorid) 
        {

            if (!Guid.TryParse(instructorid, out _)) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _coursesService.GetCourseByIdInstructor(instructorid);

            if (result.Count > 0) return Ok(new DataResponse { Data = result[0].ToDictionaryForInstructorIdRequest(), ErrorMessage = null });

            return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
            
        }
    }
}
