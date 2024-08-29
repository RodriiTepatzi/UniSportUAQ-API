using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniSportUAQ_API.Data.Base;
using UniSportUAQ_API.Data.Interfaces;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;

namespace UniSportUAQ_API.Data.Services
{
	public class InstructorsService : EntityBaseRepository<ApplicationUser>, IInstructorsService
	{


		public InstructorsService(AppDbContext context) : base(context) { }

	}
			
}
