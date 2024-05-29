namespace UniSportUAQ_API.Data.Services
{
    public class UtilsService:IUtilsService
    {


        public UtilsService() { 
        
        }
        public Task<DateTime?> GetServerDateAsync()
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"); // UTC-6
            DateTime utcTime = DateTime.UtcNow;

            // Convertir la hora UTC a la zona horaria "Central Standard Time" (UTC-6)
            DateTime utcMinusSixTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, timeZone);

            // Devolver la hora convertida como una tarea
            return Task.FromResult<DateTime?>(utcMinusSixTime);

        }

    }
}
