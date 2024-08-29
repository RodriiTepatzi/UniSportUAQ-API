using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniSportUAQ_API.Data.Base;
using UniSportUAQ_API.Data.Interfaces;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;
namespace UniSportUAQ_API.Data.Services
{
    public class AdminsService : EntityBaseRepository<ApplicationUser>, IAdminsService
    {


        public AdminsService(AppDbContext context) : base(context) { }



    }
}

