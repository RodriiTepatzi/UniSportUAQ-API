using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniSportUAQ_API.Data.Consts;
using UniSportUAQ_API.Data.Interfaces;
using UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Controllers
{

    [ApiController]
    [Route("api/utils")]
    public class UtilsController: Controller
    {
        private readonly IUtilsService _utilsService;

        public UtilsController(IUtilsService utilsService)
        {
            _utilsService = utilsService;
        }

        [HttpGet]
        [Route("serverdate")]
        [Authorize]
        
        public async Task<IActionResult> GetServerDate() {

            var data = await _utilsService.GetServerDateAsync();

            return Ok(new DataResponse { Data = data.ToString("s"), ErrorMessage = null });
        
        }
    }
}
