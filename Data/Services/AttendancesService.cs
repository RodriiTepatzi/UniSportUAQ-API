using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;

namespace UniSportUAQ_API.Data.Services

{
    public class AttendancesService : IAttendancesService
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


        //return one object
        public async Task<Attendance?> GetAttendanceByIdAsync(string id)
        {
            try
            {
                var result = await _context.Attendances.Include(s => s.Student)
                    .Include(c => c.Course)
                    .SingleAsync(i => i.Id == id);

                var entity = _context.Entry(result);

                if (entity.State == EntityState.Unchanged)
                {
                    return entity.Entity;
                }
                else
                {
                    return entity.Entity;
                }

            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public async Task<List<Attendance>> GetAttendancesByCourseIdAsync(string idCourse) {

            var result = await _context.Attendances.Include(c => c.Course).
                Include(c => c.Student).
                Where(att => att.CourseId == idCourse
                ).ToListAsync();

            return result;
        }

        public async Task<List<Attendance>> GetAttendancesByStudentIdAsync(string studenId) {

            var result = await _context.Attendances.Include(c => c.Course).
               Include(c => c.Student).
               Where(att => att.StudentId == studenId
               ).ToListAsync();

            return result;
        }

        public async Task<List<Attendance>> GetAttendancesAsync(string idCourse, string idStudent)
        {

            var result = await _context.Attendances.Include(c => c.Course).
                    Include(s => s.Student)
                    .Where(a => a.CourseId == idCourse &&
                    a.StudentId == idStudent
                ).ToListAsync();


            return result;
        }

        




    }
}
