using  UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Data.Services
{
    public interface ICoursesService
    {

        Task<Course> CreateCourseAsync(Course course);

        Task<List<Course>> GetCourseByIdAsync(string id);

        Task<List<Course>> GetCourseByNameAsync(string name);

        Task<List<Course>> GetCourseByIdInstructor(string id_isntructor);


    }
}
