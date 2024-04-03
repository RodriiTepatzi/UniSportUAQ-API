using  UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Data.Services
{
    public interface ICoursesService
    {
		
        Task<Course> CreateCourseAsync(Course course);

        Task<Course?> GetCourseByIdAsync(string id);

		Task<List<Course>> GetAllCoursesAsync();
		Task<List<Course>> GetAllInactiveCoursesAsync();

		Task<List<Course>> GetCoursesByIdInstructor(string instructorId);
		Task<List<Course>> GetActivesCoursesByIdInstructor(string instructorId);
		Task<List<Course>> GetInactivesCoursesByIdInstructor(string instructorId);
		Task<List<Course>> GetCoursesSearch(string searchTerm);

		Task<Course> UpdateCourseAsync(Course course);

    }
}
