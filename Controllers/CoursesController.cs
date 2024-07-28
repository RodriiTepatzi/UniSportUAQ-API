using IronPython.Runtime.Operations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using UniSportUAQ_API.Data.Consts;
using UniSportUAQ_API.Data.Interfaces;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;

namespace UniSportUAQ_API.Controllers
{

    [ApiController]
    [Route("api/courses")]
    public class CoursesController: Controller
    {
        private readonly ICoursesService _coursesService;
        private readonly IInscriptionsService _inscriptionsService;
		private readonly IStudentsService _studentsService;
            
        public CoursesController(ICoursesService coursesService, IInscriptionsService inscriptionsService, IStudentsService studentsService)
        {
            _coursesService = coursesService;
            _inscriptionsService = inscriptionsService;
			_studentsService = studentsService;
        }

        [HttpGet]
        [Route("id/{id}")]
        [Authorize]
        public async Task<IActionResult> GetCourseById(string Id)
        {
            if (!Guid.TryParse(Id, out _)) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _coursesService.GetCourseByIdAsync(Id);

            if (result is not null) return Ok(new DataResponse { Data = result.Dictionary, ErrorMessage = null });

            return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND});
        }

		[HttpGet]
		[Route("all")]
		[Authorize]
		public async Task<IActionResult> GetAllCourses()
		{
			var result = await _coursesService.GetAllCoursesAsync();

			var data = new List<Dictionary<string, object>>();

			foreach (var item in result) data.Add(item.Dictionary);

			if (result.Count > 0) return Ok(new DataResponse { Data = data, ErrorMessage = null });

			return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
		}

		[HttpGet]
		[Route("all/inactive")]
		[Authorize]
		public async Task<IActionResult> GetAllInactiveCourses()
		{
			var result = await _coursesService.GetAllInactiveCoursesAsync();

			var data = new List<Dictionary<string, object>>();

			foreach (var item in result) data.Add(item.Dictionary);

			if (result.Count > 0) return Ok(new DataResponse { Data = data, ErrorMessage = null });

			return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
		}

		[HttpGet]
        [Route("instructorid/{instructorid}")]
        [Authorize]
        public async Task<IActionResult> GetCoursesByInstructorId(string instructorid) 
        {

            if (!Guid.TryParse(instructorid, out _)) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _coursesService.GetCoursesByIdInstructor(instructorid);

			var data = new List<Dictionary<string, object>>();

            foreach (var item in result) data.Add(item.Dictionary);

            if (result.Count > 0) return Ok(new DataResponse { Data = data, ErrorMessage = null });

			return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
        }

		[HttpGet]
		[Route("instructorid/{instructorid}/active")]
		[Authorize]
		public async Task<IActionResult> GetActiveCoursesByInstructorId(string instructorid)
		{

			if (!Guid.TryParse(instructorid, out _)) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

			var result = await _coursesService.GetActivesCoursesByIdInstructor(instructorid);

			var data = new List<Dictionary<string, object>>();

			foreach (var item in result) data.Add(item.Dictionary);

			if (result.Count > 0) return Ok(new DataResponse { Data = data, ErrorMessage = null });

			return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
		}

		[HttpGet]
		[Route("instructorid/{instructorid}/inactive")]
		[Authorize]
		public async Task<IActionResult> GetInactiveCoursesByInstructorId(string instructorid)
		{

			if (!Guid.TryParse(instructorid, out _)) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

			var result = await _coursesService.GetInactivesCoursesByIdInstructor(instructorid);

			var data = new List<Dictionary<string, object>>();

			foreach (var item in result) data.Add(item.Dictionary);

			if (result.Count > 0) return Ok(new DataResponse { Data = data, ErrorMessage = null });

			return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
		}


        [HttpGet]
        [Route("search/{searchTerm}")]
        [Authorize]
        public async Task<IActionResult> GetCoursesSearch(string searchTerm)
        {
			if(searchTerm is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _coursesService.GetCoursesSearch(searchTerm);

			var data = new List<Dictionary<string, object>>();

			foreach (var item in result) data.Add(item.Dictionary);

			if (result.Count > 0) return Ok(new DataResponse { Data = data, ErrorMessage = null });

            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND }); 

        }


        [HttpPut]
		[Route("update")]
		[Authorize]
		public async Task<IActionResult> UpdateCourse([FromBody] CourseSchema courseSchema)
		{
            if (courseSchema.CourseName is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (courseSchema.Day is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (courseSchema.StartHour is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (courseSchema.EndHour is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (courseSchema.Day is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (courseSchema.InstructorId is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (courseSchema.MaxUsers <= 0) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var course = new Course
			{
				Id = courseSchema.Id,
				CourseName = courseSchema.CourseName,
				Day = courseSchema.Day,
				StartHour = courseSchema.StartHour,
				EndHour = courseSchema.EndHour,
				MaxUsers = courseSchema.MaxUsers,
				Description = courseSchema.Description,
				CoursePictureUrl = courseSchema.CoursePictureUrl,
            };

			var result = await _coursesService.UpdateCourseAsync(course);

			if (result is not null) return Ok(new DataResponse { Data = result.Dictionary, ErrorMessage = null });

			return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.INTERNAL_ERROR });
		}


        


        [HttpPost]
		[Route("create")]
		[Authorize]
		public async Task<IActionResult> AddToCourse([FromBody] CourseSchema courseSchema)
		{
			if(courseSchema.CourseName is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
			if(courseSchema.Day is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            if(!DateTime.TryParse(courseSchema.StartHour, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if(!DateTime.TryParse(courseSchema.EndHour, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            if(courseSchema.Day is null) return BadRequest (new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
			if(courseSchema.InstructorId is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
			if(courseSchema.MaxUsers <= 0) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var courses = await _coursesService.GetCoursesByIdInstructor(courseSchema.InstructorId);

			
			
			if(courses is not null ) {

                foreach (var course in courses)
                {
                    if (IsScheduleConflict(course, courseSchema)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.INSTRUCTOR_HINDERED });
                    
                }
            }


            var NewCourse = new Course
			{
				CourseName = courseSchema.CourseName,
				Day = courseSchema.Day,
				StartHour = courseSchema.StartHour,
				EndHour = courseSchema.EndHour,
				InstructorId = courseSchema.InstructorId,
				IsActive = true,
				MaxUsers = courseSchema.MaxUsers
			};

			var result = await _coursesService.CreateCourseAsync(NewCourse);

			if(result is not null) return Ok(new DataResponse { Data = result.Dictionary, ErrorMessage = null });

			return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.INTERNAL_ERROR});
		}

		[HttpPut]
		[Route("endcourse")]
		[Authorize]
		public async Task<IActionResult> EndCourse([FromBody] CourseSchema course) {

			if(!Guid.TryParse(course.Id, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST});

			var result = await _coursesService.GetCourseByIdAsync(course.Id);

			if (result is not null) {

                var endInscriptions = await _inscriptionsService.EndInscriptionsByCourseIdAsync(course.Id);

                if (endInscriptions == false) return BadRequest(new DataResponse { Data = false, ErrorMessage = ResponseMessages.END_INSCRIPTIONS_ERROR });

                var endCourse = await _coursesService.EndCourseAsync(course.Id);

                if (endCourse == false) return BadRequest(new DataResponse { Data = false, ErrorMessage = ResponseMessages.COURSE_ENDED });

                return Ok(new DataResponse { Data = true, ErrorMessage = null }); 
			
			}

            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
        }
		

       

		[HttpPost]
        [Route("inscription/{courseId}/{studentId}")]
        [Authorize]
        public async Task<IActionResult> AddToCourse(string courseId, string studentId)
        {
			// First we have to check if the courseId and studentId, both exist on our database. Otherwise we shall return an error.

			if (await _studentsService.GetStudentByIdAsync(studentId) is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
			if (await _coursesService.GetCourseByIdAsync(courseId) is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

			var checkIfInCourse = await _inscriptionsService.CheckInscriptionByCourseIdAndStudentIdAsync(courseId, studentId);

            if (checkIfInCourse) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.ALREADY_IN_COURSE });

            var course = await _coursesService.GetCourseByIdAsync(courseId);

            if (course is not null)
            {
				if ((course.CurrentUsers + 1 + course.PendingUsers) > course.MaxUsers) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.EXCEEDED_MAX_USERS });
				
				course.CurrentUsers++;
                // course.PendingUsers--; // TODO: check for this line in case to implement a way to reserve places in the course.

                await _coursesService.UpdateCourseAsync(course);
            }

            var inscriptionResult = await _inscriptionsService.CreateInscriptionAsync(courseId, studentId);

            return Ok(new DataResponse { Data = inscriptionResult.ToDictionary(), ErrorMessage = null});
        }

		[HttpGet]
		[Route("inscription/check/{courseId}/{studentId}")]
		[Authorize]
		public async Task<IActionResult> CheckIfInCourse(string courseId, string studentId)
		{
			// First we have to check if the courseId and studentId, both exist on our database. Otherwise we shall return an error.

			if (await _studentsService.GetStudentByIdAsync(studentId) is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
			if (await _coursesService.GetCourseByIdAsync(courseId) is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

			var checkIfInCourse = await _inscriptionsService.CheckInscriptionByCourseIdAndStudentIdAsync(courseId, studentId);

			if (checkIfInCourse) return Ok(new DataResponse { Data = true, ErrorMessage = null });

			return Ok(new DataResponse { Data = false, ErrorMessage = null });
		}

		[HttpGet]
		[Route("inscription/count/{studentId}")]
		[Authorize]
		public async Task<IActionResult> GetInscriptionCoursesByUserId(string studentId)
		{
			// First we have to check if the courseId and studentId, both exist on our database. Otherwise we shall return an error.

			if (await _studentsService.GetStudentByIdAsync(studentId) is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

			var result = await _inscriptionsService.GetStudentCoursesCountAsync(studentId);

			return Ok(new DataResponse { Data = result, ErrorMessage = null });
		}

		[HttpGet]
		[Route("inscription/enrolled/{studentId}")]
		[Authorize]
		public async Task<IActionResult> GetEnrolledCoursesByUserId(string studentId)
		{
			// First we have to check if the courseId and studentId, both exist on our database. Otherwise we shall return an error.

			if (await _studentsService.GetStudentByIdAsync(studentId) is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

			var result = await _inscriptionsService.GetInscriptionsByStudentAsync(studentId);


			var data = new List<Dictionary<string, object>>();

			foreach (var item in result) data.Add(item.ToDictionary());

			if (result.Count > 0) return Ok(new DataResponse { Data = data, ErrorMessage = null });

			return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
		}

		[HttpGet]
		[Route("inscription/finished/{studentId}")]
		[Authorize]
		public async Task<IActionResult> GetFinishedCoursesById(string studentId)
		{
			// First we have to check if the courseId and studentId, both exist on our database. Otherwise we shall return an error.

			if (await _studentsService.GetStudentByIdAsync(studentId) is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

			var result = await _inscriptionsService.GetFinishedInscriptionsByStudentAsync(studentId);

			var data = new List<Dictionary<string, object>>();

			foreach (var item in result) data.Add(item.ToDictionary());

			if (result.Count > 0) return Ok(new DataResponse { Data = data, ErrorMessage = null });

			return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
		}

		[HttpGet]
		[Route("inscription/getbycourse/{courseId}")]
		[Authorize]

		public async Task<IActionResult> GetInscriptionsByCourseAsync(string courseId) { 

			if(!Guid.TryParse(courseId, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
			
			var course = await _coursesService.GetCourseByIdAsync(courseId);

			if (course == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

			var result = await _inscriptionsService.GetInscriptionsByCourseAsync(courseId);

			if (result is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.NONE_INSCRIPTION_COURSE });

			var data = new List<Dictionary<string, object>>();

			foreach (var item in result) data.Add(item.ToDictionary());

            return Ok(new DataResponse { Data = data, ErrorMessage = null });


        }


        [HttpPut]
		[Route("inscription/acredit")]
		[Authorize]

		public async Task<IActionResult> AcreditCourse([FromBody] Inscription inscription) {

			if (!Guid.TryParse(inscription.Id, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (!Guid.TryParse(inscription.CourseId, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (!Guid.TryParse(inscription.StudentId, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

			var result = await _inscriptionsService.AcreditIsncriptionAsync(inscription.CourseId, inscription.StudentId);

			if (result) return Ok(new DataResponse { Data = result, ErrorMessage = null });

            return Ok(new DataResponse { Data = result, ErrorMessage = null });
		}




		[HttpDelete]
		[Route("inscription/remove/{courseId}/{studentId}")]
		[Authorize]
		public async Task<IActionResult> RemoveFromCourse(string courseId, string studentId)
		{
			// First we have to check if the courseId and studentId, both exist on our database. Otherwise we shall return an error.

			if (await _studentsService.GetStudentByIdAsync(studentId) is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

			if (await _coursesService.GetCourseByIdAsync(courseId) is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

			var checkIfInCourse = await _inscriptionsService.CheckInscriptionByCourseIdAndStudentIdAsync(courseId, studentId);

			if (!checkIfInCourse) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.NOT_FOUND_IN_COURSE });


			var wasRemoved = await _inscriptionsService.RemoveInscriptionByCourseIdAndStudentIdAsync(courseId, studentId);

			if (wasRemoved) 
			{
				var course = await _coursesService.GetCourseByIdAsync(courseId);

				if (course is not null)
				{
					course.CurrentUsers--;
					// course.PendingUsers--; // TODO: check for this line in case to implement a way to reserve places in the course.

					await _coursesService.UpdateCourseAsync(course);
				}

				return Ok(new DataResponse { Data = true, ErrorMessage = null }); 
			}

			return BadRequest(new DataResponse { Data = false, ErrorMessage = ResponseMessages.ERROR_REMOVING_USER_FROM_COURSE });
		}



		//local use

        private bool IsScheduleConflict(Course existingCourse, CourseSchema newCourse)
        {
            if (existingCourse.Day == newCourse.Day)
            {
                DateTime existingStartHour = DateTime.Parse(existingCourse.StartHour!);
                DateTime existingEndHour = DateTime.Parse(existingCourse.EndHour!);

                DateTime newStartHour = DateTime.Parse(newCourse.StartHour!);
                DateTime newEndHour = DateTime.Parse(newCourse.EndHour!);

                if (existingStartHour < newEndHour && newStartHour < existingEndHour)
                {
                    return true; // Conflict in schedule
                }
            }

            return false; // No  Conflict in schedule
        }

		

    }
}
