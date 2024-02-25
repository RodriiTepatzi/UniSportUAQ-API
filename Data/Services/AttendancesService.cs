using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;

namespace UniSportUAQ_API.Data.Services

{
    public class AttendancesService: IAttendancesService
    {

        private readonly AppDbContext _context;
        private readonly ICoursesService _coursesService;
        private readonly IStudentsService _studentsService;

        public AttendancesService(AppDbContext context, ICoursesService coursesService, IStudentsService studentsService)
        {
            _context = context;
            _coursesService = coursesService;
            _studentsService = studentsService;
        }

        public async Task<Attendance> CreateAttendanceAsync(AttendanceSchema attendanceSchema)
        {
            var attendance = new Attendance
            {
                Id = attendanceSchema.Id,
                StudentId = attendanceSchema.StudentId,
                CourseId = attendanceSchema.CourseId,
                AttendanceClass = attendanceSchema.AttendanceClass,
            };

            var entity = await _context.Attendances.AddAsync(attendance);
            var attendanceGen = entity.Entity;

            attendanceGen.Course = await _coursesService.GetCourseByIdAsync(attendanceSchema.CourseId!);
            attendanceGen.Student = await _studentsService.GetStudentByIdAsync(attendanceSchema.StudentId!); 


            return attendanceGen;
        }

        public async Task<List<Attendance>> GetAttendanceByIdAsync(string id)
        {
            return null;
        }

        public async Task<List<Attendance>> GetAttendanceByStudentIdAsync(string studentId)
        {
            return null;
        }

        public async Task<List<Attendance>> GetAttendanceByCourseIdAsync(string courseId)
        {
            return null;
        }

       public async Task<List<Attendance>> GetAttendanceByDayIdAsync(DateTime day)
        {
            return null;
        }

        public async Task<List<Attendance>> GetAttendanceForValidationAsync(string courseId, string studentId, DateTime day)
        {
            var result = await _context.Attendances.Where(

                att => att.CourseId == courseId && 
                att.StudentId == studentId &&
                att.Date == day
                ).ToListAsync();

            if (result is not null) return result;
            else return new List<Attendance>();

        }



    }
}
