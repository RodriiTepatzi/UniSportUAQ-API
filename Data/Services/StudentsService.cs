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
				Group= studentSchema.Group,
				Semester = studentSchema.Semester,
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
                .Include(s => s.CurrentCourse)
				.ToListAsync();
		}

		public async Task<Student?> GetStudentByEmailAsync(string email)
		{
            try
            {
                var result = await _context.Students.SingleAsync(
					s => s.Email == email
				);

                if (result is not null) return result;
                else return null;
            }
            catch
            {
                return null;
            }
        }

		public async Task<Student?> GetStudentByIdAsync(string id)
        {
            try
            {
                var result = await _context.Students.SingleAsync(
					s => s.Id == id
                );

                if (result is not null) return result;
                else return null;
            }
            catch{
                return null;
            }
        }

        public async Task<Student?> GetStudentByExpAsync(string exp)
        {
            try
            {
                var result = await _context.Students.SingleAsync(
                    s => s.Expediente == exp
                );

                if (result is not null) return result;
                else return null;
            }
            catch{
                return null;
            }
        }
    }
}
