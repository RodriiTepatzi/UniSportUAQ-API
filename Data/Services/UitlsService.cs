namespace UniSportUAQ_API.Data.Services
{
    public class UitlsService:IUtilsService
    {


        public UitlsService() { 
        
        }

        public Task<DateTime?> GetServerDateAsync()
        {
            TimeZoneInfo mxZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)");
            DateTime utcTime = DateTime.UtcNow;
            DateTime mxTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, mxZone);
            return Task.FromResult<DateTime?>(mxTime);
        }
    }
}
