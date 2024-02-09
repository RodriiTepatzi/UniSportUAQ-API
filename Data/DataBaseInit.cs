using Microsoft.AspNetCore.Mvc.Diagnostics;
using UniSportUAQ_API.Data;

namespace UniSportUAQ_API.Data
{
    public class DataBaseInit
    {

        public static void FeedDataBase(ApplicationBuilder applicationBuilder) {

            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AppDbContext>();

                if(context.Students.Any())
                {
                    return;
                }
            }







        }
    }
}
