using System;
using Microsoft.EntityFrameworkCore;
using UniSportUAQ_API.Data.Interfaces;
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

		public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
		{
			try
			{
				var result = await _context.ApplicationUsers.SingleAsync(u => u.Email == email);

				return result;
			}
			catch 
			{
				return null;
			}
		}

		public async Task<ApplicationUser?> GetUserById(string id) {

            try
            {
                var result = await _context.ApplicationUsers
                    .SingleOrDefaultAsync(i => i.Id == id);

				if (result == null) return null;

                var entity = _context.Entry(result);

                if (entity.State == EntityState.Unchanged)
                {
                    return entity.Entity;
                }
                else
                {
                    return entity.Entity;
                }
            }
            catch (InvalidOperationException)
            {
                return null;
            }

        }


        public async Task<ApplicationUser?> UpdateUserAsync(ApplicationUser user) {


			var result = await _context.ApplicationUsers.SingleOrDefaultAsync(u => u.Id == user.Id);

			if (result is null) return null;

			var entity = _context.Entry(result);

			
			entity.Entity.Name = user.Name;
			entity.Entity.LastName = user.LastName;
			entity.Entity.PhoneNumber = user.PhoneNumber;
			entity.Entity.Email = user.Email;
			entity.Entity.IsInFIF = user.IsInFIF;
			entity.Entity.Semester = user.Semester;
			entity.Entity.IsActive = user.IsActive;
			entity.Entity.IsAdmin = user.IsAdmin;
			entity.Entity.IsStudent = user.IsStudent;
			entity.Entity.IsInstructor = user.IsInstructor;
			entity.Entity.PictureUrl = user.PictureUrl;

			entity.State = EntityState.Modified;

			await _context.SaveChangesAsync();

			return result;
		}
    }
}

