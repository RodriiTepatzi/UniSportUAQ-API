using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;

namespace UniSportUAQ_API.Data.Services
{
    public interface IAttendancesService
    {

        Task<Attendance> CreateAttendanceAsync(AttendanceSchema attendance);

        Task <Attendance?> GetAttendanceByIdAsync(string id);

        Task<Attendance?> GetAttendanceByStudentIdAsync(string studentId);

        Task<Attendance?> GetAttendanceByCourseIdAsync(string courseId);

        Task<Attendance?> GetAttendanceByDateAsync(DateTime day);

        Task<Attendance?> GetAttendancesAsync(string courseId, string studentId);


    }
}
