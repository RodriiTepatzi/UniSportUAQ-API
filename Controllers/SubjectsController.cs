using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using UniSportUAQ_API.Data.Consts;
using UniSportUAQ_API.Data.Interfaces;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;

namespace UniSportUAQ_API.Controllers
{
    [ApiController]
    [Route("api/v1/subjects")]
    public class SubjectsController : Controller
    {
        private readonly ISubjectsService _subjectsService;
        private readonly IInstructorsService _instructorsService;

        public SubjectsController(ISubjectsService subjectsService, IInstructorsService instructorsService)
        {

            _subjectsService = subjectsService;
            _instructorsService = instructorsService;

        }

        [HttpGet]
        [Route("id/{id}")]
        [Authorize]
        public async Task<IActionResult> GetSubjectById(string id)
        {

            if (!Guid.TryParse(id, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _subjectsService.GetByIdAsync(id);

            if (result == null) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            return Ok(new DataResponse { Data = result.ToDictionary(), ErrorMessage = null });


        }

        [HttpPost]
        [Route("create")]
        [Authorize]
        public async Task<IActionResult> CreateSubject([FromBody] SubjectSchema subject) {

            //attrib validations
            if(string.IsNullOrEmpty(subject.Name)) return BadRequest(new DataResponse { Data = null, ErrorMessage ="name: " + ResponseMessages.BAD_REQUEST });
            if(string.IsNullOrEmpty(subject.CoursePictureUrl)) return BadRequest(new DataResponse { Data = null, ErrorMessage = "urlpicture: "+ResponseMessages.BAD_REQUEST });
            
            //name existence
            var courseNameCheck = await _subjectsService.GetAllAsync(i => i.Name!.ToLower() == subject.Name!.ToLower());

            if(courseNameCheck.Count() < 0) return Ok(new DataResponse { Data = null, ErrorMessage = "subject name: " + ResponseMessages.ENTITY_EXISTS });
                
            //create new Subject object
            var newSubject = new Subject { 
                Id = Guid.NewGuid().ToString(),
                Name = subject.Name,
                CoursePictureUrl = subject.CoursePictureUrl,
            };

            //add to EF database
            var registSubject = _subjectsService.AddAsync(newSubject);

            if(registSubject == null) return Ok(new DataResponse { Data = null, ErrorMessage = "subject:" + ResponseMessages.BAD_REQUEST});


            return Ok(new DataResponse { Data = newSubject.ToDictionary(), ErrorMessage = null });
        }

        [HttpPut]
        [Route("update")]
        [Authorize]
        public async Task<IActionResult> UpdateSubject([FromBody] SubjectSchema subject) {

            //attrib val
            if(!Guid.TryParse(subject.Id, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = "Id: " + ResponseMessages.BAD_REQUEST });
            if (!string.IsNullOrEmpty(subject.Name)) return BadRequest(new DataResponse { Data = null, ErrorMessage = "name: " + ResponseMessages.BAD_REQUEST });
            if (!string.IsNullOrEmpty(subject.CoursePictureUrl)) return BadRequest(new DataResponse { Data = null, ErrorMessage = "urlpicture: " + ResponseMessages.BAD_REQUEST });
            
            //verify name existene

            var subjectNameCheck = await _subjectsService.GetAllAsync(i => i.Name!.ToLower() == subject.Name!.ToLower());

            if(subjectNameCheck == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = "subject name: " + ResponseMessages.ENTITY_EXISTS });


            //create object

            var newSubject = new Subject
            {
                Id = subject.Id,
                Name = subject.Name,
                CoursePictureUrl = subject.CoursePictureUrl,

            };

            var result = await _subjectsService.UpdateAsync(newSubject);

            if(result == null) return Ok(new DataResponse { Data = null, ErrorMessage = "subject:" + ResponseMessages.INTERNAL_ERROR });

            return Ok(new DataResponse { Data = result.ToDictionary(), ErrorMessage = null });





        }
    }
}
