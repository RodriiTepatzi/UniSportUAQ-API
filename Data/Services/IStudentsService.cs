using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;

namespace UniSportUAQ_API.Data.Services
{
	public interface IStudentsService
	{
        Task<Student?> GetStudentByEmailAsync(string email);
        Task<Student?> GetStudentByIdAsync(string id);

		Task<Student?> GetStudentByExpAsync(string exp);
		Task<List<Student>> GetAllInRangeAsync(int start, int end);
		Task<Student> CreateStudentAsync(StudentSchema studentSchema);
	}
}
