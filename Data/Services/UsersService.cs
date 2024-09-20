using System;
using Microsoft.EntityFrameworkCore;
using UniSportUAQ_API.Data.Base;
using UniSportUAQ_API.Data.Interfaces;
using UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Data.Services
{
    public class UsersService : EntityBaseRepository<ApplicationUser>, IUsersService
	{
       public UsersService(AppDbContext context): base(context) { }
        
		
    }
}

