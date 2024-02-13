using System;
using UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Data.Services
{
	public interface IAdminsService
	{
		Task<List<Admin>> GetAdminByIdAsync(string id);
        Task<List<Admin>> GetAdminByExpAsync(string exp);
        Task<List<Admin>> GetAdminByEmailAsync(string email);
    }
}

