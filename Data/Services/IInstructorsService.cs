using UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Data.Services
{
	public interface IInstructorsService
	{
		Task<Instructor> CreateInstructorAsync(Instructor instructor);
		Task<List<Instructor>> GetInstructorByIdAsync(string id);
        Task<List<Instructor>> GetInstructorByExpAsync(string id);
		Task<List<Instructor>> GetInstructorByEmailAsync(string email);
    }
}
