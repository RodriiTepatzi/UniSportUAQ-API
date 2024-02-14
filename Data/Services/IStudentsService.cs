using UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Data.Services
{
	public interface IStudentsService
	{
		Task<List<Student>> GetStudentByEmailAsync(string email);

		Task<List<Student>> GetStudentByIdAsync(string id);
	}
}
