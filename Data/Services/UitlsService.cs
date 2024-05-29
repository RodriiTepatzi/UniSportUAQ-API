namespace UniSportUAQ_API.Data.Services
{
    public class UitlsService:IUtilsService
    {


        public UitlsService() { 
        
        }

       public async Task<DateTime?> GetServerDateAsync(){
             
             return await Task.FromResult<DateTime?>(DateTime.Now);
        }
    }
}
