using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;

namespace UniSportUAQ_API.Data.Services
{
	public interface IInstructorsService
	{
		Task<ApplicationUser> CreateInstructorAsync(InstructorSchema instructor);
		Task<ApplicationUser?> GetInstructorByIdAsync(string id);
		Task<ApplicationUser?> GetInstructorByExpAsync(string exp);
		Task<ApplicationUser?> GetInstructorByEmailAsync(string email);
		Task<List<ApplicationUser>> GetAllInRangeAsync(int start, int end);
        Task<List<ApplicationUser>> GetInstructorSeacrhAsync(string searchTerm);

		Task<ApplicationUser?> UpdateInstructorAsync(ApplicationUser instructor);
    }
}
