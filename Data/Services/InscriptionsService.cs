using Microsoft.EntityFrameworkCore;
using UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Data.Services
{
	public class InscriptionsService : IInscriptionsService
	{
		private readonly AppDbContext _context;
		public InscriptionsService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<bool> CheckInscriptionByCourseIdAndStudentIdAsync(string courseId, string studentId)
		{
			var result = await _context.Inscriptions.Where(
				i => i.CourseId == courseId && i.StudentId == studentId
			).ToListAsync();

			if (result.Count > 0) return true;

			return false;
		}

		public async Task<Inscription> CreateInscriptionAsync(string courseId, string studentId)
		{

			var entity = new Inscription
			{
				DateInscription = DateTime.Now,
				Accredit = false, // False as default.
				CourseId = courseId,
				StudentId = studentId,
			};

			var result = await _context.AddAsync(
				entity
			);

			await _context.SaveChangesAsync();

			return result.Entity;
		}

		public async Task<int> GetStudentCoursesCountAsync(string id)
		{
			var result = await _context.Inscriptions
				.Where(i => i.StudentId == id)
				.ToListAsync();

			return (int)result.Count;
		}

		public async Task<List<Inscription>> GetInscriptionsByStudentAsync(string id)
		{
			var result = await _context.Inscriptions
				.Where(i => i.StudentId == id && i.Accredit == false)
				.Include(i => i.Course)
				.ThenInclude(c => c.Instructor)
				.IgnoreAutoIncludes()
				.ToListAsync();

			return result;
		}


		public async Task<bool> RemoveInscriptionByCourseIdAndStudentIdAsync(string courseId, string studentId)
		{
			try
			{
				var result = await _context.Inscriptions.SingleAsync(
					i => i.CourseId == courseId && i.StudentId == studentId
				);

				var entity = _context.Entry(result);
				entity.State = EntityState.Deleted;

				await _context.SaveChangesAsync();

				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
