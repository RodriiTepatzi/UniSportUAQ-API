using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniSportUAQ_API.Data.Base;
using UniSportUAQ_API.Data.Interfaces;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;

namespace UniSportUAQ_API.Data.Services
{
    public class StudentsService : EntityBaseRepository <ApplicationUser>, IStudentsService
    {
		private readonly AppDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;
        public StudentsService(AppDbContext context, UserManager<ApplicationUser> userManager) : base(context)
        {
			_context = context;
			_userManager = userManager;
        }

		public async Task<ApplicationUser> CreateStudentAsync(StudentSchema studentSchema)
		{
			var student = new ApplicationUser
			{
				UserName = studentSchema.Expediente,
				Name = studentSchema.Name,
				LastName = studentSchema.LastName,
				Email = studentSchema.Email,
				PhoneNumber = studentSchema.PhoneNumber,
				Expediente = studentSchema.Expediente,
				Group= studentSchema.Group,
				Semester = studentSchema.Semester,
				IsStudent = true,
				IsActive = true,
			};

			await _userManager.CreateAsync(student, studentSchema.Password!);

			var entity = _context.Entry(student);
			var result = entity.Entity;


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

		public async Task<ApplicationUser?> GetStudentByEmailAsync(string email)
		{
            try
            {
                var result = await _context.ApplicationUsers.SingleOrDefaultAsync(s => s.Email == email && s.IsStudent);
					

                if (result is not null) return result;
                else return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<ApplicationUser?> GetStudentByIdAsync(string id)
        {
			try
			{
				var result = await _context.ApplicationUsers
					.SingleAsync(i => i.Id == id && i.IsStudent);
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

        public async Task<ApplicationUser?> GetStudentByExpAsync(string exp)
        {
            try
            {
                var result = await _context.ApplicationUsers
					.SingleOrDefaultAsync(s => s.Expediente == exp && s.IsStudent);

                if (result is not null) return result;
                else return null;
            }
            catch{
                return null;
            }
        }

		public async Task<List<ApplicationUser>> GetStudentsSeacrhAsync(string searchTerm) {

			searchTerm.ToLower();

			var result = await _context.ApplicationUsers
				.Where(s => (s!.Name!.ToLower().Contains(searchTerm) ||
				s.LastName!.ToLower().Contains(searchTerm)||
				s.Expediente!.ToLower().Contains(searchTerm) ||
				s.Email!.ToLower().Contains(searchTerm)) 
				&& 
				(s.IsStudent == true))
				.ToListAsync();


            return result;
		}
    }
}
