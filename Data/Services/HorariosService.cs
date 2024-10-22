using UniSportUAQ_API.Data.Base;
using UniSportUAQ_API.Data.Interfaces;
using UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Data.Services
{
    public class HorariosService : EntityBaseRepository<Horario>, IHorariosService
    {
        public HorariosService(AppDbContext context) : base(context) { }
    }
}
