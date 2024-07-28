using System;
using UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Data.Interfaces
{
    public interface IUsersService
    {
        Task<List<ApplicationUser>> GetAllAsync();
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        Task<List<ApplicationUser>> GetAllInRangeAsync(int start, int end);

        Task<ApplicationUser?> GetUserById(string id);
        Task<ApplicationUser?> UpdateUserAsync(ApplicationUser user);
    }
}

