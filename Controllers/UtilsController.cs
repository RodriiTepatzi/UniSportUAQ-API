using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniSportUAQ_API.Data.Base;
using UniSportUAQ_API.Data.Consts;
using UniSportUAQ_API.Data.Interfaces;
using UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Controllers
{

    [ApiController]
    [Route("api/v1/utils")]
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
        
        public IActionResult GetServerDate() {

            var data = _utilsService.GetServerDateAsync();

            return Ok(new BaseResponse<string> { Data = data.ToString("s"), Error = null });

        }
    }
}
