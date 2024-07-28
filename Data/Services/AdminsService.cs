using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniSportUAQ_API.Data.Interfaces;
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
                IsAdmin = true,
                IsActive = true,
            };

            await _userManager.CreateAsync(admin, adminSchema.Password!);

            var entity = _context.Entry(admin);
            var result = entity.Entity;
            entity.State = EntityState.Added;

            await _context.SaveChangesAsync();

            return result;
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
				.Where(a => a.IsAdmin)
				.OrderBy(u => u.UserName)
				.Skip(start)
				.Take(range)
				.ToListAsync();
		}

        public async Task<List<ApplicationUser>> GetAdminSeacrhAsync(string searchTerm)
        {

            searchTerm.ToLower();

            var result = await _context.ApplicationUsers
                .Where(s =>
                (s.IsAdmin == true) &&
                (s.Name.ToLower().Contains(searchTerm) ||
                s.LastName.ToLower().Contains(searchTerm) ||
                s.Expediente.ToLower().Contains(searchTerm) ||
                s.Email.ToLower().Contains(searchTerm))
                )
                .ToListAsync();


            return result;
        }
    }
}

