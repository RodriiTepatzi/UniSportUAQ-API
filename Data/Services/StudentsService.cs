using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;

namespace UniSportUAQ_API.Data.Services
{
	public class StudentsService : IStudentsService
	{
		private readonly AppDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;
        public StudentsService(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
			_context = context;
			_userManager = userManager;
        }

		public async Task<Student> CreateStudentAsync(StudentSchema studentSchema)
		{
			var student = new Student
			{
				UserName = studentSchema.Expediente,
				Name = studentSchema.Name,
				LastName = studentSchema.LastName,
				Email = studentSchema.Email,
				PhoneNumber = studentSchema.PhoneNumber,
				Expediente = studentSchema.Expediente,
			};

			await _userManager.CreateAsync(student, studentSchema.Password!);

			return student;
		}

		public async Task<List<Student>> GetAllInRangeAsync(int start, int end)
		{
			int range = end - start + 1;

			return await _context.Students
				.OrderBy(u => u.UserName)
				.Skip(start)
				.Take(range)
				.ToListAsync();
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

        public async Task<List<Student>> GetStudentByExpAsync(string exp)
        {
            var result = await _context.Students.Where(
                s => s.Expediente == exp
            ).
            ToListAsync();

            if (result is not null) return result;
            else return new List<Student>();
        }
    }
}
