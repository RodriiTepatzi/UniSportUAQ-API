using System;
using UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Data.Services
{
	public interface IUsersService
	{
		Task<List<ApplicationUser>> GetAllAsync();
	}
}

