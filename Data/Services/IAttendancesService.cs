using System.Reflection.Metadata.Ecma335;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;

namespace UniSportUAQ_API.Data.Services
{
    public interface IAttendancesService
    {
        Task<Attendance?> GetAttendanceByIdAsync(string id);

        Task<List<Attendance>> GetAttendancesByCourseIdAsync(string idCourse);
        Task<List<Attendance>> GetAttendancesByStudentIdAsync(string studenId);

        Task<List<Attendance>> GetAttendancesAsync(string idCourse, string idStudent);

        Task<Attendance> CreateAttendanceAsync(Attendance attendance);

 
    }
}
