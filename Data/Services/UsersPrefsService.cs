using UniSportUAQ_API.Data.Base;
using UniSportUAQ_API.Data.Interfaces;
using UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Data.Services
{
    public class UsersPrefsService:EntityBaseRepository<UserPreferences>, IUsersPrefsService
    {
        public UsersPrefsService(AppDbContext context):base(context) { }

    }
}
