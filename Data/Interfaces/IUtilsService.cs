using System.Runtime.CompilerServices;

namespace UniSportUAQ_API.Data.Interfaces
{
    public interface IUtilsService
    {
        Task<DateTime> GetServerDateAsync();
    }
}
