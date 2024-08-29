using UniSportUAQ_API.Data.Base;
using UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Data.Interfaces
{
    public interface ICartasLiberacionService : IEntityBaseRepository<CartaLiberacion>
    {
        Task<string?> UploadLetterAsync(Stream stream, string fileName);
    }
}
