using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniSportUAQ_API.Data.Consts;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;
using UniSportUAQ_API.Data.Services;

namespace UniSportUAQ_API.Controllers
{
    [ApiController]
    [Route("api/cartasLiberacion")]
    public class CartasLiberacionController: Controller
    {

        private readonly ICartasLiberacionService _cartasLiberacionService;
        private readonly ICoursesService _coursesService;
        private readonly IStudentsService _studentsService;
        private readonly IInscriptionsService _inscriptionsService;

        public CartasLiberacionController(ICartasLiberacionService cartasLiberacionService, ICoursesService coursesService, IStudentsService studentsService, IInscriptionsService inscriptionsService) {

            _cartasLiberacionService = cartasLiberacionService;
            _coursesService = coursesService;
            _studentsService = studentsService;
            _inscriptionsService = inscriptionsService;


        }

        [HttpGet]
        [Route("id/{id}")]
        [Authorize]

        public async Task<IActionResult> GetCartaByIdAsync(string id) {

            if (!Guid.TryParse(id, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _cartasLiberacionService.GetCartaByIdAsync(id);

            if (result is not null) return Ok(new DataResponse { Data = result.Dictionary, ErrorMessage = null });

            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

        }

        [HttpGet]
        [Route("course/{courseid}")]
        [Authorize]

        public async Task<IActionResult> GetCartaByCourseIdAsync(string courseId) {

            if (!Guid.TryParse(courseId, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _cartasLiberacionService.GetCartaByCourseIdAsync(courseId);

            var data = new List<Dictionary<string, object>>();

            foreach (var item in result) data.Add(item.Dictionary);

            if (result.Count > 0) return Ok(new DataResponse { Data = data, ErrorMessage = null });

            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
        }

        [HttpGet]
        [Route("student/{studentid}")]
        [Authorize]
        public async Task<IActionResult> GetCartaByStudentIdAsync(string studentid) {

            if (!Guid.TryParse(studentid, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _cartasLiberacionService.GetCartaByStudentIdAsync(studentid);

            var Data = new List<Dictionary<string, object>>();

            foreach (var item in result) Data.Add(item.Dictionary);

            if (result.Count > 0) return Ok(new DataResponse { Data = Data, ErrorMessage = null });

            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

        }

        [HttpPost]
        [Route("create")]
        [Authorize]

        public async Task<IActionResult> CreateCartaAsync([FromBody] CartaLiberacionSchema cartaLiberacionSchema) {

            string? studentId = cartaLiberacionSchema.StudentId;
            string? courseId = cartaLiberacionSchema.CourseId;

            //check if student and course exist
            if (await _studentsService.GetStudentByIdAsync(studentId) is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = "student:"+ResponseMessages.OBJECT_NOT_FOUND });
            if (await _coursesService.GetCourseByIdAsync(courseId) is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = "course:" + ResponseMessages.OBJECT_NOT_FOUND });

            //check if student is inscribed
            if (!await _inscriptionsService.CheckInscriptionByCourseIdAndStudentIdAsync(courseId,studentId)) return BadRequest(new DataResponse { Data = null, ErrorMessage = "Inscription:" + ResponseMessages.NOT_FOUND_IN_COURSE});
            
            
            var result = await _cartasLiberacionService.GetCartaByStudentIdAsync(studentId);

            if (result is not null && result.Count >= 3) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.LIBERATION_LIMIT});

            //TO DO: generate file in fire base and get url

            string? url = "This is url";

            cartaLiberacionSchema.Url= url;

            var carta = new CartaLiberacion { 

                Id = Guid.NewGuid().ToString(),
                CourseId = cartaLiberacionSchema.CourseId,
                StudentId= cartaLiberacionSchema.StudentId,
                Url = cartaLiberacionSchema.Url,
                
            };

            var cartaRegister = await _cartasLiberacionService. CreateCartaAsync(carta);

            return Ok(new DataResponse { Data = cartaRegister.Dictionary, ErrorMessage = null});
        }




    }
}
