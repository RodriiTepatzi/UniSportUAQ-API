using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using iTextSharp.text;
using iTextSharp.text.pdf;
using UniSportUAQ_API.Data.Models;
using Microsoft.Scripting.Interpreter;
using System.IO;

namespace UniSportUAQ_API.Data.Services
{
    public class CartasLiberacionService : ICartasLiberacionService
    {
        private static string ApiKey = "AIzaSyCy6D1jpRckWh_HN_AKrorYF4-UB0IdfKI";
        private static string Bucket = "unisport-uaq.appspot.com";
        private static string AuthEmail = "gmorales37@alumnos.uaq.mx";
        private static string AuthPasswrord = "Uni$portUaq";

        private readonly AppDbContext _context;
        private readonly IStudentsService _studentsService;
        private readonly ICoursesService _coursesService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartasLiberacionService(AppDbContext context, IStudentsService studentsService, ICoursesService coursesService, UserManager<ApplicationUser> userManager)
        {

            _context = context;
            _studentsService = studentsService;
            _coursesService = coursesService;
            _userManager = userManager;


        }

        public async Task<CartaLiberacion?> GetCartaByIdAsync(string id)
        {

            try
            {

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

        public async Task<string?> UploadLetterAsync(Stream stream, string fileName)
        {
            var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPasswrord);

            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            var cancellation = new CancellationTokenSource();

            var storage = new FirebaseStorage(
                Bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true //manejar la cancelación adecuadamente
                }
            );

            var storageReference = storage.Child("uploads").Child("user_letters").Child(fileName);

            try
            {
                // Subir el archivo
                var uploadTask = await storageReference.PutAsync(stream, cancellation.Token);


                // Intentar obtener el enlace de descarga público
                var link = await storageReference.GetDownloadUrlAsync();
                return link; // Devuelve el enlace de descarga público
            }
            catch (Exception ex)
            {
                
                return null; // Devolver una cadena vacía o manejar el error de manera diferente
            }
        }



    }
}
