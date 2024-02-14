using Microsoft.EntityFrameworkCore;
using UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Data.Services
{
	public class StudentsService : IStudentsService
	{
		private readonly AppDbContext _context;
        public StudentsService(AppDbContext context)
        {
			_context = context;
        }

        public async Task<List<Student>> GetStudentByEmailAsync(string email)
		{
			var result = await _context.Students.Where(
				s => s.Email == email
			).
			ToListAsync();

			if (result is not null) return result;
			else return new List<Student>();
		}

		public async Task<List<Student>> GetStudentByIdAsync(string id){
		
			var result = await _context.Students.Where(
					s => s.Id == id
				).
				ToListAsync();

            if (result is not null) return result;
            else return new List<Student>();
        }
	}
}
