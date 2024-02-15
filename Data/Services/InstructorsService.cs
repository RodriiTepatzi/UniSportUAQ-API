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

        public async Task<Instructor> CreateInstructorAsync(InstructorSchema instructorSchema)
		{
			var student = new Instructor
			{
				UserName = instructorSchema.Expediente,
				Name = instructorSchema.Name,
				LastName = instructorSchema.LastName,
				Email = instructorSchema.Email,
				PhoneNumber = instructorSchema.PhoneNumber,
				Expediente = instructorSchema.Expediente,
			};

			await _userManager.CreateAsync(student, instructorSchema.Password!);

			return student;
		}

		public async Task<List<Instructor>> GetInstructorByIdAsync(string id)
		{
			var result = await _context.Instructors.Where(
				i => i.Id == id
				).ToListAsync();

			return result;
		}

		public async Task<List<Instructor>> GetInstructorByExpAsync(string exp)
		{
			var result = await _context.Instructors.Where(
					i => i.Expediente == exp
				).ToListAsync();

			return result;
		}

		public async Task<List<Instructor>> GetInstructorByEmailAsync(string email)
		{
			var result = await _context.Instructors.Where(
				i => i.Email == email)
				.ToListAsync();

			return result;
		}

		public async Task<List<Instructor>> GetAllInRangeAsync(int start, int end)
		{
			int range = end - start + 1;

			return await _context.Instructors
				.OrderBy(u => u.UserName)
				.Skip(start)
				.Take(range)
				.ToListAsync();
		}
	}
}
