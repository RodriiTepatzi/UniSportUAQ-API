using Microsoft.EntityFrameworkCore;
using UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Data.Services
{
	public class InstructorsService : IInstructorsService
	{
		private readonly AppDbContext _context;
        public InstructorsService(AppDbContext context)
        {
			_context = context;
        }

        public Task<Instructor> CreateInstructorAsync(Instructor instructor)
		{
			throw new NotImplementedException();
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
