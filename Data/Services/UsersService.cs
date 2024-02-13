using System;
using Microsoft.EntityFrameworkCore;
using UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Data.Services
{
	public class UsersService : IUsersService
	{
        private readonly AppDbContext _context;
		public UsersService(AppDbContext context)
		{
            _context = context;
		}

        public async Task<List<ApplicationUser>> GetAllAsync()
        {
            return await _context.ApplicationUsers.ToListAsync();
        }
    }
}

