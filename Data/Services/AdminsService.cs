using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;
namespace UniSportUAQ_API.Data.Services
{
	public class AdminsService : IAdminsService
	{
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminsService(AppDbContext context, UserManager<ApplicationUser> userManager)
		{
            _context = context;
            _userManager = userManager;
        }


        public async Task<ApplicationUser> CreateAdminAsync(AdminSchema adminSchema) { 
            

            var admin = new ApplicationUser
			{

                UserName = adminSchema.Expediente,
                Name = adminSchema.Name,
                LastName = adminSchema.LastName,
                Email = adminSchema.Email,
                PhoneNumber = adminSchema.PhoneNumber,
                Expediente = adminSchema.Expediente,
            };

            await _userManager.CreateAsync(admin, adminSchema.Password!);

            return admin;
        }

        public async Task<List<ApplicationUser>> GetAdminByEmailAsync(string email)
        {
            var result = await _context.ApplicationUsers.Where(
                    a => a.Email == email
                ).ToListAsync();

            return result;
        }

        public async Task<List<ApplicationUser>> GetAdminByExpAsync(string exp)
        {
            var result = await _context.ApplicationUsers.Where(
                    a => a.Expediente == exp
                ).ToListAsync();

            return result;
        }

        public async Task<List<ApplicationUser>> GetAdminByIdAsync(string id)
        {
            var result = await _context.ApplicationUsers.Where(
                    a => a.Id == id
                ).ToListAsync();

            return result;
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
	}
}

