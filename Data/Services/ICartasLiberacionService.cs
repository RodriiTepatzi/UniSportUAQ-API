using UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Data.Services
{
    public interface ICartasLiberacionService
    {
        Task<CartaLiberacion?> GetCartaByIdAsync(string id);

        Task<List<CartaLiberacion>> GetCartaByStudentIdAsync(string studentId);

        Task<List<CartaLiberacion>> GetCartaByCourseIdAsync(string courseId);

        Task <CartaLiberacion>CreateCartaAsync(CartaLiberacion cartaLiberacion);

        Task <string?> UploadLetterAsync(Stream stream, string fileName);








    }
}
