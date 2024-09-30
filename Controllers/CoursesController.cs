using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniSportUAQ_API.Data.Base;
using UniSportUAQ_API.Data.Consts;
using UniSportUAQ_API.Data.Interfaces;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;
using UniSportUAQ_API.Data.DTO;

using Hangfire;

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

        public CoursesController(IHangfireJobsService hangfireJobsService, IAttendancesService attendancesService, ICoursesService coursesService, IInscriptionsService inscriptionsService, IUsersService userService, IHorariosService horariosService)
        {
            _coursesService = coursesService;
            _inscriptionsService = inscriptionsService;
            _userService = userService;
            _horariosService = horariosService;
            _attendancesService = attendancesService;
            _hangfireJobsService = hangfireJobsService;
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
                    Horarios = horariosDTO,
                    Description = result.Description,
                    Link = result.Link,
                    Location = result.Location,
                    IsVirtual = result.VirtualOrHybrid,
                    CoursePictureUrl = result.CoursePictureUrl,

                };

                return Ok(new BaseResponse<object> { Data = response });
            }

            return NotFound(new BaseResponse<CourseDTO> { Data = null });

        }

        [HttpGet]
        [Route("all")]
        [Authorize]
        public async Task<IActionResult> GetAllCourses()
        {
            var result = await _coursesService.GetAllAsync(c => c.IsActive == true, c => c.Instructor!, c => c.Horarios!);

            if (result != null)
            {

                var data = new List<CourseDTO>();

                foreach (var item in result)
                {

                    List<HorarioDTO> horariosDTO = new List<HorarioDTO>();

                    foreach (var horario in item.Horarios!) 
                    {

                        var horariosCourse = new HorarioDTO
                        {
                            Day = horario.Day,
                            StartHour = horario.StartHour,
                            EndHour = horario.EndHour,
                            CourseId = horario.CourseId,
                        };

                        horariosDTO.Add(horariosCourse);
                    }

                    var course = new CourseDTO
                    {

                        Id = item.Id,
                        CourseName = item.CourseName,
                        InstructorName = item.Instructor!.FullName,
                        InstructorPicture = item.Instructor!.PictureUrl,
                        InstructorId = item.InstructorId,
                        MaxUsers = item.MaxUsers,
                        CurrentUsers = item.CurrentUsers,
                        Horarios = horariosDTO,
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

                List<HorarioDTO> horariosDTO = new List<HorarioDTO>();

                foreach (var horario in item.Horarios!)
                {

                    var horariosCourse = new HorarioDTO
                    {
                        Day = horario.Day,
                        StartHour = horario.StartHour,
                        EndHour = horario.EndHour,
                        CourseId = horario.CourseId,
                    };

                    horariosDTO.Add(horariosCourse);
                }

                var course = new CourseDTO
                {

                    Id = item.Id,
                    CourseName = item.CourseName,
                    InstructorName = item.Instructor!.FullName,
                    InstructorPicture = item.Instructor!.PictureUrl,
                    InstructorId = item.InstructorId,
                    MaxUsers = item.MaxUsers,
                    CurrentUsers = item.CurrentUsers,
                    Horarios = horariosDTO,
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

            if (result.Count() < 1) return NotFound(new BaseResponse<List<UserDTO>> { Data = null });

            var courseInRange = result.Skip(start).Take(end - start + 1).ToList();

            var data = new List<CourseDTO>();

            foreach (var item in courseInRange)
            {

                List<HorarioDTO> horariosDTO = new List<HorarioDTO>();

                foreach (var horario in item.Horarios!)
                {

                    var horariosCourse = new HorarioDTO
                    {
                        Day = horario.Day,
                        StartHour = horario.StartHour,
                        EndHour = horario.EndHour,
                        CourseId = horario.CourseId,
                    };

                    horariosDTO.Add(horariosCourse);
                }

                var course = new CourseDTO
                {

                    Id = item.Id,
                    CourseName = item.CourseName,
                    InstructorName = item.Instructor!.FullName,
                    InstructorPicture = item.Instructor!.PictureUrl,
                    InstructorId = item.InstructorId,
                    MaxUsers = item.MaxUsers,
                    CurrentUsers = item.CurrentUsers,
                    Horarios = horariosDTO,
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

            if (!result.Any()) return NotFound(new BaseResponse<List<CourseDTO>> { Data = null });

            var data = new List<CourseDTO>();

            foreach (var item in result)
            {
                List<HorarioDTO> horariosDTO = new List<HorarioDTO>();

                foreach (var horario in item.Horarios!)
                {

                    var horariosCourse = new HorarioDTO
                    {
                        Day = horario.Day,
                        StartHour = horario.StartHour,
                        EndHour = horario.EndHour,
                        CourseId = horario.CourseId,
                    };

                    horariosDTO.Add(horariosCourse);
                }


                var course = new CourseDTO
                {

                    Id = item.Id,
                    CourseName = item.CourseName,
                    InstructorName = item.Instructor!.FullName,
                    InstructorPicture = item.Instructor!.PictureUrl,
                    InstructorId = item.InstructorId,
                    MaxUsers = item.MaxUsers,
                    CurrentUsers = item.CurrentUsers,
                    Horarios = horariosDTO,
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
        [Route("instructorid/{instructorid}/active")]
        [Authorize]
        public async Task<IActionResult> GetActiveCoursesByInstructorId(string instructorid)
        {

            if (string.IsNullOrEmpty(instructorid)) return BadRequest(new BaseResponse<List<CourseDTO>> { Error = ResponseErrors.AttributeIdInvalidlFormat });

            var result = await _coursesService.GetAllAsync(c => c.InstructorId == instructorid && c.IsActive == true, c => c.Instructor!);

            if (!result.Any()) return NotFound(new BaseResponse<List<CourseDTO>> { Error = ResponseErrors.EntityNotExist });

            var data = new List<CourseDTO>();

            foreach (var item in result)
            {
                List<HorarioDTO> horariosDTO = new List<HorarioDTO>();

                foreach (var horario in item.Horarios!)
                {

                    var horariosCourse = new HorarioDTO
                    {
                        Day = horario.Day,
                        StartHour = horario.StartHour,
                        EndHour = horario.EndHour,
                        CourseId = horario.CourseId,
                    };

                    horariosDTO.Add(horariosCourse);
                }


                var course = new CourseDTO
                {

                    Id = item.Id,
                    CourseName = item.CourseName,
                    InstructorName = item.Instructor!.FullName,
                    InstructorPicture = item.Instructor!.PictureUrl,
                    InstructorId = item.InstructorId,
                    MaxUsers = item.MaxUsers,
                    CurrentUsers = item.CurrentUsers,
                    Horarios = horariosDTO,
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
        [Route("instructorid/{instructorid}/inactive")]
        [Authorize]
        public async Task<IActionResult> GetInactiveCoursesByInstructorId(string instructorid)
        {

            if (string.IsNullOrEmpty(instructorid)) return BadRequest(new BaseResponse<List<CourseDTO>> { Error = ResponseErrors.AttributeIdInvalidlFormat });

            var result = await _coursesService.GetAllAsync(c => c.InstructorId == instructorid && c.IsActive == false, c => c.Instructor!);

            var data = new List<CourseDTO>();

            foreach (var item in result)
            {
                List<HorarioDTO> horariosDTO = new List<HorarioDTO>();

                foreach (var horario in item.Horarios!)
                {

                    var horariosCourse = new HorarioDTO
                    {
                        Day = horario.Day,
                        StartHour = horario.StartHour,
                        EndHour = horario.EndHour,
                        CourseId = horario.CourseId,
                    };

                    horariosDTO.Add(horariosCourse);
                }

                var course = new CourseDTO
                {

                    Id = item.Id,
                    CourseName = item.CourseName,
                    InstructorName = item.Instructor!.FullName,
                    InstructorPicture = item.Instructor!.PictureUrl,
                    InstructorId = item.InstructorId,
                    MaxUsers = item.MaxUsers,
                    CurrentUsers = item.CurrentUsers,
                    Horarios = horariosDTO,
                    Description = item.Description,
                    Link = item.Link,
                    Location = item.Location,
                    IsVirtual = item.VirtualOrHybrid,
                    CoursePictureUrl = item.CoursePictureUrl,

                };

                data.Add(course);
            }

            if (result.Any()) return Ok(new BaseResponse<List<CourseDTO>> { Data = data });

            return NotFound(new BaseResponse<List<CourseDTO>> { Error = ResponseErrors.EntityNotExist });
        }


        [HttpGet]
        [Route("search/{searchTerm}")]
        [Authorize]
        public async Task<IActionResult> GetCoursesSearch(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return BadRequest(new BaseResponse<List<CourseDTO>> { Data = null, Error = ResponseErrors.FilterInvalidSearchTerm });

            var result = await _coursesService.GetAllAsync(i =>
                ((i.CourseName != null && i.CourseName.ToLower().Contains(searchTerm)) ||
                 (i.Description != null && i.Description.ToLower().Contains(searchTerm)) ||
                 (i.Horarios != null && i.Horarios.Any(h => h.Day != null && h.Day.ToLower().Contains(searchTerm)))
                )
                &&
                i.IsActive == true, c => c.Instructor!
            );

            var distinctResult = result.Distinct();

            var data = new List<CourseDTO>();

            foreach (var item in distinctResult)
            {

                List<HorarioDTO> horariosDTO = new List<HorarioDTO>();

                foreach (var horario in item.Horarios!)
                {

                    var horariosCourse = new HorarioDTO
                    {
                        Day = horario.Day,
                        StartHour = horario.StartHour,
                        EndHour = horario.EndHour,
                        CourseId = horario.CourseId,
                    };

                    horariosDTO.Add(horariosCourse);
                }

                var course = new CourseDTO
                {

                    Id = item.Id,
                    CourseName = item.CourseName,
                    InstructorName = item.Instructor!.FullName,
                    InstructorPicture = item.Instructor!.PictureUrl,
                    InstructorId = item.InstructorId,
                    MaxUsers = item.MaxUsers,
                    CurrentUsers = item.CurrentUsers,
                    Horarios = horariosDTO,
                    Description = item.Description,
                    Link = item.Link,
                    Location = item.Location,
                    IsVirtual = item.VirtualOrHybrid,
                    CoursePictureUrl = item.CoursePictureUrl,

                };

                data.Add(course);
            }

            if (distinctResult.Any()) return Ok(new BaseResponse<List<CourseDTO>> { Data = data });

            return NotFound(new BaseResponse<List<CourseDTO>> { Error = ResponseErrors.EntityNotExist });
        }


        [HttpPut]
        [Route("update")]
        [Authorize]
        public async Task<IActionResult> UpdateCourse([FromBody] CourseSchema courseSchema)
        {
            if (courseSchema.CourseName is null) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeEmptyOrNull });
            if (courseSchema.InstructorId is null) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeEmptyOrNull });
            if (courseSchema.MaxUsers <= 0) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeEmptyOrNull });

            var course = new Course
            {
                Id = courseSchema.Id,
                CourseName = courseSchema.CourseName,
                MaxUsers = courseSchema.MaxUsers,
                Description = courseSchema.Description,
                MinAttendances = courseSchema.MinAttendances,
            };

            var result = await _coursesService.UpdateAsync(course);

            if (result is not null) return Ok(new BaseResponse<bool> { Data = true });

            return BadRequest(new BaseResponse<bool> { Data = false, Error = ResponseErrors.ServerDataBaseErrorUpdating });
        }





        [HttpPost]
        [Route("create")]
        [Authorize]
        public async Task<IActionResult> AddToCourse([FromBody] CourseSchema courseSchema)
        {
            if (courseSchema == null) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeSchemaEmpty });
            if (courseSchema.CourseName is null) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeNameEmpty });
            if (courseSchema.InstructorId is null) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeEmptyOrNull});

            if ((await _userService.GetAllAsync(i => i.Id == courseSchema.InstructorId && i.IsInstructor == true)).Count() < 1) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeIsInstructorFalse });

            if (courseSchema.MaxUsers <= 0) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeEmptyOrNull });


            var courses = await _coursesService.GetAllAsync(c => c.InstructorId == courseSchema.InstructorId && c.IsActive == true);

            if (courses.Count() > 0)
            {

                foreach (var course in courses)
                {

                    var findedHorarios = await _horariosService.GetAllAsync(h => h.CourseId == course.Id);


                    //delete data in response

                    if (IsScheduleConflict(findedHorarios, courseSchema.Horarios!)) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.CourseInstructorHindered });

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

                if (IsHorarioConflict(courseSchema.Horarios!)) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.CourseHorarioConfict });

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

                    if (newHorario == null)
                    {

                        return BadRequest(new BaseResponse<object> { Data = NewHorario, Error = ResponseErrors.ServerDataBaseErrorUpdating });
                    }

                    horarios.Add(newHorario);
                }


                //get course to set hangfire
                var course = await _coursesService.GetByIdAsync(NewCourse.Id);
                if (course == null) return NotFound();
                if (horarios == null) return NotFound();


                HangfireSetter(course, horarios);
                return Ok((new BaseResponse<bool> { Data = true }));
            }

            return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.ServerDataBaseErrorUpdating });
        }

        [HttpPut]
        [Route("endcourse/{id}")]
        [Authorize]
        public async Task<IActionResult> EndCourse(string Id)
        {

            if (string.IsNullOrEmpty(Id)) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeIdInvalidlFormat});

            //get course
            var endCourse = await _coursesService.GetByIdAsync(Id);

            List<string> list = new List<string>();


            //check course existence
            if (endCourse is not null)
            {
                //get inscriptions related with course
                var inscriptions = await _inscriptionsService.GetAllAsync(i =>
                i.CourseId == Id && 
                i.UnEnrolled == false);

                //verify inscriptions related with course
                if (inscriptions.Count() < 1) return NotFound(new BaseResponse<bool> { Data = false, Error = ResponseErrors.EntityNotExist });

                //deactivate course
                endCourse.IsActive = false;

                //try to update course status
                var endedResult = await _coursesService.UpdateAsync(endCourse);

                if (endedResult == null) return BadRequest(new BaseResponse<bool> { Data = false, Error = ResponseErrors.CourseCanNotEnd });

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

                return Ok(new BaseResponse<bool> { Data = true });




            }

            return NotFound(new BaseResponse<bool> { Data = false });
        }




        /*******************************inscriptions*******************************/

        [HttpPost]
        [Route("inscription/{courseId}/{studentId}")]
        [Authorize]
        public async Task<IActionResult> AddToCourse(string courseId, string studentId)
        {
            // First we have to check if the courseId and studentId, both exist on our database. Otherwise we shall return an error.

            if (await _userService.GetByIdAsync(studentId) is null) return NotFound(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.EntityNotExist });
            if (await _coursesService.GetByIdAsync(courseId) is null) return NotFound(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.EntityNotExist });


            var existingInscription = await _inscriptionsService.GetAllAsync(i => i.StudentId == studentId, i => i.Course!);

            foreach (var item in existingInscription)
            {

                if (item.Course!.IsActive) return BadRequest(new BaseResponse<bool> { Data = false, Error = ResponseErrors.InscriptionStudentAlredyInscripted });

            }

            //if (existingInscription.Any()) return BadRequest(new BaseResponse<bool> { Data = false, Error= ResponseErrors.InscriptionAlreadyExist });


            //
            var checkIfInCourse = await _inscriptionsService.GetAllAsync(i => i.CourseId == courseId && i.StudentId == studentId);

            if (checkIfInCourse.Any()) return BadRequest(new BaseResponse<bool> { Data = false, Error = ResponseErrors.InscriptionAlreadyExist });

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

            if (await _userService.GetByIdAsync(studentId) is null) return NotFound(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.EntityNotExist });
            if (await _coursesService.GetByIdAsync(courseId) is null) return NotFound(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.EntityNotExist });

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

            if (await _userService.GetByIdAsync(studentId) is null) return NotFound(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.EntityNotExist });

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

            if (await _userService.GetByIdAsync(studentId) is null) return NotFound(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.EntityNotExist });

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
                    CourseDescription = item.Course!.Description

                };

                data.Add(inscription);
            }

            if (result.Any()) return Ok(new BaseResponse<List<InscriptionDTO>> { Data = data });

            return NotFound(new BaseResponse<InscriptionDTO> { Data = null });
        }

        [HttpPut]
        [Route("inscription/unenrolled/{courseId}/{studentId}")]
        [Authorize]
        public async Task<IActionResult> UnenrolledStudent(string courseId, string studentId)
        {
            //check exists
            if (await _coursesService.GetByIdAsync(courseId) is null) return NotFound(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.EntityNotExist });

            if (await _userService.GetByIdAsync(studentId) is null) return NotFound(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.EntityNotExist });

            //check inscription
            var result = await _inscriptionsService.GetAllAsync(i => i.CourseId == courseId && i.StudentId == studentId);

            if (!result.Any()) return BadRequest(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.CourseNotFoundInscription });

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

            return BadRequest(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.ServerDataBaseError });
        }

        [HttpPut]
        [Route("inscription/switchstudents/{inscription1}/{inscription2}")]
        [Authorize]
        public async Task<IActionResult> TransferStudent(string inscription1, string inscription2)
        {
            //cehck format string
            if (string.IsNullOrEmpty(inscription1)) return NotFound(new BaseResponse<bool> { Error = ResponseErrors.AttributeIdInvalidlFormat });
            if (string.IsNullOrEmpty(inscription2)) return NotFound(new BaseResponse<bool> { Error = ResponseErrors.AttributeIdInvalidlFormat });

            //check exists


            //guardar en dos var ins1 y la 2
            var ins1 = await _inscriptionsService.GetByIdAsync(inscription1);
            if (ins1 is null) return NotFound(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.EntityNotExist });

            var ins2 = await _inscriptionsService.GetByIdAsync(inscription2);
            if (ins2 is null) return NotFound(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.EntityNotExist });


            //obtener todas las asistencias de ins1 por courso y alumno con attendances service getall(params)
            var ins1Attendances = await _attendancesService.GetAllAsync(a => a.StudentId == ins1.StudentId && a.CourseId == ins1.CourseId);
            if (!ins1Attendances.Any()) return NotFound(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.EntityNotExist });

            //obtener todas las asistencias de ins2 por courso y alumno con attendances service getall(params)
            var ins2Attendances = await _attendancesService.GetAllAsync(i => i.StudentId == ins2.StudentId && i.CourseId == ins2.CourseId);
            if (!ins1Attendances.Any()) return NotFound(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.EntityNotExist });

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

            if (await _userService.GetByIdAsync(studentId) is null) return NotFound(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.EntityNotExist });

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

            if (result.Any()) return Ok(new BaseResponse<List<InscriptionDTO>> { Data = data });



            return NotFound(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.EntityNotExist });
        }

        [HttpGet]
        [Route("inscription/getbycourse/{courseId}")]
        [Authorize]

        public async Task<IActionResult> GetInscriptionsByCourseAsync(string courseId)
        {

            if (!Guid.TryParse(courseId, out _)) return BadRequest(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.AttributeIdInvalidlFormat });

            var course = await _coursesService.GetByIdAsync(courseId);

            if (course == null) return NotFound(new BaseResponse<InscriptionDTO> { Error = ResponseErrors.EntityNotExist });

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

            return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.InscriptionNotAecredit });

        }




        [HttpDelete]
        [Route("inscription/remove/{courseId}/{studentId}")]
        [Authorize]
        public async Task<IActionResult> RemoveFromCourse(string courseId, string studentId)
        {
            // First we have to check if the courseId and studentId, both exist on our database. Otherwise we shall return an error.

            if (await _userService.GetByIdAsync(studentId) is null) return NotFound(new BaseResponse<bool> { Error = ResponseErrors.EntityNotExist });

            if (await _coursesService.GetByIdAsync(courseId) is null) return NotFound(new BaseResponse<bool> { Error = ResponseErrors.EntityNotExist });

            var checkIfInCourse = await _inscriptionsService.GetAllAsync(i => i.CourseId == courseId && i.StudentId == studentId);

            if (!checkIfInCourse.Any()) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.CourseNotFoundInscription });


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

            HorarioSchema lastHorario = null;

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

            Console.WriteLine($"horarios {horarios}");
            //assistancecheck recurrent job
            try
            {
                // Obtener la zona horaria de Ciudad de México
                TimeZoneInfo cdmxTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");

                // Obtener la fecha y hora actual en la zona horaria de Ciudad de México
                DateTime cdmxDateTime = TimeZoneInfo.ConvertTime(DateTimeOffset.Now.Date, cdmxTimeZone);

                DateTime datenow= DateTime.Now.Date;

                //para cada horario
                foreach (var horario in horarios)
                {
                    var day = horario.Day;
                    TimeSpan checkHour = horario.EndHour.Add(TimeSpan.FromHours(1));
                    string? cronExpresion = GetCronExpressionForDayAndTime(day!, checkHour.ToString(@"hh\:mm"));
                    Console.WriteLine($"cdmxDateTime {cdmxDateTime}");

                    if (cronExpresion != null)
                    {
                        var options = new RecurringJobOptions
                        {
                            TimeZone = cdmxTimeZone
                        };

                        RecurringJob.AddOrUpdate(
                            $"job-{day+"-"+course.Id!}",
                            () => CheckTodayAttendance(datenow, course.Id!, day!.ToLower(), course.StartDate, course.EndDate),
                            cronExpresion,
                            options
                        );
                    }
                    else
                    {
                        Console.WriteLine($"Not valid day: {day!.ToLower()} or var CronExpression null");
                    }

                    Console.WriteLine($"Task for cours: {course.Id} was created");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error dates {ex}");

            }

            //end course programed task
            try
            {
                BackgroundJob.Schedule(
                    () => EndCourseTask(course.Id!),
                    course.EndDate
                    );


            }
            catch
            {
                Console.WriteLine($"Error creating task end course automatically");
            }

            try
            {
                BackgroundJob.Schedule(
                    () => _hangfireJobsService.GenerateAllCartasAsync(course.Id!),
                    course.EndDate.AddDays(1)
                    );


            }
            catch
            {
                Console.WriteLine($"Error creating task GENERATE ALL CARTAS automatically");
            }
        }


        // Método sincrónico que llama al método asincrónico
        [ApiExplorerSettings(IgnoreApi = true)]
        public void CheckTodayAttendance(DateTime date, string courseId, string day, DateTime startDate, DateTime endDate)
        {
            CheckTodayAttendanceAsync(date, courseId, day, startDate, endDate).GetAwaiter().GetResult();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task CheckTodayAttendanceAsync(DateTime date, string courseId, string day, DateTime startDate, DateTime endDate)
        {

            if (date > startDate && date < endDate)
            {

                Console.WriteLine($"Executing VerifyAssistence to day: {day}");



                var Attendances = await _attendancesService.GetAllAsync(i =>
                    i.CourseId == courseId &&
                    i.Date == date);

                if (Attendances.Count() > 0)
                {
                    Console.WriteLine($"Intructor for course with {courseId} has taken attendances for day:{day}");
                }
                else
                {

                    Console.WriteLine($"Intructor for course with {courseId} has NOT TAKEN attendances for day:{day}");

                    // Obtener inscripciones para el curso
                    var inscriptions = await _inscriptionsService.GetAllAsync(i =>
                        i.CourseId == courseId);

                    if (!inscriptions.Any())
                    {
                        Console.WriteLine($"this course {courseId} has NOT inscriptions");
                    }

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
                                Console.WriteLine($"Generated attendance for student with exp {inscription.Student.Expediente} for course with id:{inscription.Course.Id}");
                            }
                            else
                            {
                                Console.WriteLine($"Student or Course information is missing for inscription with CourseId: {inscription.CourseId}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Can not generate attendance for student with exp {inscription.Student!.Expediente} for course with id:{inscription.Course!.Id}");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine($"Can not generate attendance for this course :{courseId} cause it ended or is inactive");
            }




        }
        //add another task to end course automatically when cousre reach his cours enddate -done

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task EndCourseTask(string CourseId)
        {
            

            //get course
            var endCourse = await _coursesService.GetByIdAsync(CourseId);

            List<string> list = new List<string>();


            //check course existence
            if (endCourse is not null)
            {
                //get inscriptions related with course
                var inscriptions = await _inscriptionsService.GetAllAsync(i => 
                i.CourseId == CourseId && 
                i.UnEnrolled == false);

                //verify inscriptions related with course
                if (inscriptions.Count() > 0)
                {
                    //deactivate course
                    endCourse.IsActive = false;

                    //try to update course status
                    var endedResult = await _coursesService.UpdateAsync(endCourse);

                    if (endedResult != null)
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

                            Console.WriteLine($"all the inscriptions ended correctly");

                            foreach (var element in list)
                            {
                                Console.WriteLine($"partial isncriptions ended, Can not end inscription for this user id: {element}");
                            }
                        }
                        else {
                            Console.WriteLine($"all the inscriptions ended correctly");
                        }

                    }
                    //return BadRequest(new BaseResponse<bool> { Data = false, Error = ResponseErrors.CourseCanNotEnd });



                }
                //return NotFound(new BaseResponse<bool> { Data = false, Error = ResponseErrors.EntityNotExist });
                Console.WriteLine($"There is no isncriptions for this course: {endCourse.Id}");






            }
            else {

                Console.WriteLine($"Can not find course with id provided");

            }



        }

        //
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
