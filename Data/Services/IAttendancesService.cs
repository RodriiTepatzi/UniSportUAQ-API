using UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Data.Services
{
    public interface IAttendancesService
    {
        Task<Attendance?> GetAttendanceByIdAsync(string id);

        Task<List<Attendance>> GetAttendancesByCourseIdAsync(string idCourse);
        Task<List<Attendance>> GetAttendancesByStudentIdAsync(string studenId);

        Task<List<Attendance>> GetAttendancesAsync(string idCourse, string idStudent);

        Task<Attendance?> CreateAttendanceAsync(Attendance attendance);

        Task<Attendance?> UpDateAttedanceAsync(Attendance attendance);

 
    }
}
