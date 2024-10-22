using UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Data.Interfaces
{
    public interface IHangfireJobsService
    {
        Task GenerateAllCartasAsync(string courseId);


    }
}
