using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Data.Services
{
    public class CoursesService : ICoursesService
    {
        private readonly AppDbContext _context;

        public CoursesService(AppDbContext context) {
            _context = context;
        }

        public Task<Course> CreateCourseAsync(Course course) { 
            throw new NotImplementedException();
        }

        public async Task<Course?> GetCourseByIdAsync(string id) {

            try
            {
                var result = await _context.Courses.SingleAsync(
                i => i.Id == id);

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

        public async Task<List<Course>> GetCourseByNameAsync(string name)
        {

            var result = await _context.Courses.Where(
                i => i.CourseName == name)
                .ToListAsync();

            return result;
        }

        public async Task<List<Course>> GetCourseByIdInstructor(string instructorId) 
        {

            var result = await _context.Courses.Where(
               i => i.InstructorId == instructorId)
               .ToListAsync();

            return result;


        }

        public async Task<Course> UpdateCourseAsync(Course course)
        {
            EntityEntry entityEntry = _context.Entry(course);
            entityEntry.State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return course;
        }
    }
}
