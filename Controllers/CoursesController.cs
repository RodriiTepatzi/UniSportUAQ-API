using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using UniSportUAQ_API.Data.Base;
using UniSportUAQ_API.Data.Consts;
using UniSportUAQ_API.Data.Interfaces;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;
using UniSportUAQ_API.Data.DTO;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Org.BouncyCastle.Bcpg;
using System.Reflection.Metadata.Ecma335;

namespace UniSportUAQ_API.Controllers
{

    [ApiController]
    [Route("api/v1/courses")]
    public class CoursesController: Controller
    {
        private readonly ICoursesService _coursesService;
        private readonly IInscriptionsService _inscriptionsService;
		private readonly IUsersService _userService;
        private readonly IHorariosService _horariosService;
            
        public CoursesController(ICoursesService coursesService, IInscriptionsService inscriptionsService, IUsersService userService, IHorariosService horariosService)
        {
            _coursesService = coursesService;
            _inscriptionsService = inscriptionsService;
			_userService = userService;
            _horariosService = horariosService;
        }

		[HttpGet]
		[Route("id/{id}")]
		[Authorize]
		public async Task<IActionResult> GetCourseById(string id)
		{
			if (string.IsNullOrWhiteSpace(id)) return BadRequest(new BaseResponse<CourseDTO> { Error = ResponseErrors.AttributeIdInvalidlFormat });

			var result = await _coursesService.GetByIdAsync(id, c => c.Instructor!);

			if (result != null) {

				var response = new CourseDTO
				{

					Id = result.Id,
					CourseName = result.CourseName,
					InstructorName = result.Instructor!.FullName,
					InstructorPicture = result.Instructor!.PictureUrl,
					InstructorId = result.InstructorId,
					Day = result.Day,
					MaxUsers = result.MaxUsers,
					CurrentUsers = result.CurrentUsers,
                    Horarios = result.Horarios,
					StartHour = result.StartHour,
					EndHour = result.EndHour,
					Description = result.Description,
					Link = result.Link,
					Location = result.Location,
					IsVirtual = result.VirtualOrHybrid,
					CoursePictureUrl = result.CoursePictureUrl,

				};

				return Ok(new BaseResponse<CourseDTO> { Data = response});
			}

            return NotFound(new BaseResponse<CourseDTO> { Data = null });

        }

        [HttpGet]
        [Route("all")]
        [Authorize]
        public async Task<IActionResult> GetAllCourses()
        {
            var result = await _coursesService.GetAllAsync(c => c.IsActive == true, c => c.Instructor!);

            if (result != null) { 

                var data = new List<CourseDTO>();

                foreach (var item in result)
                {

                    var course = new CourseDTO
                    {

                        Id = item.Id,
                        CourseName = item.CourseName,
                        InstructorName = item.Instructor!.FullName,
                        InstructorPicture = item.Instructor!.PictureUrl,
                        InstructorId = item.InstructorId,
                        Day = item.Day,
                        MaxUsers = item.MaxUsers,
                        CurrentUsers = item.CurrentUsers,
                        Horarios = item.Horarios,
                        StartHour = item.StartHour,
                        EndHour = item.EndHour,
                        Description = item.Description,
                        Link = item.Link,
                        Location = item.Location,
                        IsVirtual = item.VirtualOrHybrid,
                        CoursePictureUrl = item.CoursePictureUrl,

                    };

                    data.Add(course);
                }

                return Ok(new BaseResponse<List<CourseDTO>> { Data = data });
            }

            return NotFound(new BaseResponse<List<CourseDTO>> { Data = null });
        }

        [HttpGet]
        [Route("all/inactive")]
        [Authorize]
        public async Task<IActionResult> GetAllInactiveCourses()
        {
            var result = await _coursesService.GetAllAsync(c => c.IsActive == false, c => c.Instructor!);

            if (result.Count() < 1) return NotFound(new BaseResponse<List<CourseDTO>> { Data = null });

            var data = new List<CourseDTO>();

			foreach (var item in result) 
			{

                var course = new CourseDTO
                {

                    Id = item.Id,
                    CourseName = item.CourseName,
                    InstructorName = item.Instructor!.FullName,
                    InstructorPicture = item.Instructor!.PictureUrl,
                    InstructorId = item.InstructorId,
                    Day = item.Day,
                    MaxUsers = item.MaxUsers,
                    CurrentUsers = item.CurrentUsers,
                    Horarios = item.Horarios,
                    StartHour = item.StartHour,
                    EndHour = item.EndHour,
                    Description = item.Description,
                    Link = item.Link,
                    Location = item.Location,
                    IsVirtual = item.VirtualOrHybrid,
                    CoursePictureUrl = item.CoursePictureUrl,

                };

                data.Add(course);

            }

			return Ok(new BaseResponse<List<CourseDTO>> { Data = data });

			
		}

        [HttpGet]
        [Route("all/range/{start}/{end}")]
        [Authorize]
        public async Task<IActionResult> GetAllCourses(int start, int end)
        {
            if (start < 0 || end < start) return BadRequest(new BaseResponse<List<UserDTO>> { Data = null, Error = ResponseErrors.FilterStartEndContradiction });

            var result = await _coursesService.GetAllAsync(c => c.IsActive == true, c => c.Instructor!);

            if (result.Count() < 1) return NotFound(new BaseResponse<List<UserDTO>> { Data = null});

            var courseInRange = result.Skip(start).Take(end - start + 1).ToList();

            var data = new List<CourseDTO>();

            foreach (var item in courseInRange)
            {

                var course = new CourseDTO
                {

                    Id = item.Id,
                    CourseName = item.CourseName,
                    InstructorName = item.Instructor!.FullName,
                    InstructorPicture = item.Instructor!.PictureUrl,
                    InstructorId = item.InstructorId,
                    Day = item.Day,
                    MaxUsers = item.MaxUsers,
                    CurrentUsers = item.CurrentUsers,
                    Horarios = item.Horarios,
                    StartHour = item.StartHour,
                    EndHour = item.EndHour,
                    Description = item.Description,
                    Link = item.Link,
                    Location = item.Location,
                    IsVirtual = item.VirtualOrHybrid,
                    CoursePictureUrl = item.CoursePictureUrl,

                };

                data.Add(course);
            }

            return Ok(new BaseResponse<List<CourseDTO>> { Data = data });
        }

        [HttpGet]
        [Route("instructorid/{instructorid}")]
        [Authorize]
        public async Task<IActionResult> GetCoursesByInstructorId(string instructorid) 
        {

            if (string.IsNullOrEmpty(instructorid)) return BadRequest(new BaseResponse<List<CourseDTO>> { Error = ResponseErrors.AttributeIdInvalidlFormat });

            var result = await _coursesService.GetAllAsync(c => c.InstructorId == instructorid, c => c.Instructor!);

			if(!result.Any()) return NotFound(new BaseResponse<List<CourseDTO>> { Data= null });

            var data = new List<CourseDTO>();

			foreach (var item in result)
			{

                var course = new CourseDTO
                {

                    Id = item.Id,
                    CourseName = item.CourseName,
                    InstructorName = item.Instructor!.FullName,
                    InstructorPicture = item.Instructor!.PictureUrl,
                    InstructorId = item.InstructorId,
                    Day = item.Day,
                    MaxUsers = item.MaxUsers,
                    CurrentUsers = item.CurrentUsers,
                    Horarios = item.Horarios,
                    StartHour = item.StartHour,
                    EndHour = item.EndHour,
                    Description = item.Description,
                    Link = item.Link,
                    Location = item.Location,
                    IsVirtual = item.VirtualOrHybrid,
                    CoursePictureUrl = item.CoursePictureUrl,

                };

                data.Add(course);

            }

            return Ok(new BaseResponse<List<CourseDTO>> { Data = data});
        }

		[HttpGet]
		[Route("instructorid/{instructorid}/active")]
		[Authorize]
		public async Task<IActionResult> GetActiveCoursesByInstructorId(string instructorid)
		{

			if (string.IsNullOrEmpty(instructorid)) return BadRequest(new BaseResponse<List<CourseDTO>> { Error = ResponseErrors.AttributeIdInvalidlFormat });

            var result = await _coursesService.GetAllAsync(c => c.InstructorId == instructorid && c.IsActive == true, c => c.Instructor!);

			if(!result.Any()) return NotFound(new BaseResponse<List<CourseDTO>> { Error = ResponseErrors.EntityNotExist });

            var data = new List<CourseDTO>();

            foreach (var item in result)
            {

                var course = new CourseDTO
                {

                    Id = item.Id,
                    CourseName = item.CourseName,
                    InstructorName = item.Instructor!.FullName,
                    InstructorPicture = item.Instructor!.PictureUrl,
                    InstructorId = item.InstructorId,
                    Day = item.Day,
                    MaxUsers = item.MaxUsers,
                    CurrentUsers = item.CurrentUsers,
                    Horarios = item.Horarios,
                    StartHour = item.StartHour,
                    EndHour = item.EndHour,
                    Description = item.Description,
                    Link = item.Link,
                    Location = item.Location,
                    IsVirtual = item.VirtualOrHybrid,
                    CoursePictureUrl = item.CoursePictureUrl,

                };

                data.Add(course);

            }

            return Ok(new BaseResponse<List<CourseDTO>> { Data = data});

			
		}

		[HttpGet]
		[Route("instructorid/{instructorid}/inactive")]
		[Authorize]
		public async Task<IActionResult> GetInactiveCoursesByInstructorId(string instructorid)
		{

			if (string.IsNullOrEmpty(instructorid)) return BadRequest(new BaseResponse<List<CourseDTO>> { Error = ResponseErrors.AttributeIdInvalidlFormat });

            var result = await _coursesService.GetAllAsync(c => c.InstructorId == instructorid && c.IsActive == false, c => c.Instructor!);

			var data = new List<CourseDTO>();

			foreach (var item in result)
            {
                var course = new CourseDTO
                {

                    Id = item.Id,
                    CourseName = item.CourseName,
                    InstructorName = item.Instructor!.FullName,
                    InstructorPicture = item.Instructor!.PictureUrl,
                    InstructorId = item.InstructorId,
                    Day = item.Day,
                    MaxUsers = item.MaxUsers,
                    CurrentUsers = item.CurrentUsers,
                    Horarios = item.Horarios,
                    StartHour = item.StartHour,
                    EndHour = item.EndHour,
                    Description = item.Description,
                    Link = item.Link,
                    Location = item.Location,
                    IsVirtual = item.VirtualOrHybrid,
                    CoursePictureUrl = item.CoursePictureUrl,

                };

                data.Add(course);
            }

			if (result.Any()) return Ok(new BaseResponse<List<CourseDTO>> { Data = data });

            return NotFound(new BaseResponse<List<CourseDTO>> { Error = ResponseErrors.EntityNotExist});
		}


        [HttpGet]
        [Route("search/{searchTerm}")]
        [Authorize]
        public async Task<IActionResult> GetCoursesSearch(string searchTerm)
        {
			if(string.IsNullOrWhiteSpace(searchTerm)) return BadRequest(new BaseResponse<List<CourseDTO>> { Data = null, Error = ResponseErrors.FilterInvalidSearchTerm});

			var result = await _coursesService.GetAllAsync(i =>
				((i.CourseName != null && i.CourseName.ToLower().Contains(searchTerm)) ||
					(i.Description != null && i.Description.ToLower().Contains(searchTerm)) ||
					(i.Day != null && i.Day.ToLower().Contains(searchTerm))
				)
				&&
				i.IsActive == true, c => c.Instructor!
            );

			var distinctResult = result.Distinct();

			var data = new List<CourseDTO>();

			foreach (var item in distinctResult) {

                var course = new CourseDTO
                {

                    Id = item.Id,
                    CourseName = item.CourseName,
                    InstructorName = item.Instructor!.FullName,
                    InstructorPicture = item.Instructor!.PictureUrl,
                    InstructorId = item.InstructorId,
                    Day = item.Day,
                    MaxUsers = item.MaxUsers,
                    CurrentUsers = item.CurrentUsers,
                    Horarios = item.Horarios,
                    StartHour = item.StartHour,
                    EndHour = item.EndHour,
                    Description = item.Description,
                    Link = item.Link,
                    Location = item.Location,
                    IsVirtual = item.VirtualOrHybrid,
                    CoursePictureUrl = item.CoursePictureUrl,

                };

                data.Add(course);
            }

			if (distinctResult.Any()) return Ok(new BaseResponse<List<CourseDTO>> { Data = data });

            return NotFound(new BaseResponse<List<CourseDTO>> { Error = ResponseErrors.EntityNotExist});
        }


        [HttpPut]
		[Route("update")]
		[Authorize]
		public async Task<IActionResult> UpdateCourse([FromBody] CourseSchema courseSchema)
		{
            if (courseSchema.CourseName is null) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeEmptyOrNull });
            if (courseSchema.Day is null) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeEmptyOrNull });
            if (courseSchema.StartHour is null) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeEmptyOrNull });
            if (courseSchema.EndHour is null) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeEmptyOrNull });
            if (courseSchema.Day is null) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeEmptyOrNull });
            if (courseSchema.InstructorId is null) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeEmptyOrNull });
            if (courseSchema.MaxUsers <= 0) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeEmptyOrNull });

            var course = new Course
			{
				Id = courseSchema.Id,
				CourseName = courseSchema.CourseName,
				Day = courseSchema.Day,
				StartHour = courseSchema.StartHour,
				EndHour = courseSchema.EndHour,
				MaxUsers = courseSchema.MaxUsers,
				Description = courseSchema.Description,
            };

			var result = await _coursesService.UpdateAsync(course);

			if (result is not null) return Ok(new BaseResponse<bool> { Data = true});

			return BadRequest(new BaseResponse<bool> { Data = false, Error = ResponseErrors.ServerDataBaseErrorUpdating});
		}


        


        [HttpPost]
		[Route("create")]
		[Authorize]
		public async Task<IActionResult> AddToCourse([FromBody] CourseSchema courseSchema)
		{
			if(courseSchema.CourseName is null) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeEmptyOrNull });
            if (courseSchema.Day is null) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeEmptyOrNull });

            if(courseSchema.Horarios!.Count() < 1) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeEmptyOrNull });

            if (courseSchema.Day is null)  return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeEmptyOrNull });
            if (courseSchema.InstructorId is null) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeEmptyOrNull });

            if ((await _userService.GetAllAsync(i => i.Id == courseSchema.InstructorId && i.IsInstructor == true)).Count() < 1) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeIsInstructorFalse });

            if (courseSchema.MaxUsers <= 0) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeEmptyOrNull });            

            var courses = await _coursesService.GetAllAsync(c => c.InstructorId == courseSchema.InstructorId);
			
			if(courses is not null ) {

                foreach (var course in courses)
                {
                    if (IsScheduleConflict(course.Horarios!, courseSchema.Horarios!)) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.CourseInstructorHindered });
                    
                }
            }


			var NewCourse = new Course
			{
				Id = Guid.NewGuid().ToString(),
                SubjectId = courseSchema.SubjectId,
                CourseName = courseSchema.CourseName,
                InstructorId = courseSchema.InstructorId,
				Day = courseSchema.Day,
				StartHour = courseSchema.StartHour,
                EndHour = courseSchema.EndHour,
                MaxUsers = courseSchema.MaxUsers,
                CurrentUsers = 0,
				Description = courseSchema.Description,
                IsActive = true,
				Location = courseSchema.location,
				
			};
            var result = await _coursesService.AddAsync(NewCourse);

            if(result != null) { 

                if(IsHorarioConflict(courseSchema.Horarios!)) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.CourseHorarioConfict });

                foreach (var horario in courseSchema.Horarios!)
                {
                    

                    var NewHorario = new Horario
                    {
                        Id = Guid.NewGuid().ToString(),
                        Day = horario.Day,
                        StartHour = horario.StartHour,
                        EndHour = horario.EndHour,
                        CourseId = NewCourse.Id
                    };

                    //create horario
                    var newHorario = await _horariosService.AddAsync(NewHorario);
                    if (newHorario == null)  return BadRequest(new BaseResponse<object> {Data = NewHorario, Error = ResponseErrors.ServerDataBaseErrorUpdating }); 
                }
                return Ok((new BaseResponse<bool> { Data = true }));
            }
           
			return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.ServerDataBaseErrorUpdating });
        }

		[HttpPut]
		[Route("endcourse")]
		[Authorize]
		public async Task<IActionResult> EndCourse([FromBody] CourseSchema course) {

			if(string.IsNullOrEmpty(course.Id)) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeIdInvalidlFormat });
            var result = await _coursesService.GetByIdAsync(course.Id);

			if (result is not null) {

				var inscriptions = await _inscriptionsService.GetAllAsync(i => i.CourseId == course.Id);

				if (inscriptions.Count() < 1) return NotFound(new BaseResponse<bool> { Data = false, Error = ResponseErrors.EntityNotExist});


                var endCourse = await _coursesService.GetByIdAsync(course.Id);

                if (endCourse != null)
                {
                    endCourse.IsActive = false;

                    var endedResult = await _coursesService.UpdateAsync(endCourse);

                    if (endedResult != null) return BadRequest(new BaseResponse<bool> { Data = false, Error = ResponseErrors.CourseCanNotEnd});
                }

                foreach (var inscription in inscriptions)
				{

					inscription.IsFinished = true;
					
					if (inscription.Accredit == true) inscription.Grade = 10;
					if (inscription.Accredit == false) inscription.Grade = 5;

					var endedInscription =await _inscriptionsService.UpdateAsync(inscription);

					if(endedInscription == null) return BadRequest(new BaseResponse<bool> { Data = false, Error = ResponseErrors.InscriptionNotEnded });
                }

                

                return Ok(new BaseResponse<bool> { Data = true}); 
			
			}

            return NotFound(new BaseResponse<bool> { Data = false});
        }
        /*******************************horarios*******************************/




        /*******************************inscriptions*******************************/

        [HttpPost]
        [Route("inscription/{courseId}/{studentId}")]
        [Authorize]
        public async Task<IActionResult> AddToCourse(string courseId, string studentId)
        {
			// First we have to check if the courseId and studentId, both exist on our database. Otherwise we shall return an error.

			if (await _userService.GetByIdAsync(studentId) is null) return NotFound(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.EntityNotExist});
			if (await _coursesService.GetByIdAsync(courseId) is null) return NotFound(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.EntityNotExist});

			
            var existingInscription = await _inscriptionsService.GetAllAsync(i => i.StudentId == studentId);

			foreach (var item in existingInscription) { 

				if(item.Course!.IsActive) return BadRequest(new BaseResponse<bool> { Data = false, Error = ResponseErrors.InscriptionStudentAlredyInscripted });

            }

            //if (existingInscription.Any()) return BadRequest(new BaseResponse<bool> { Data = false, Error= ResponseErrors.InscriptionAlreadyExist });


			//
            var checkIfInCourse = await _inscriptionsService.GetAllAsync(i => i.CourseId == courseId && i.StudentId == studentId);

			if (checkIfInCourse.Any()) return BadRequest(new BaseResponse<bool> { Data = false, Error = ResponseErrors.InscriptionAlreadyExist});

            var course = await _coursesService.GetByIdAsync(courseId);

			if (course == null) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            var entity = new Inscription
			{
				DateInscription = DateTime.Now,
				Accredit = false,
				CourseId = courseId,
				StudentId = studentId,
			};

			if (course.CurrentUsers >= course.MaxUsers) return Ok(new BaseResponse<bool> { Data = false, Error = ResponseErrors.CourseExceedMaxUsers });

            var inscriptionResult = await _inscriptionsService.AddAsync(entity);

            if (inscriptionResult == null) return NotFound(new BaseResponse<bool> { Data = false, Error = ResponseErrors.ServerDataBaseErrorUpdating });

			course.CurrentUsers++;

            await _coursesService.UpdateAsync(course);

            return Ok(new BaseResponse<bool> { Data = true });
        }

        

		[HttpGet]
		[Route("inscription/check/{courseId}/{studentId}")]
		[Authorize]
		public async Task<IActionResult> CheckIfInCourse(string courseId, string studentId)
		{
			// First we have to check if the courseId and studentId, both exist on our database. Otherwise we shall return an error.

			if (await _userService.GetByIdAsync(studentId) is null) return NotFound(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.EntityNotExist});
			if (await _coursesService.GetByIdAsync(courseId) is null) return NotFound(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.EntityNotExist});

			var checkIfInCourse = await _inscriptionsService.GetAllAsync(i => i.CourseId == courseId && i.StudentId == studentId);

			if (checkIfInCourse.Any()) return Ok(new BaseResponse<bool> { Data = true });

			return NotFound(new BaseResponse<bool> { Data = false, Error = ResponseErrors.EntityNotExist });
		}

		[HttpGet]
		[Route("inscription/count/{studentId}")]
		[Authorize]
		public async Task<IActionResult> GetInscriptionCoursesByUserId(string studentId)
		{
			// First we have to check if the courseId and studentId, both exist on our database. Otherwise we shall return an error.

			if (await _userService.GetByIdAsync(studentId) is null) return NotFound(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.EntityNotExist});

			var result = await _inscriptionsService.GetAllAsync(i => i.StudentId == studentId);

			var count = result.Count();

			return Ok(new DataResponse { Data = count, ErrorMessage = null });
		}

		[HttpGet]
		[Route("inscription/enrolled/{studentId}")]
		[Authorize]
		public async Task<IActionResult> GetEnrolledCoursesByUserId(string studentId)
		{
			// First we have to check if the courseId and studentId, both exist on our database. Otherwise we shall return an error.

			if (await _userService.GetByIdAsync(studentId) is null) return NotFound(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.EntityNotExist});

			var result = await _inscriptionsService.GetAllAsync(i => i.StudentId == studentId,
				i => i.Student!,
				i => i.Course!,
				i => i.Course!.Instructor!
			);

			var data = new List<InscriptionDTO>();

			foreach (var item in result) {

				var inscription = new InscriptionDTO
				{
					Id = item.Id,
					CourseId = item.CourseId,
					CourseName = item.Course!.CourseName,
					CourseDescription = item.Course!.Description

				};

				data.Add(inscription); 
			}

            if (result.Any()) return Ok(new BaseResponse<List<InscriptionDTO>> { Data = data });

            return NotFound(new BaseResponse<InscriptionDTO> { Data = null });
		}

		[HttpGet]
		[Route("inscription/finished/{studentId}")]
		[Authorize]
		public async Task<IActionResult> GetFinishedCoursesById(string studentId)
		{
			// First we have to check if the courseId and studentId, both exist on our database. Otherwise we shall return an error.

			if (await _userService.GetByIdAsync(studentId) is null) return NotFound(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.EntityNotExist});

			var result = await _inscriptionsService.GetAllAsync(i => i.StudentId == studentId && i.IsFinished == false,
				i => i.Student!,
				i => i.Course!,
				i => i.Course!.Instructor!
			);

            var data = new List<InscriptionDTO>();

            foreach (var item in result)
            {

                var inscription = new InscriptionDTO
                {
                    Id = item.Id,
                    CourseId = item.CourseId,
                    CourseName = item.Course!.CourseName,
                    CourseDescription = item.Course!.Description,
					Accredit = item.Accredit

                };

                data.Add(inscription);
            }

			if (result.Any()) return Ok(new BaseResponse<List<InscriptionDTO>> { Data = data});

				

			return NotFound(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.EntityNotExist});
		}

		[HttpGet]
		[Route("inscription/getbycourse/{courseId}")]
		[Authorize]

		public async Task<IActionResult> GetInscriptionsByCourseAsync(string courseId) { 

			if(!Guid.TryParse(courseId, out _)) return BadRequest(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.AttributeIdInvalidlFormat });

            var course = await _coursesService.GetByIdAsync(courseId);

			if (course == null) return NotFound(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.EntityNotExist});

			var result = await _inscriptionsService.GetAllAsync(i => i.CourseId == courseId,
				i => i.Student!,
				i => i.Course!,
				i => i.Course!.Instructor!
			);

			if (!result.Any()) return NotFound(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.CourseNoneInscription });

            var data = new List<InscriptionDTO>();

            foreach (var item in result)
            {

                var inscription = new InscriptionDTO
                {
                    Id = item.Id,
                    CourseId = item.CourseId,
                    CourseName = item.Course!.CourseName,
                    CourseDescription = item.Course!.Description,
					Accredit = item.Accredit

                };

                data.Add(inscription);
            }

            return Ok(new BaseResponse<List<InscriptionDTO>> { Data = data });


        }


        [HttpPut]
		[Route("inscription/acredit")]
		[Authorize]

		public async Task<IActionResult> AcreditCourse([FromBody] Inscription inscription) {

			if (!Guid.TryParse(inscription.Id, out _)) return BadRequest(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.AttributeIdInvalidlFormat });
            if (!Guid.TryParse(inscription.CourseId, out _)) return BadRequest(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.AttributeIdInvalidlFormat });
            if (!Guid.TryParse(inscription.StudentId, out _)) return BadRequest(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.AttributeIdInvalidlFormat });


            var inscriptionResult = await _inscriptionsService.GetAllAsync(i => i.CourseId == inscription.CourseId && i.StudentId == inscription.StudentId,
				i => i.Student!,
				i => i.Course!,
				i => i.Course!.Instructor!
			);

			var inscriptionEntity = inscriptionResult.FirstOrDefault();

			if (inscriptionEntity != null)
			{
				inscriptionEntity.Accredit = true;

				var result = await _inscriptionsService.UpdateAsync(inscriptionEntity);

				if (result == null) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.ServerDataBaseError });

        
				return Ok(new BaseResponse<bool> { Data = true });

			}

			return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.InscriptionNotAecredit});

		}




		[HttpDelete]
		[Route("inscription/remove/{courseId}/{studentId}")]
		[Authorize]
		public async Task<IActionResult> RemoveFromCourse(string courseId, string studentId)
		{
			// First we have to check if the courseId and studentId, both exist on our database. Otherwise we shall return an error.

			if (await _userService.GetByIdAsync(studentId) is null) return NotFound(new BaseResponse<bool> { Error = ResponseErrors.EntityNotExist});

			if (await _coursesService.GetByIdAsync(courseId) is null) return NotFound(new BaseResponse<bool> { Error = ResponseErrors.EntityNotExist});

			var checkIfInCourse = await _inscriptionsService.GetAllAsync(i => i.CourseId == courseId && i.StudentId == studentId);

			if (!checkIfInCourse.Any()) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.CourseNotFoundInscription});


			var query = await _inscriptionsService.GetAllAsync(i => i.CourseId == courseId && i.StudentId == studentId);
			var entity = query.FirstOrDefault();

			if (entity != null)
			{
				var wasRemoved = await _inscriptionsService.DeleteAsync(entity.Id!);

				if (wasRemoved)
				{
					var course = await _coursesService.GetByIdAsync(courseId);

					if (course is not null)
					{
						course.CurrentUsers--;
						// course.PendingUsers--; // TODO: check for this line in case to implement a way to reserve places in the course.

						await _coursesService.UpdateAsync(course);
					}

					return Ok(new BaseResponse<bool> { Data = true });
				} }

			return BadRequest(new BaseResponse<bool> {  Error = ResponseErrors.CourseErrorRemoving });
		}



		//local use

        private bool IsScheduleConflict(IEnumerable<Horario> existingCourse, List<HorarioSchema> newCourse)
        {
            if(existingCourse.Count() < 1 || newCourse.Count() < 1) return true;

            foreach(var existingHr in existingCourse)
            {

                foreach(var newHr in newCourse) 
                {
                    if (existingHr.Day == newHr.Day)
                    {

                        if (existingHr.StartHour < newHr.EndHour && newHr.StartHour < existingHr.EndHour)
                        {
                            return true; // Conflict in schedule
                        }
                    }

                }

            
            }

            return false; // No  Conflict in schedule
        }

        private bool IsHorarioConflict(List<HorarioSchema> horarios) {

            HorarioSchema lastHorario = null;

            foreach (var horario in horarios) {

                if (lastHorario != null) {

                    if (horario.Day == lastHorario.Day) {

                        if (horario.StartHour < lastHorario.EndHour && horario.EndHour > lastHorario.StartHour) return true;

                    }
                    
                }

                lastHorario = horario;

            }


            return false;
        }



    }
}
