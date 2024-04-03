﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;

namespace UniSportUAQ_API.Data.Services
{
	public class InstructorsService : IInstructorsService
	{
		private readonly AppDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;

		public InstructorsService(AppDbContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		public async Task<ApplicationUser> CreateInstructorAsync(InstructorSchema instructorSchema)
		{

			var instructor = new ApplicationUser
			{
				Id = instructorSchema.Id,
			};

			var trackedEntity = await _context.ApplicationUsers.SingleAsync(
				e => e.Id == instructor.Id && e.IsStudent
			);

			trackedEntity.IsInstructor = true;

			var result = await _context.SaveChangesAsync();

			return trackedEntity;

		}

		public async Task<ApplicationUser?> GetInstructorByIdAsync(string id)
		{
			try { 
				var result = await _context.ApplicationUsers
					.Include(a => a.Courses)
					.SingleAsync(i => i.Id == id && i.IsInstructor);

				return result;
			}
			catch
			{
				return null;
			}
		}

		public async Task<ApplicationUser?> GetInstructorByExpAsync(string exp)
		{
			try { 
				var result = await _context.ApplicationUsers
					.Include(a => a.Courses)
					.SingleAsync(i => i.Expediente == exp && i.IsInstructor);

				return result;
			}
			catch
			{
				return null;
			}
		}

		public async Task<ApplicationUser?> GetInstructorByEmailAsync(string email)
		{
			try
			{
				var result = await _context.ApplicationUsers
					.Include(a => a.Courses)
					.SingleAsync(i => i.Email == email && i.IsInstructor);

				return result;
			}
			catch
			{
				return null;
			}
		}

		public async Task<List<ApplicationUser>> GetAllInRangeAsync(int start, int end)
		{
			int range = end - start + 1;

			return await _context.ApplicationUsers
				.Where(a => a.IsInstructor)
				.Include(a => a.Courses)
				.OrderBy(u => u.UserName)
				.Skip(start)
				.Take(range)
				.ToListAsync();
		}

        public async Task<List<ApplicationUser>> GetInstructorSeacrhAsync(string searchTerm)
        {

            searchTerm.ToLower();

            var result = await _context.ApplicationUsers
                .Include(u => u.CurrentCourse)
                .ThenInclude(c => c.Course)
                .Where(s => 
				(s.IsStudent == true &&
                s.IsInstructor == true) && 
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
