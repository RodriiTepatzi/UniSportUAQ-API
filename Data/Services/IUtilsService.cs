using System.Runtime.CompilerServices;

namespace UniSportUAQ_API.Data.Services
{
    public interface IUtilsService
    {
       Task<DateTime?> GetServerDateAsync();
    }
}
