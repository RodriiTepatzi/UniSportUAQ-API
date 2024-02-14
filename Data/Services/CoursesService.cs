using Microsoft.EntityFrameworkCore;
using UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Data.Services
{
    public class CoursesService:ICoursesService
    {
        private readonly AppDbContext _context;

        public CoursesService(AppDbContext context) {
            _context = context;
        }

        public Task<Course> CreateCourseAsync(Course course) { 
            throw new NotImplementedException();
        }

        public async Task<List<Course>> GetCourseByIdAsync(string id) {

            var result = await _context.Courses.Where(
                i => i.Id == id)
                .ToListAsync();

            return result;
        }

        public async Task<List<Course>> GetCourseByNameAsync(string name)
        {

            var result = await _context.Courses.Where(
                i => i.CourseName == name)
                .ToListAsync();

            return result;
        }

        public async Task<List<Course>> GetCourseByIdInstructor(string Id) 
        {

            var result = await _context.Courses.Where(
               i => i.InstructorId == Id)
               .ToListAsync();

            return result;


        }
    }
}
