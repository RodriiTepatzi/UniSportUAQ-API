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

		public async Task<List<ApplicationUser>> GetAllInRangeAsync(int start, int end)
		{
			int range = end - start + 1;

			return await _context.ApplicationUsers
				.OrderBy(u => u.UserName)
				.Skip(start)
				.Take(range)
				.ToListAsync();
		}

		public async Task<List<ApplicationUser>> GetUserByEmailAsync(string email)
		{
			return await _context.ApplicationUsers.Where(
				u => u.Email == email
			).ToListAsync();
		}
	}
}

