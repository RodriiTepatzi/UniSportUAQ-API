using UniSportUAQ_API.Data.Base;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;

namespace UniSportUAQ_API.Data.Interfaces
{
    public interface IStudentsService : IEntityBaseRepository <ApplicationUser>
    {
        Task<ApplicationUser?> GetStudentByEmailAsync(string email);
        Task<ApplicationUser?> GetStudentByIdAsync(string id);

        Task<ApplicationUser?> GetStudentByExpAsync(string exp);
        Task<List<ApplicationUser>> GetAllInRangeAsync(int start, int end);

        Task<List<ApplicationUser>> GetStudentsSeacrhAsync(string searchTerm);
        Task<ApplicationUser> CreateStudentAsync(StudentSchema studentSchema);
    }
}
