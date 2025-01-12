using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniSportUAQ_API.Data.Base;
using UniSportUAQ_API.Data.Consts;
using UniSportUAQ_API.Data.Interfaces;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;
using UniSportUAQ_API.Data.DTO;

using Hangfire;
using System.Globalization;
using Hangfire.Storage;
using Microsoft.AspNetCore.SignalR;
using UniSportUAQ_API.Hubs;
using Newtonsoft.Json;
using UniSportUAQ_API.Data.Services;
using Hangfire.States;

namespace UniSportUAQ_API.Controllers
{

    [ApiController]
    [Route("api/v1/courses")]
    public class CoursesController : Controller
    {
        private readonly ICoursesService _coursesService;
        private readonly IInscriptionsService _inscriptionsService;
        private readonly IUsersService _userService;
        private readonly IHorariosService _horariosService;
        private readonly IAttendancesService _attendancesService;
        private readonly IHangfireJobsService _hangfireJobsService;
        private readonly ISubjectsService _subjectsService;
        private readonly IHubContext<LessonHub> _hubContext;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public CoursesController(
            IBackgroundJobClient backgroundJobs,
            ISubjectsService subjectsService,
            IHangfireJobsService hangfireJobsService,
            IAttendancesService attendancesService,
            ICoursesService coursesService,
            IInscriptionsService inscriptionsService,
            IUsersService userService,
            IHorariosService horariosService,
            IHubContext<LessonHub> hubContext
        )
        {
            _backgroundJobClient = backgroundJobs;
            _coursesService = coursesService;
            _inscriptionsService = inscriptionsService;
            _userService = userService;
            _horariosService = horariosService;
            _attendancesService = attendancesService;
            _hangfireJobsService = hangfireJobsService;
            _subjectsService = subjectsService;
            _hubContext = hubContext;
        }

        [HttpGet]
        [Route("count")]
        [Authorize]
        public async Task<IActionResult> GetCourseCount()
        {

            var courses = await _coursesService.GetAllAsync(i => i.IsActive == true);
            var count = courses.Count();

            return Ok(new BaseResponse<int> { Data = count });
        }

        [HttpGet]
        [Route("id/{id}")]
        [Authorize]
        public async Task<IActionResult> GetCourseById(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest(new BaseResponse<CourseDTO> { Error = ResponseErrors.AttributeIdInvalidlFormat });

            var result = await _coursesService.GetByIdAsync(id, c => c.Instructor!, c => c.Horarios!);

            List<HorarioDTO> horariosDTO = new List<HorarioDTO>();

            if (result != null)
            {

                foreach (var horario in result.Horarios!)
                {

                    var horariosCourse = new HorarioDTO
                    {
                        Id = horario.Id,
                        Day = horario.Day,
                        StartHour = horario.StartHour,
                        EndHour = horario.EndHour,
                        CourseId = horario.CourseId,
                    };

                    horariosDTO.Add(horariosCourse);
                }
                var response = new CourseDTO
                {

                    Id = result.Id,
                    CourseName = result.CourseName,
                    InstructorName = result.Instructor!.FullName,
                    InstructorPicture = result.Instructor!.PictureUrl,
                    InstructorId = result.InstructorId,
                    MaxUsers = result.MaxUsers,
                    CurrentUsers = result.CurrentUsers,
                    Schedules = horariosDTO,
                    Description = result.Description,
                    Link = result.Link,
                    Location = result.Location,
                    IsVirtual = result.VirtualOrHybrid,
                    CoursePictureUrl = result.CoursePictureUrl,
                    StartDate = result.StartDate,
                    EndDate = result.EndDate,
					WorkshopId = result.SubjectId,
					MinAttendances = result.MinAttendances,
                };

                return Ok(new BaseResponse<object> { Data = response });
            }

            return Ok(new BaseResponse<CourseDTO> { Data = null });

        }

        [HttpGet]
        [Route("filter")]
        [Authorize]
        public async Task<IActionResult> GetCoursesByFilter(
            [FromQuery] string? q,
            [FromQuery] string? id,
            [FromQuery] string? instructorId,
            [FromQuery] int? start,
            [FromQuery] int? end,
            [FromQuery] bool? isActive
        )
        {
            var courses = new List<CourseDTO>();

            var result = await _coursesService.GetAllAsync(u =>
                (string.IsNullOrEmpty(instructorId) || u.InstructorId == instructorId) &&
                (string.IsNullOrEmpty(id) || u.Id == id) &&
                (string.IsNullOrEmpty(q) ||
                 u.CourseName!.ToLower().Contains(q.ToLower()) ||
                 u.Description!.ToLower().Contains(q.ToLower())) &&
                (!isActive.HasValue || u.IsActive == isActive.Value),
                start,
                end,
                c => c.Instructor!,
                c => c.Horarios!
            );

            foreach (var item in result)
            {
                courses.Add(new CourseDTO
                {
                    Id = item.Id,
                    CourseName = item.CourseName,
                    InstructorName = item.Instructor!.FullName,
                    InstructorPicture = item.Instructor.PictureUrl,
                    InstructorId = item.InstructorId,
                    MaxUsers = item.MaxUsers,
                    CurrentUsers = item.CurrentUsers,
                    Schedules = item.Horarios!.Select(h => new HorarioDTO
                    {
                        Id = h.Id,
                        Day = h.Day,
                        StartHour = h.StartHour,
                        EndHour = h.EndHour,
                        CourseId = h.CourseId,
                    }).ToList(),
                    Description = item.Description,
                    Link = item.Link,
                    Location = item.Location,
                    IsVirtual = item.VirtualOrHybrid,
                    CoursePictureUrl = item.CoursePictureUrl,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
					MinAttendances = item.MinAttendances,
					WorkshopId = item.SubjectId,
                });
            }


            if (result == null) return Ok(new BaseResponse<List<UserDTO>> { Data = new List<UserDTO>() });
            else return Ok(new BaseResponse<List<CourseDTO>> { Data = courses });
        }


        [HttpPut]
        [Route("update")]
        [Authorize]
		public async Task<IActionResult> UpdateCourse([FromBody] CourseSchema courseSchema)
		{
			if (string.IsNullOrEmpty(courseSchema.Id))
				return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeEmptyOrNull });

			if (string.IsNullOrEmpty(courseSchema.CourseName))
				return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeEmptyOrNull });

			if (courseSchema.InstructorId is null)
				return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeEmptyOrNull });

			if (courseSchema.MaxUsers <= 0)
				return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeEmptyOrNull });

			if (courseSchema.StartDate == DateTime.MinValue || courseSchema.EndDate == DateTime.MinValue)
				return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.CourseStartOrEndateMinValue });

			if (courseSchema.StartDate >= courseSchema.EndDate)
				return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.CourseStartEndateContradiction });

			var course = await _coursesService.GetByIdAsync(courseSchema.Id, i => i.Horarios!);
			if (course == null)
				return Ok(new BaseResponse<bool> { Data = false, Error = ResponseErrors.EntityNotExist });

			var dbSchedules = await _horariosService.GetAllAsync(i => i.CourseId == courseSchema.Id);

			course.CourseName = courseSchema.CourseName;
			course.MaxUsers = courseSchema.MaxUsers;
			course.Description = courseSchema.Description;
			course.MinAttendances = courseSchema.MinAttendances;
			course.StartDate = courseSchema.StartDate;
			course.EndDate = courseSchema.EndDate;
			course.Location = courseSchema.location;

			var newSchedules = courseSchema.Schedules ?? new List<HorarioSchema>();

			var schedulesToDelete = dbSchedules.Where(dbSchedule =>
				!newSchedules.Any(newSchedule => newSchedule.Day == dbSchedule.Day)).ToList();

			var schedulesToCreate = newSchedules.Where(newSchedule =>
				!dbSchedules.Any(dbSchedule => dbSchedule.Day == newSchedule.Day)).ToList();

			foreach (var dbSchedule in dbSchedules)
			{
				var matchingNewSchedule = newSchedules.FirstOrDefault(newSchedule => newSchedule.Day == dbSchedule.Day);
				if (matchingNewSchedule != null)
				{
					dbSchedule.StartHour = matchingNewSchedule.StartHour;
					dbSchedule.EndHour = matchingNewSchedule.EndHour;
					await _horariosService.UpdateAsync(dbSchedule);
				}
			}

			foreach (var scheduleToCreate in schedulesToCreate)
			{
				var newSchedule = new Horario
				{
					CourseId = courseSchema.Id,
					Day = scheduleToCreate.Day,
					StartHour = scheduleToCreate.StartHour,
					EndHour = scheduleToCreate.EndHour
				};
				await _horariosService.AddAsync(newSchedule);
			}

			foreach (var scheduleToDelete in schedulesToDelete)
			{
				await _horariosService.DeleteAsync(scheduleToDelete.Id);
			}

			var result = await _coursesService.UpdateAsync(course);
			return result is not null ?
				Ok(new BaseResponse<bool> { Data = true }) :
				Ok(new BaseResponse<bool> { Data = false, Error = ResponseErrors.ServerDataBaseErrorUpdating });
		}


		[HttpPut]
        [Route("deactivate/{id}")]
        [Authorize]
        public async Task<IActionResult> DeactivateCourse( string Id) 
        {
            if (string.IsNullOrEmpty(Id) || string.IsNullOrWhiteSpace(Id)) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeIdInvalidlFormat});

            var course = await _coursesService.GetByIdAsync(Id);

            if (course == null) return Ok(new BaseResponse<bool> { Data = false, Error = ResponseErrors.EntityNotExist });

            course.IsActive = false;

            var courseSaved = await _coursesService.UpdateAsync(course);

            if(courseSaved == null) return Ok(new BaseResponse<bool> { Error = ResponseErrors.ServerDataBaseErrorUpdating });

            return Ok(new BaseResponse<bool> { Data = true });
        }

        [HttpPut]
        [Route("deactivate/{id}/subject")]
        [Authorize]
        public async Task<IActionResult> DeactivateCourseAndSubject(string Id)
        {
            if (string.IsNullOrEmpty(Id) || string.IsNullOrWhiteSpace(Id)) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeIdInvalidlFormat });

            var course = await _coursesService.GetByIdAsync(Id, i => i.Subject!);
            

            if (course == null) return Ok(new BaseResponse<bool> { Data = false, Error = ResponseErrors.EntityNotExist });
            if (course.Subject == null) return Ok(new BaseResponse<bool> { Data = false, Error = ResponseErrors.EntityNotExist });

            course.IsActive = false;
            course.Subject.IsActive = false;

            var courseSaved = await _coursesService.UpdateAsync(course);
            
            if (courseSaved == null) return Ok(new BaseResponse<bool> { Error = ResponseErrors.ServerDataBaseErrorUpdating });

            var subjectSaved = await _subjectsService.UpdateAsync(course.Subject);

            if (subjectSaved == null) return Ok(new BaseResponse<bool> { Error = ResponseErrors.ServerDataBaseErrorUpdating });

            return Ok(new BaseResponse<bool> { Data = true });
        }

        [HttpPost]
        [Route("create")]
        [Authorize]
        public async Task<IActionResult> AddToCourse([FromBody] CourseSchema courseSchema)
        {
            if (courseSchema == null) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeSchemaEmpty });
            if (courseSchema.CourseName is null) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeNameEmpty });
            if (courseSchema.InstructorId is null) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeEmptyOrNull });
            if (courseSchema.SubjectId is null) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeEmptyOrNull });
            if (await _subjectsService.GetByIdAsync(courseSchema.SubjectId) == null) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.DataNotFound });
            if (courseSchema.EndDate == DateTime.MinValue || courseSchema.StartDate == DateTime.MinValue) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.CourseStartOrEndateMinValue });
            if (courseSchema.StartDate >= courseSchema.EndDate) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.CourseStartEndateContradiction });

            if ((await _userService.GetAllAsync(i => i.Id == courseSchema.InstructorId && i.IsInstructor == true)).Count() < 1) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeIsInstructorFalse });

            if (courseSchema.MaxUsers <= 0) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeEmptyOrNull });


            var courses = await _coursesService.GetAllAsync(c => c.InstructorId == courseSchema.InstructorId && c.IsActive == true);

            if (courses.Count() > 0)
            {
                foreach (var course in courses)
                {
                    var findedHorarios = await _horariosService.GetAllAsync(h => h.CourseId == course.Id);

                    if (findedHorarios.Count() > 0) 
                    {
                        if (IsScheduleConflict(findedHorarios, courseSchema.Schedules!)) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.CourseInstructorHindered });
                    }

                    
                }
            }

            var NewCourse = new Course
            {
                Id = Guid.NewGuid().ToString(),
                SubjectId = courseSchema.SubjectId,
                CourseName = courseSchema.CourseName,
                InstructorId = courseSchema.InstructorId,
                EndDate = courseSchema.EndDate,
                StartDate = courseSchema.StartDate,
                MaxUsers = courseSchema.MaxUsers,
                CurrentUsers = 0,
                Description = courseSchema.Description,
                IsActive = true,
                Location = courseSchema.location,
                MinAttendances = courseSchema.MinAttendances

            };
            var result = await _coursesService.AddAsync(NewCourse);

            List<Horario> horarios = new List<Horario>();

            if (result != null)
            {

                if (IsHorarioConflict(courseSchema.Schedules!)) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.CourseHorarioConfict });

                foreach (var horario in courseSchema.Schedules!)
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

                    if (newHorario == null)
                    {

                        return BadRequest(new BaseResponse<object> { Data = NewHorario, Error = ResponseErrors.ServerDataBaseErrorUpdating });
                    }

                    horarios.Add(newHorario);
                }

                var course = await _coursesService.GetByIdAsync(NewCourse.Id);

                if (course != null)
                {
                    HangfireSetter(course, horarios);


                    var hangfiresetted = await _coursesService.UpdateAsync(course);
                    if (hangfiresetted == null) return Ok(new BaseResponse<bool> { Data = true, Error = ResponseErrors.ServerDataBaseErrorUpdating });
                }
                return Ok((new BaseResponse<bool> { Data = true }));
            }

            return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.ServerDataBaseErrorUpdating });
        }

        [HttpPut]
        [Route("endcourse/{id}")]
        [Authorize]
        public async Task<IActionResult> EndCourse(string Id)
        {

            if (string.IsNullOrEmpty(Id)) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeIdInvalidlFormat });

            //get course
            var endCourse = await _coursesService.GetByIdAsync(Id);

            List<string> list = new List<string>();

            if (endCourse == null) return Ok(new BaseResponse<bool> { Data = false , Error = ResponseErrors.EntityNotExist});


            //check course existence

            if (endCourse.IsActive == true)
            {
                //get inscriptions related with course
                var inscriptions = await _inscriptionsService.GetAllAsync(i =>
                i.CourseId == Id &&
                i.UnEnrolled == false);

                //deactivate course
                endCourse.IsActive = false;

                //try to update course status
                var endedResult = await _coursesService.UpdateAsync(endCourse);

                if (endedResult == null) return BadRequest(new BaseResponse<bool> { Data = false, Error = ResponseErrors.CourseCanNotEnd });


                //verify inscriptions related with course
                if (inscriptions.Count() > 0)
                {

                    //End inscriptions related with that course
                    foreach (var inscription in inscriptions)
                    {
                        //if(inscription.unenroled == false)
                        //execute next lines

                        //count the assitance for that inscription 
                        var assistance = await _attendancesService.GetAllAsync(i => i.StudentId == inscription.StudentId &&
                        i.CourseId == inscription.CourseId &&
                        i.AttendanceClass == true
                        );

                        var countAttendance = assistance.Count();

                        //if have more or equal to the minimal attendances acrredit user
                        if (countAttendance >= endCourse.MinAttendances)
                        {


                            inscription.Grade = 10;
                            inscription.Accredit = true;

                        }
                        else
                        {

                            inscription.Grade = 5;
                            inscription.Accredit = false;
                        }

                        //end inscription
                        inscription.IsFinished = true;

                        var endedInscription = await _inscriptionsService.UpdateAsync(inscription);

                        //in case ther is an error add user id to a list to provide info
                        if (endedInscription == null) list.Add(inscription.StudentId!);
                    }



                    if (list.Any()) return Ok(new BaseResponse<List<string>> { Data = list, Error = ResponseErrors.CourseEndInscriptionProblem });



                }
            }

            return Ok(new BaseResponse<bool> { Data = true });




        }

        /**************************************************************inscriptions**************************************************************/

        [HttpPost]
        [Route("inscription/{courseId}/{studentId}")]
        [Authorize]
        public async Task<IActionResult> AddToCourse(string courseId, string studentId)
        {
            // First we have to check if the courseId and studentId, both exist on our database. Otherwise we shall return an error.

            if (await _userService.GetByIdAsync(studentId) is null) return BadRequest(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.EntityNotExist });
            if (await _coursesService.GetByIdAsync(courseId) is null) return BadRequest(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.EntityNotExist });


            var existingInscription = await _inscriptionsService.GetAllAsync(i => i.StudentId == studentId, i => i.Course!);

            foreach (var item in existingInscription)
            {

                if (item.Course!.IsActive) return BadRequest(new BaseResponse<bool> { Data = false, Error = ResponseErrors.InscriptionStudentAlredyInscripted });

            }

            //if (existingInscription.Any()) return BadRequest(new BaseResponse<bool> { Data = false, Error= ResponseErrors.InscriptionAlreadyExist });


            //
            var checkIfInCourse = await _inscriptionsService.GetAllAsync(i => i.CourseId == courseId && i.StudentId == studentId);

            if (checkIfInCourse.Any()) return BadRequest(new BaseResponse<bool> { Data = false, Error = ResponseErrors.InscriptionAlreadyExist });

            var course = await _coursesService.GetByIdAsync(courseId, c => c.Horarios!, c => c.Instructor!);

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

            if (inscriptionResult == null) return Ok(new BaseResponse<bool> { Data = false, Error = ResponseErrors.ServerDataBaseErrorUpdating });

            course.CurrentUsers++;

            await _coursesService.UpdateAsync(course);

            var courseDto = new CourseDTO
            {
                Id = course.Id,
                CourseName = course.CourseName,
                InstructorName = course.Instructor!.FullName,
                InstructorPicture = course.Instructor!.PictureUrl,
                InstructorId = course.InstructorId,
                MaxUsers = course.MaxUsers,
                CurrentUsers = course.CurrentUsers,
                Description = course.Description,
                Link = course.Link,
                Location = course.Location,
                Schedules = course.Horarios!.Select(h => new HorarioDTO
                {
                    Id = h.Id,
                    Day = h.Day,
                    StartHour = h.StartHour,
                    EndHour = h.EndHour,
                    CourseId = h.CourseId
                }).ToList(),
                CoursePictureUrl = course.CoursePictureUrl,
                IsVirtual = course.VirtualOrHybrid,
				MinAttendances = course.MinAttendances,
				WorkshopId = course.SubjectId,
				EndDate = course.EndDate,
				StartDate = course.StartDate,
            };

            await _hubContext.Clients.All.SendAsync("CourseUpdated", courseDto);

            return Ok(new BaseResponse<bool> { Data = true });
        }



        [HttpGet]
        [Route("inscription/check/{courseId}/{studentId}")]
        [Authorize]
        public async Task<IActionResult> CheckIfInCourse(string courseId, string studentId)
        {
            // First we have to check if the courseId and studentId, both exist on our database. Otherwise we shall return an error.

            if (await _userService.GetByIdAsync(studentId) is null) return BadRequest(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.AuthUserNotFound });
            if (await _coursesService.GetByIdAsync(courseId) is null) return BadRequest(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.AuthUserNotFound });

            var checkIfInCourse = await _inscriptionsService.GetAllAsync(i => i.CourseId == courseId && i.StudentId == studentId);

            if (checkIfInCourse.Any()) return Ok(new BaseResponse<bool> { Data = true });

            return Ok(new BaseResponse<bool> { Data = false, Error = ResponseErrors.EntityNotExist });
        }

        [HttpGet]
        [Route("inscription/count/{studentId}")]
        [Authorize]
        public async Task<IActionResult> GetInscriptionCoursesByUserId(string studentId)
        {
            // First we have to check if the courseId and studentId, both exist on our database. Otherwise we shall return an error.

            if (await _userService.GetByIdAsync(studentId) is null) return BadRequest(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.EntityNotExist });

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

            if (await _userService.GetByIdAsync(studentId) is null) return BadRequest(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.EntityNotExist });

            var result = await _inscriptionsService.GetAllAsync(i => i.StudentId == studentId,
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
                    Expediente = item.Student!.Expediente

                };

                data.Add(inscription);
            }

            return Ok(new BaseResponse<List<InscriptionDTO>> { Data = data });
        }

        [HttpPut]
        [Route("inscription/unenrolled/{courseId}/{studentId}")]
        [Authorize]
        public async Task<IActionResult> UnenrolledStudent(string courseId, string studentId)
        {
            //check exists
            if (await _coursesService.GetByIdAsync(courseId) is null) return BadRequest(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.EntityNotExist });

            if (await _userService.GetByIdAsync(studentId) is null) return BadRequest(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.EntityNotExist });

            //check inscription
            var result = await _inscriptionsService.GetAllAsync(i => i.CourseId == courseId && i.StudentId == studentId);

            if (!result.Any()) return BadRequest(new BaseResponse<InscriptionDTO> { Data = null });

            //update inscription
            var inscription = result.FirstOrDefault();
            if (inscription != null)
            {
                inscription.IsFinished = true;
                inscription.UnEnrolled = true;
                await _inscriptionsService.UpdateAsync(inscription);

                //update inscriptions
                var fromCourse = await _coursesService.GetByIdAsync(courseId);
                if (fromCourse != null && fromCourse.CurrentUsers > 0)
                {
                    fromCourse.CurrentUsers--;
                    await _coursesService.UpdateAsync(fromCourse);
                }

                return Ok(new BaseResponse<bool> { Data = true });
            }

            return NotFound(new BaseResponse<bool> { Data = false });
        }

        [HttpPut]
        [Route("inscription/switchstudents/{inscription1}/{inscription2}")]
        [Authorize]
        public async Task<IActionResult> TransferStudent(string inscription1, string inscription2)
        {
            //cehck format string
            if (string.IsNullOrEmpty(inscription1)) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeIdInvalidlFormat });
            if (string.IsNullOrEmpty(inscription2)) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeIdInvalidlFormat });

            //check exists


            //guardar en dos var ins1 y la 2
            var ins1 = await _inscriptionsService.GetByIdAsync(inscription1);
            if (ins1 is null) return Ok(new BaseResponse<bool> { Error = ResponseErrors.EntityNotExist });

            var ins2 = await _inscriptionsService.GetByIdAsync(inscription2);
            if (ins2 is null) return Ok(new BaseResponse<bool> { Error = ResponseErrors.EntityNotExist });


            //obtener todas las asistencias de ins1 por courso y alumno con attendances service getall(params)
            var ins1Attendances = await _attendancesService.GetAllAsync(a => a.StudentId == ins1.StudentId && a.CourseId == ins1.CourseId);
            if (!ins1Attendances.Any()) return Ok(new BaseResponse<bool> { Error = ResponseErrors.EntityNotExist });

            //obtener todas las asistencias de ins2 por courso y alumno con attendances service getall(params)
            var ins2Attendances = await _attendancesService.GetAllAsync(i => i.StudentId == ins2.StudentId && i.CourseId == ins2.CourseId);
            if (!ins1Attendances.Any()) return Ok(new BaseResponse<bool> { Error = ResponseErrors.EntityNotExist });

            //not posible to update list
            List<Attendance> notPossibleUpdt = new List<Attendance>();


            //cambiar el course id de ins 1 por el course id de ins2
            foreach (var attendance in ins1Attendances)
            {
                attendance.CourseId = ins2.CourseId;
                var ins1att = await _attendancesService.UpdateAsync(attendance);

                if (ins1att is not null) notPossibleUpdt.Add(attendance);
            }

            //cambiar el course id de ins 2 por el course id de ins1
            foreach (var attendance in ins2Attendances)
            {
                attendance.CourseId = ins1.CourseId;
                var ins2att = await _attendancesService.UpdateAsync(attendance);
                if (ins2att is not null) notPossibleUpdt.Add(attendance);
            }



            //si se guardan correctamente cambiar ins1 course por ins2 course y cambiar ins2 course por ins1 course
            var tempCourseId = ins1.CourseId;
            ins1.CourseId = ins2.CourseId;
            ins2.CourseId = tempCourseId;

            //guardar ambos con update async
            await _inscriptionsService.UpdateAsync(ins1);
            await _inscriptionsService.UpdateAsync(ins2);

            if (notPossibleUpdt.Any()) return Ok(new BaseResponse<List<Attendance>> { Data = notPossibleUpdt, Error = ResponseErrors.CourseInscriptionAttendanceProblemUpdate });

            //si se guarda satisfactoriamente return ok data true
            return Ok(new BaseResponse<bool> { Data = true });
        }

        [HttpGet]
        [Route("inscription/finished/{studentId}")]
        [Authorize]
        public async Task<IActionResult> GetFinishedCoursesById(string studentId)
        {
            // First we have to check if the courseId and studentId, both exist on our database. Otherwise we shall return an error.

            if (await _userService.GetByIdAsync(studentId) is null) return BadRequest(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.EntityNotExist });

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
                    Accredit = item.Accredit,
                    Expediente = item.Student!.Expediente,

                };

                data.Add(inscription);
            }

            return Ok(new BaseResponse<List<InscriptionDTO>> { Data = data });
        }

        [HttpGet]
        [Route("inscription/course/{courseId}")]
        [Authorize]

        public async Task<IActionResult> GetInscriptionsByCourseAsync(string courseId)
        {
            var course = await _coursesService.GetByIdAsync(courseId);

            if (course == null) return BadRequest(new BaseResponse<List<InscriptionDTO>> { Error = ResponseErrors.CourseNotFound });

            var result = await _inscriptionsService.GetAllAsync(q => q.CourseId == courseId,
                i => i.Student!,
                i => i.Course!,
                i => i.Course!.Instructor!
            );

            if (!result.Any()) return Ok(new BaseResponse<List<InscriptionDTO>> { Data = new List<InscriptionDTO>() });

            var data = new List<InscriptionDTO>();

            foreach (var item in result)
            {
                var inscription = new InscriptionDTO
                {
                    Id = item.Id,
                    CourseId = item.CourseId,
                    CourseName = item.Course!.CourseName,
                    CourseDescription = item.Course!.Description,
                    Accredit = item.Accredit,
                    StudentName = item.Student!.FullName,
                    StudentId = item.StudentId,
                    Expediente = item.Student.Expediente
                };

                data.Add(inscription);
            }

            return Ok(new BaseResponse<List<InscriptionDTO>> { Data = data });
        }


        [HttpPut]
        [Route("inscription/acredit")]
        [Authorize]
        public async Task<IActionResult> AcreditCourse([FromBody] Inscription inscription)
        {

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

            return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.InscriptionNotAccredit });

        }




        [HttpDelete]
        [Route("inscription/remove/{courseId}/{studentId}")]
        [Authorize]
        public async Task<IActionResult> RemoveFromCourse(string courseId, string studentId)
        {
            // First we have to check if the courseId and studentId, both exist on our database. Otherwise we shall return an error.

            if (await _userService.GetByIdAsync(studentId) is null) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.EntityNotExist });

            if (await _coursesService.GetByIdAsync(courseId) is null) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.EntityNotExist });

            var checkIfInCourse = await _inscriptionsService.GetAllAsync(i => i.CourseId == courseId && i.StudentId == studentId);

            if (!checkIfInCourse.Any()) return Ok(new BaseResponse<bool> { Error = ResponseErrors.CourseNotFoundInscription });


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
                }
            }

            return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.CourseErrorRemoving });
        }



        //**********************************local use**********************************



        [ApiExplorerSettings(IgnoreApi = true)]
        private bool IsScheduleConflict(IEnumerable<Horario> existingCourse, List<HorarioSchema> newCourse)
        {

            

            if (existingCourse.Count() < 1 || newCourse.Count() < 1) return false;

            foreach (var existingHr in existingCourse)
            {

                foreach (var newHr in newCourse)
                {
                    if (existingHr.Day!.ToLower() == newHr.Day!.ToLower())
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

        [ApiExplorerSettings(IgnoreApi = true)]
        private bool IsHorarioConflict(List<HorarioSchema> horarios)
        {

            HorarioSchema? lastHorario = null;

            foreach (var horario in horarios)
            {

                if (lastHorario != null)
                {

                    if (horario.Day == lastHorario.Day)
                    {

                        if (horario.StartHour < lastHorario.EndHour && horario.EndHour > lastHorario.StartHour) return true;

                    }

                }

                lastHorario = horario;

            }


            return false;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private async void HangfireSetter(Course course, List<Horario> horarios)
        {

            TimeZoneInfo utcMinusSix = TimeZoneInfo.CreateCustomTimeZone("UTC-06", new TimeSpan(-6, 0, 0), "UTC-06", "UTC-06");


            //assistancecheck recurrent job
            try
            {

                //if it is updated delete previous recurring jobs
                if (course.UnsetAttenancesReccJob != null)
                {
                    try
                    {
                        BackgroundJob.Delete(course.UnsetAttenancesReccJob);
                        RemoveAllJobsForCourse(course.Id!);
                        course.UnsetAttenancesReccJob = null;
                    }
                    catch (Exception ex){
                        Console.WriteLine(ex.ToString()+"\n");
                    }

                }

                //para cada horario
                foreach (var horario in horarios)
                {
                    var day = horario.Day;
                    TimeSpan checkHour = horario.EndHour.Add(new TimeSpan(1, 0, 0));


                    string? cronExpresion = GetCronExpressionForDayAndTime(day!, checkHour.ToString(@"hh\:mm"));


                    if (cronExpresion != null)
                    {


                        //to create new ones
                        RecurringJob.AddOrUpdate(
                            $"job-{day + "-" + course.Id! + "-" + horario.EndHour}",
                            () => CheckTodayAttendance(DateTime.Now, course.Id!, day!.ToLower(), course.StartDate, course.EndDate, horario.Id!),
                            cronExpresion,
                            new RecurringJobOptions
                            {
                                TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")
                            }
                        );

                        if (course.UnsetAttenancesReccJob == null)
                        {

                            //create new scheduled job to delete the above recurring jobs
                            string UnsetAttenancesReccJob = BackgroundJob.Schedule(
                                () => RemoveAllJobsForCourse(course.Id!),
                                course.EndDate.AddMinutes(20)
                            );

                            course.UnsetAttenancesReccJob = UnsetAttenancesReccJob;
                        }

                    }
                    else
                    {
                        Console.WriteLine($"Not valid day: {day!.ToLower()} or var CronExpression null\n");
                    }

                    Console.WriteLine($"Task for cours: {course.Id} was created\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error dates {ex}\n");

            }

            //end course programed task
            try
            {
                if (course.EndCourseIdJob != null)
                {
                    BackgroundJob.Delete(course.EndCourseIdJob);
                }

                string EndCourseIdJob = BackgroundJob.Schedule(
                    () => EndCourseTask(course.Id!),
                    course.EndDate.AddMinutes(10)
                    );

                course.EndCourseIdJob = EndCourseIdJob;




            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating task end course automatically: {ex}\n");
            }

            try
            {
                if (course.GenerateCartasIdJob != null)
                {
                    BackgroundJob.Delete(course.GenerateCartasIdJob);

                }

                string GenerateCartasIdJob = BackgroundJob.Schedule(
                 () => _hangfireJobsService.GenerateAllCartasAsync(course.Id!),
                 course.EndDate.AddMinutes(15)
                 );

                course.GenerateCartasIdJob = GenerateCartasIdJob;



            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating task GENERATE ALL CARTAS automatically: {ex}\n");
            }
        }


        // Método sincrónico que llama al método asincrónico
        [ApiExplorerSettings(IgnoreApi = true)]
        public void CheckTodayAttendance(DateTime date, string courseId, string day, DateTime startDate, DateTime endDate, string horarioId)
        {
            CheckTodayAttendanceAsync(date, courseId, day, startDate, endDate, horarioId).GetAwaiter().GetResult();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task CheckTodayAttendanceAsync(DateTime date, string courseId, string day, DateTime startDate, DateTime endDate, string horarioId)
        {


            var course = await _coursesService.GetByIdAsync(courseId);
            var horario = await _horariosService.GetByIdAsync(horarioId);

            if (course != null && horario != null)
            {
                if (course.IsActive == true)
                {

                    DateTime dateEndHour = DateTime.Now.Date.Date.Add(horario.EndHour);
                    DateTime dateStartHour = DateTime.Now.Date.Date.Add(horario.StartHour);

                    Console.WriteLine($"Executing VerifyAssistence to day: {day}\n");


                    var Attendances = await _attendancesService.GetAllAsync(i =>
                        i.CourseId == courseId &&
                        i.Date >= dateStartHour &&
                        i.Date <= dateEndHour);



                    if (Attendances.Count() > 0)
                    {
                        Console.WriteLine($"Intructor for course with Id {courseId} has taken attendances for day:{day}\n");
                    }
                    else
                    {

                        Console.WriteLine($"Intructor for course with {courseId} has NOT TAKEN attendances for day:{day}\n");

                        // Obtener inscripciones para el curso
                        var inscriptions = await _inscriptionsService.GetAllAsync(i =>
                            i.CourseId == courseId &&
                            i.UnEnrolled == false,
                            i => i.Course!,
                            i => i.Student!);

                        if (!inscriptions.Any())
                        {
                            Console.WriteLine($"this course {courseId} has NOT inscriptions cannot make attendances for today\n");
                        }
                        else
                        {



                            // Generar asistencias
                            foreach (var inscription in inscriptions)
                            {
                                var attendance = new Attendance
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    CourseId = inscription.CourseId,
                                    StudentId = inscription.StudentId,
                                    Date = date,
                                    AttendanceClass = true,
                                };


                                var newattendance = await _attendancesService.AddAsync(attendance);

                                var checkattendance = await _attendancesService.GetByIdAsync(attendance.Id);

                                if (checkattendance is not null)
                                {
                                    if (inscription.Student != null && inscription.Course != null)
                                    {
                                        Console.WriteLine($"Generated attendance for student with exp {inscription.Student.Expediente} for course with id:{inscription.Course.Id}\n");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Student or Course information is missing for inscription with CourseId: {inscription.CourseId}\n");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"Can not generate attendance for student with exp {inscription.Student!.Expediente} for course with id:{inscription.Course!.Id}\n");
                                }
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Can not generate attendance for course: {course.Id}: It ended\n");

                }
            }

        }


        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task EndCourseTask(string CourseId)
        {
            var endCourse = await _coursesService.GetByIdAsync(CourseId);

            List<string> list = new List<string>();


            //check course existence
            if (endCourse is not null)
            {
                if (endCourse.IsActive == true)
                {
                    //get inscriptions related with course
                    var inscriptions = await _inscriptionsService.GetAllAsync(i =>
                    i.CourseId == CourseId &&
                    i.UnEnrolled == false);

                    //deactivate course
                    endCourse.IsActive = false;

                    //try to update course status
                    var endedResult = await _coursesService.UpdateAsync(endCourse);

                    //verify inscriptions related with course
                    if (endedResult != null)
                    {


                        if (inscriptions.Count() > 0)
                        {

                            //End inscriptions related with that course
                            foreach (var inscription in inscriptions)
                            {
                                //if(inscription.unenroled == false)
                                //execute next lines

                                //count the assitance for that inscription 
                                var assistance = await _attendancesService.GetAllAsync(i => i.StudentId == inscription.StudentId && i.CourseId == inscription.CourseId
                                && i.AttendanceClass == true);

                                var countAttendance = assistance.Count();

                                //if have more or equal to the minimal attendances acrredit user
                                if (countAttendance >= endCourse.MinAttendances)
                                {


                                    inscription.Grade = 10;
                                    inscription.Accredit = true;

                                }
                                else
                                {

                                    inscription.Grade = 5;
                                    inscription.Accredit = false;
                                }

                                //end inscription
                                inscription.IsFinished = true;

                                var endedInscription = await _inscriptionsService.UpdateAsync(inscription);

                                //in case ther is an error add user id to a list to provide info
                                if (endedInscription == null) list.Add(inscription.StudentId!);
                            }

                            if (list.Any())
                            {

                                Console.WriteLine($"all the inscriptions ended correctly\n");

                                foreach (var element in list)
                                {
                                    Console.WriteLine($"partial isncriptions ended, Can not end inscription for this user id: {element}\n");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"all the inscriptions ended correctly\n");
                            }

                        }
                        else
                        {

                            //return NotFound(new BaseResponse<bool> { Data = false, Error = ResponseErrors.EntityNotExist });
                            Console.WriteLine($"COURSE ENDED but: There is no isncriptions for this course: {endCourse.Id}\n");
                        }
                        //return BadRequest(new BaseResponse<bool> { Data = false, Error = ResponseErrors.CourseCanNotEnd });
                    }
                    else
                    {
                        Console.WriteLine($"COURSE can not end error updating in database\n");
                    }
                }
                else
                {
                    Console.WriteLine($"The course has already ended\n");
                }
            }
            else
            {
                Console.WriteLine($"Can not find a course with the id you provided \n");
            }

        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public void RemoveAllJobsForCourse(string courseId)
        {
            var recurringJobs = JobStorage.Current.GetConnection().GetRecurringJobs();

            foreach (var job in recurringJobs.Where(j => j.Id.Contains(courseId)))
            {
                RecurringJob.RemoveIfExists(job.Id);
            }
        }





        private string? GetCronExpressionForDayAndTime(string day, string time)
        {
            var timeParts = time.Split(':');
            if (timeParts.Length != 2) return null;

            var minute = timeParts[1];
            var hour = timeParts[0];

            Console.WriteLine($"day {day}, hour {hour}, minute{minute}");

            return day.ToLower() switch
            {
                "monday" => $"{minute} {hour} * * 1",
                "lunes" => $"{minute} {hour} * * 1",
                "tuesday" => $"{minute} {hour} * * 2",
                "martes" => $"{minute} {hour} * * 2",
                "wednesday" => $"{minute} {hour} * * 3",
                "miercoles" => $"{minute} {hour} * * 3",
                "miércoles" => $"{minute} {hour} * * 3",
                "thursday" => $"{minute} {hour} * * 4",
                "jueves" => $"{minute} {hour} * * 4",
                "friday" => $"{minute} {hour} * * 5",
                "viernes" => $"{minute} {hour} * * 5",
                "saturday" => $"{minute} {hour} * * 6",
                "sabado" => $"{minute} {hour} * * 6",
                "sábado" => $"{minute} {hour} * * 6",
                "sunday" => $"{minute} {hour} * * 0",
                "domingo" => $"{minute} {hour} * * 0",
                _ => null
            };
        }
    }
}
