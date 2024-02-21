using System;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;

namespace UniSportUAQ_API.Data.Services
{
	public interface IAdminsService
	{
		Task<List<Admin>> GetAdminByIdAsync(string id);
        Task<List<Admin>> GetAdminByExpAsync(string exp);
        Task<List<Admin>> GetAdminByEmailAsync(string email);
		Task<List<Admin>> GetAllInRangeAsync(int start, int end);

        Task<Admin> CreateAdminAsync(AdminSchema adminSchema);
    }
}

