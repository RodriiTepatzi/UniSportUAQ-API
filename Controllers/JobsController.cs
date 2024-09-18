using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UniSportUAQ_API.Controllers
{
    [Route("api/v1/hangfire")]
    [ApiController]
    public class JobsController : Controller
    {
        [HttpPost]
        [Route("simpletask")]
        [AllowAnonymous]
        public ActionResult CreateBackgroundJob() {

            BackgroundJob.Enqueue(() => Console.WriteLine("Its working"));
            return Ok();
        
        }
    }
}
