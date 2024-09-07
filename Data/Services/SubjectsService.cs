using UniSportUAQ_API.Data.Base;
using UniSportUAQ_API.Data.Interfaces;
using UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Data.Services
{
    public class SubjectsService:EntityBaseRepository<Subject>, ISubjectsService
    {
        public SubjectsService(AppDbContext context ): base(context) { }
    }
}
