using System;
using Microsoft.EntityFrameworkCore;
using UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Data.Services
{
	public class AdminsService : IAdminsService
	{
        private readonly AppDbContext _context;

        public AdminsService(AppDbContext context)
		{
            _context = context;
		}

        public async Task<List<Admin>> GetAdminByEmailAsync(string email)
        {
            var result = await _context.Admins.Where(
                    a => a.Email == email
                ).ToListAsync();

            return result;
        }

        public async Task<List<Admin>> GetAdminByExpAsync(string exp)
        {
            var result = await _context.Admins.Where(
                    a => a.Expediente == exp
                ).ToListAsync();

            return result;
        }

        public async Task<List<Admin>> GetAdminByIdAsync(string id)
        {
            var result = await _context.Admins.Where(
                    a => a.Id == id
                ).ToListAsync();

            return result;
        }
    }
}

