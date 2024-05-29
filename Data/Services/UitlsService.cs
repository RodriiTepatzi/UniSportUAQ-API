namespace UniSportUAQ_API.Data.Services
{
    public class UitlsService:IUtilsService
    {


        public UitlsService() { 
        
        }
        public Task<DateTime?> GetServerDateAsync()
        {
            TimeZoneInfo utcMinusSixZone = TimeZoneInfo.CreateCustomTimeZone("UTC-06", TimeSpan.FromHours(-6), "UTC-06", "UTC-06");
            DateTime utcTime = DateTime.UtcNow;
            DateTime utcMinusSixTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, utcMinusSixZone);
            return Task.FromResult<DateTime?>(utcMinusSixTime);
        }

    }
}
