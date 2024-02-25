using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;

namespace UniSportUAQ_API.Data.Services
{
    public interface IAttendancesService
    {

        Task<Attendance> CreateAttendanceAsync(AttendanceSchema attendance);

        Task<List<Attendance>> GetAttendanceByIdAsync(string id);

        Task<List<Attendance>> GetAttendanceByStudentIdAsync(string studentId);

        Task<List<Attendance>> GetAttendanceByCourseIdAsync(string courseId);

        Task<List<Attendance>> GetAttendanceByDayAsync(DateTime day);

        Task<List<Attendance>> GetAttendanceForValidationAsync(string courseId, string studentId, DateTime day);


    }
}
