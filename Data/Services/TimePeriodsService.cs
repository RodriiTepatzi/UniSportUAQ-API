using UniSportUAQ_API.Data.Base;
using UniSportUAQ_API.Data.Interfaces;
using UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Data.Services
{
    public class TimePeriodsService : EntityBaseRepository<TimePeriod>, ITimePeriodsService
    {
        public TimePeriodsService(AppDbContext context) : base(context) { }

    }
}
