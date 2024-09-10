﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using UniSportUAQ_API.Data.Base;
using UniSportUAQ_API.Data.Consts;
using UniSportUAQ_API.Data.Interfaces;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;
using UniSportUAQ_API.Data.DTO;

namespace UniSportUAQ_API.Controllers
{

    [ApiController]
    [Route("api/v1/courses")]
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
		public async Task<IActionResult> GetCourseById(string id)
		{
			if (!Guid.TryParse(id, out _)) return BadRequest(new BaseResponse<CourseDTO> { Error = ResponseErrors.AuthInvalidData });

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

            return BadRequest(new BaseResponse<Course> { Error = ResponseErrors.AuthInvalidData });

        }

		[HttpGet]
		[Route("all")]
		[Authorize]
		public async Task<IActionResult> GetAllCourses()
		{
			var result = await _coursesService.GetAllAsync(c => c.IsActive == true, c => c.Instructor!);

			if (result.Count() < 1) return BadRequest(new BaseResponse<Course> { Error = ResponseErrors.AuthUserEmailAlreadyExists });


            var data = new List<CourseDTO>();


			foreach (var item in result) { 

				var course = new CourseDTO {

                    Id = item.Id,
                    CourseName = item.CourseName,
                    InstructorName = item.Instructor!.FullName,
                    InstructorPicture = item.Instructor!.PictureUrl,
                    InstructorId = item.InstructorId,
                    Day = item.Day,
                    MaxUsers = item.MaxUsers,
                    CurrentUsers = item.CurrentUsers,
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

			if (result.Any()) return Ok(new DataResponse { Data = data, ErrorMessage = null });

			return NotFound(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
		}

		[HttpGet]
		[Route("all/inactive")]
		[Authorize]
		public async Task<IActionResult> GetAllInactiveCourses()
		{
			var result = await _coursesService.GetAllAsync(c => c.IsActive == false, c => c.Instructor!);

			if (result.Count() < 1) return NotFound(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

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

			return Ok(new DataResponse { Data = data, ErrorMessage = null });

			
		}

		[HttpGet]
        [Route("instructorid/{instructorid}")]
        [Authorize]
        public async Task<IActionResult> GetCoursesByInstructorId(string instructorid) 
        {

            if (!Guid.TryParse(instructorid, out _)) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _coursesService.GetAllAsync(c => c.InstructorId == instructorid, c => c.Instructor!);

			if(!result.Any()) return NotFound(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

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

            return Ok(new DataResponse { Data = data});
        }

		[HttpGet]
		[Route("instructorid/{instructorid}/active")]
		[Authorize]
		public async Task<IActionResult> GetActiveCoursesByInstructorId(string instructorid)
		{

			if (!Guid.TryParse(instructorid, out _)) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

			var result = await _coursesService.GetAllAsync(c => c.InstructorId == instructorid && c.IsActive == true, c => c.Instructor!);

			if(!result.Any()) return NotFound(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

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

            return Ok(new DataResponse { Data = data, ErrorMessage = null });

			
		}

		[HttpGet]
		[Route("instructorid/{instructorid}/inactive")]
		[Authorize]
		public async Task<IActionResult> GetInactiveCoursesByInstructorId(string instructorid)
		{

			if (!Guid.TryParse(instructorid, out _)) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

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

			if (result.Any()) return Ok(new DataResponse { Data = data, ErrorMessage = null });

			return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
		}


        [HttpGet]
        [Route("search/{searchTerm}")]
        [Authorize]
        public async Task<IActionResult> GetCoursesSearch(string searchTerm)
        {
			if(string.IsNullOrWhiteSpace(searchTerm)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

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

			if (distinctResult.Any()) return Ok(new DataResponse { Data = data, ErrorMessage = null });

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
            };

			var result = await _coursesService.UpdateAsync(course);

			if (result is not null) return Ok();

			return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.INTERNAL_ERROR });
		}


        


        [HttpPost]
		[Route("create")]
		[Authorize]
		public async Task<IActionResult> AddToCourse([FromBody] CourseSchema courseSchema)
		{
			if(courseSchema.CourseName is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = "course name: "+ResponseMessages.BAD_REQUEST });
			if(courseSchema.Day is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            if(!DateTime.TryParse(courseSchema.StartHour, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if(!DateTime.TryParse(courseSchema.EndHour, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            if(courseSchema.Day is null) return BadRequest (new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
			if(courseSchema.InstructorId is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
			if(courseSchema.MaxUsers <= 0) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });


            var courses = await _coursesService.GetAllAsync(c => c.InstructorId == courseSchema.InstructorId);
			
			if(courses is not null ) {

                foreach (var course in courses)
                {
                    if (IsScheduleConflict(course, courseSchema)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.INSTRUCTOR_HINDERED });
                    
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

			if(result != null) return Ok(new DataResponse { Data = result.Dictionary, ErrorMessage = null });

			return BadRequest(new DataResponse { Data = NewCourse.Dictionary, ErrorMessage = ResponseMessages.INTERNAL_ERROR});
		}

		[HttpPut]
		[Route("endcourse")]
		[Authorize]
		public async Task<IActionResult> EndCourse([FromBody] CourseSchema course) {

			if(!Guid.TryParse(course.Id, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST});

			var result = await _coursesService.GetByIdAsync(course.Id);

			if (result is not null) {

				var inscriptions = await _inscriptionsService.GetAllAsync(i => i.CourseId == course.Id);

				if (inscriptions.Count() < 1) return BadRequest(new DataResponse { Data = false, ErrorMessage = ResponseMessages.END_INSCRIPTIONS_ERROR });

				foreach (var inscription in inscriptions)
				{

					inscription.IsFinished = true;
					
					if (inscription.Accredit == true) inscription.Grade = 10;
					if (inscription.Accredit == false) inscription.Grade = 5;

					await _inscriptionsService.UpdateAsync(inscription);
				}

                var endCourse = await _coursesService.GetByIdAsync(course.Id);

				if (endCourse != null)
				{
					endCourse.IsActive = false;

					var endedResult = await _coursesService.UpdateAsync(endCourse);

					if (endedResult != null) return BadRequest(new DataResponse { Data = false, ErrorMessage = ResponseMessages.COURSE_ENDED });
				}

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

			if (await _studentsService.GetByIdAsync(studentId) is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
			if (await _coursesService.GetByIdAsync(courseId) is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

			
            var existingInscription = await _inscriptionsService.GetAllAsync(i => i.StudentId == studentId);
            if (existingInscription.Any())
                return BadRequest(new DataResponse { Data = null, ErrorMessage = "El estudiante" });


			//
            var checkIfInCourse = await _inscriptionsService.GetAllAsync(i => i.CourseId == courseId && i.StudentId == studentId,
				i => i.Student!,
				i => i.Course!,
				i => i.Course!.Instructor!
			);

			if (checkIfInCourse.Any()) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.ALREADY_IN_COURSE });

            var course = await _coursesService.GetByIdAsync(courseId);

			if (course == null) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            var entity = new Inscription
			{
				DateInscription = DateTime.Now,
				Accredit = false,
				CourseId = courseId,
				StudentId = studentId,
			};

			if (course.CurrentUsers >= course.MaxUsers) return Ok(new DataResponse { Data = null, ErrorMessage = "not added: " + ResponseMessages.EXCEEDED_MAX_USERS });

            var inscriptionResult = await _inscriptionsService.AddAsync(entity);

            if (inscriptionResult == null) return Ok(new DataResponse { Data = null, ErrorMessage = "not added: "+ResponseMessages.OBJECT_NOT_FOUND });

			course.CurrentUsers++;

            await _coursesService.UpdateAsync(course);

            return Ok(new DataResponse { Data = inscriptionResult!.ToDictionary(), ErrorMessage = null});
        }

		[HttpGet]
		[Route("inscription/check/{courseId}/{studentId}")]
		[Authorize]
		public async Task<IActionResult> CheckIfInCourse(string courseId, string studentId)
		{
			// First we have to check if the courseId and studentId, both exist on our database. Otherwise we shall return an error.

			if (await _studentsService.GetByIdAsync(studentId) is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
			if (await _coursesService.GetByIdAsync(courseId) is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

			var checkIfInCourse = await _inscriptionsService.GetAllAsync(i => i.CourseId == courseId && i.StudentId == studentId,
				i => i.Student!,
				i => i.Course!,
				i => i.Course!.Instructor!
			);

			if (checkIfInCourse.Any()) return Ok(new DataResponse { Data = true, ErrorMessage = null });

			return Ok(new DataResponse { Data = false, ErrorMessage = null });
		}

		[HttpGet]
		[Route("inscription/count/{studentId}")]
		[Authorize]
		public async Task<IActionResult> GetInscriptionCoursesByUserId(string studentId)
		{
			// First we have to check if the courseId and studentId, both exist on our database. Otherwise we shall return an error.

			if (await _studentsService.GetByIdAsync(studentId) is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

			var result = await _inscriptionsService.GetAllAsync(i => i.StudentId == studentId,
				i => i.Student!,
				i => i.Course!,
				i => i.Course!.Instructor!
			);

			var count = result.Count();

			return Ok(new DataResponse { Data = count, ErrorMessage = null });
		}

		[HttpGet]
		[Route("inscription/enrolled/{studentId}")]
		[Authorize]
		public async Task<IActionResult> GetEnrolledCoursesByUserId(string studentId)
		{
			// First we have to check if the courseId and studentId, both exist on our database. Otherwise we shall return an error.

			if (await _studentsService.GetByIdAsync(studentId) is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

			var result = await _inscriptionsService.GetAllAsync(i => i.StudentId == studentId,
				i => i.Student!,
				i => i.Course!,
				i => i.Course!.Instructor!
			);

			var data = new List<Dictionary<string, object>>();

			foreach (var item in result) data.Add(item.ToDictionary());

			if (result.Any()) return Ok(new DataResponse { Data = data, ErrorMessage = null });

			return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
		}

		[HttpGet]
		[Route("inscription/finished/{studentId}")]
		[Authorize]
		public async Task<IActionResult> GetFinishedCoursesById(string studentId)
		{
			// First we have to check if the courseId and studentId, both exist on our database. Otherwise we shall return an error.

			if (await _studentsService.GetByIdAsync(studentId) is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

			var result = await _inscriptionsService.GetAllAsync(i => i.StudentId == studentId && i.IsFinished == false,
				i => i.Student!,
				i => i.Course!,
				i => i.Course!.Instructor!
			);

			var data = new List<Dictionary<string, object>>();

			foreach (var item in result) data.Add(item.ToDictionary());

			if (result.Any()) return Ok(new DataResponse { Data = data, ErrorMessage = null });

			return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
		}

		[HttpGet]
		[Route("inscription/getbycourse/{courseId}")]
		[Authorize]

		public async Task<IActionResult> GetInscriptionsByCourseAsync(string courseId) { 

			if(!Guid.TryParse(courseId, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
			
			var course = await _coursesService.GetByIdAsync(courseId);

			if (course == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

			var result = await _inscriptionsService.GetAllAsync(i => i.CourseId == courseId,
				i => i.Student!,
				i => i.Course!,
				i => i.Course!.Instructor!
			);

			if (!result.Any()) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.NONE_INSCRIPTION_COURSE });

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

				if (result != null) return Ok(new DataResponse { Data = result, ErrorMessage = null });

				return Ok(new DataResponse { Data = result, ErrorMessage = null });
			}

			return Ok(new DataResponse { Data = null, ErrorMessage = "We could not accredit this user due to an internal error." });

		}




		[HttpDelete]
		[Route("inscription/remove/{courseId}/{studentId}")]
		[Authorize]
		public async Task<IActionResult> RemoveFromCourse(string courseId, string studentId)
		{
			// First we have to check if the courseId and studentId, both exist on our database. Otherwise we shall return an error.

			if (await _studentsService.GetByIdAsync(studentId) is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

			if (await _coursesService.GetByIdAsync(courseId) is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

			var checkIfInCourse = await _inscriptionsService.GetAllAsync(i => i.CourseId == courseId && i.StudentId == studentId);

			if (!checkIfInCourse.Any()) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.NOT_FOUND_IN_COURSE });


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

					return Ok(new DataResponse { Data = true, ErrorMessage = null });
				} }

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
