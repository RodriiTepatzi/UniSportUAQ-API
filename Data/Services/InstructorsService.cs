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

    }
}
