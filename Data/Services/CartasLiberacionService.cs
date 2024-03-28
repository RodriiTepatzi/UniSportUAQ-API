

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Data.Services
{
    public class CartasLiberacionService: ICartasLiberacionService
    {
        private readonly AppDbContext _context;
        private readonly IStudentsService _studentsService;
        private readonly ICoursesService _coursesService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartasLiberacionService(AppDbContext context,  IStudentsService studentsService, ICoursesService coursesService, UserManager<ApplicationUser> userManager) { 

            _context = context;
            _studentsService = studentsService;
            _coursesService = coursesService;
            _userManager = userManager;


        }

        public async Task<CartaLiberacion?> GetCartaByIdAsync(string id) {

            try {

                var result = await _context.CartasLiberacion.Include(s => s.Student)
                    .Include(c => c.Course)
                    .SingleOrDefaultAsync(i => i.Id == id);

                if (result == null)
                {
                    return null;
                }

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

        public async Task<List<CartaLiberacion>> GetCartaByStudentIdAsync(string studentId)
        {
            var result = await _context.CartasLiberacion.Include(s => s.Student)
                     .Include(c => c.Course)
                     .Where(cart => cart.StudentId == studentId).ToListAsync();

            return result;
        }

        public async Task<List<CartaLiberacion>> GetCartaByCourseIdAsync(string courseId)
        {
            var result = await _context.CartasLiberacion.Include(s => s.Student)
                     .Include(c => c.Course)
                     .Where(cart => cart.CourseId == courseId).ToListAsync();

            return result;
        }

        public async Task<CartaLiberacion> CreateCartaAsync(CartaLiberacion cartaLiberacion)
        {
            var entity = _context.Entry(cartaLiberacion);
            var result = entity.Entity;

            entity.State = EntityState.Added;

            await _context.SaveChangesAsync();

            return result;

        }


    }
}
