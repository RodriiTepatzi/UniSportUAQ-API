using System;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;

namespace UniSportUAQ_API.Data.Interfaces
{
    public interface IAdminsService
    {
        Task<List<ApplicationUser>> GetAdminByIdAsync(string id);
        Task<List<ApplicationUser>> GetAdminByExpAsync(string exp);
        Task<List<ApplicationUser>> GetAdminByEmailAsync(string email);
        Task<List<ApplicationUser>> GetAllInRangeAsync(int start, int end);

        Task<List<ApplicationUser>> GetAdminSeacrhAsync(string searchTerm);


        Task<ApplicationUser> CreateAdminAsync(AdminSchema adminSchema);
    }
}

