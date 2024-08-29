using UniSportUAQ_API.Data.Base;
using UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Data.Interfaces
{
    public interface IAttendancesService : IEntityBaseRepository<Attendance>
    {
		Task<Attendance?> CreateAttendanceAsync(Attendance attendance);

	}
}
