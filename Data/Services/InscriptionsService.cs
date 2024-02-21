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
                InInfo = true, //TODO: Check for this line in future cases.
                Accredit = false,
                CourseId = courseId,
                StudentId  = studentId,
            };

            var result = await _context.AddAsync(
                entity
            );

            await _context.SaveChangesAsync();

            return result.Entity;
        }
    }
}
