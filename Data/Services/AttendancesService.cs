using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace UniSportUAQ_API.Data.Services

{
    public class AttendancesService : IAttendancesService
    {

        private readonly AppDbContext _context;
        private readonly ICoursesService _coursesService;
        private readonly IStudentsService _studentsService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AttendancesService(AppDbContext context, ICoursesService coursesService, IStudentsService studentsService, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;

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
                    .Include(c => c.Course).
					Include(c => c.Course)
					.ThenInclude(c => c.Instructor)
					.SingleAsync(i => i.Id == id);


                if (result == null)
                {
                    return null;
                }


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
				Include(c => c.Course)
				.ThenInclude(c => c.Instructor)
				.
				Where(att => att.CourseId == idCourse
                ).ToListAsync();

            return result;
        }

        public async Task<List<Attendance>> GetAttendancesByStudentIdAsync(string studenId) {

            var result = await _context.Attendances.Include(c => c.Course).
               Include(c => c.Student).
			   Include(c => c.Course)
			   .ThenInclude(c => c.Instructor).
			   Where(att => att.StudentId == studenId
               ).ToListAsync();

            return result;
        }

        public async Task<List<Attendance>> GetAttendancesAsync(string idCourse, string idStudent)
        {

            var result = await _context.Attendances.Include(c => c.Course).
                    Include(s => s.Student).
					Include(c => c.Course)
					.ThenInclude(c => c.Instructor)
					.Where(a => a.CourseId == idCourse &&
                    a.StudentId == idStudent
                ).ToListAsync();


            return result;
        }

        public async Task<Attendance?> CreateAttendanceAsync(Attendance attendance) {

            var CourseCheckDay = await _context.Courses.SingleOrDefaultAsync(
                    c => c.Id == attendance.CourseId
                );

            if (CourseCheckDay is null) return null;



            var entity = _context.Entry(attendance);

            var result = entity.Entity;

            entity.State = EntityState.Added;

            await _context.SaveChangesAsync();

            return result;
        }

        public async Task<Attendance?> UpDateAttedanceAsync(Attendance attendance) {

            //get entity

            EntityEntry entityEntry = _context.Entry(attendance);

            //modify the entity
            
            entityEntry.State = EntityState.Modified;

            await _context.SaveChangesAsync();

            var newAttendance = await GetAttendanceByIdAsync(attendance.Id!);

            if (newAttendance is not null) return newAttendance;

            return null;
        }






    }
}
