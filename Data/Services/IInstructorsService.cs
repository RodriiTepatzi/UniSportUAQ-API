using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;

namespace UniSportUAQ_API.Data.Services
{
	public interface IInstructorsService
	{
		Task<Instructor> CreateInstructorAsync(InstructorSchema instructor);
		Task<List<Instructor>> GetInstructorByIdAsync(string id);
        Task<List<Instructor>> GetInstructorByExpAsync(string exp);
		Task<List<Instructor>> GetInstructorByEmailAsync(string email);
		Task<List<Instructor>> GetAllInRangeAsync(int start, int end);
	}
}
