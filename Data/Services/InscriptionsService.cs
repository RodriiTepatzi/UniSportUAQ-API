using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Eventing.Reader;
using UniSportUAQ_API.Data.Base;
using UniSportUAQ_API.Data.Interfaces;
using UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Data.Services
{
    public class InscriptionsService : EntityBaseRepository<Inscription>, IInscriptionsService
	{
		public InscriptionsService(AppDbContext context) : base(context){ }
    }
}
