using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniSportUAQ_API.Data.Interfaces;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Consts;
using UniSportUAQ_API.Data.Schemas;
using UniSportUAQ_API.Data.Base;
using UniSportUAQ_API.Data.DTO;

namespace UniSportUAQ_API.Controllers
{

    [ApiController]
    [Route("api/v1/periods")]
    public class TimePeriodsController: Controller
    {
        private readonly ITimePeriodsService _timePeriodsService;
        
        public TimePeriodsController(ITimePeriodsService timePeriodsService) 
		{
            _timePeriodsService = timePeriodsService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCurrentTimePeriod(){

            var periods = await _timePeriodsService.GetAllAsync(i => i.IsActive == true);

            return Ok(new BaseResponse<IEnumerable<TimePeriod>> { Data = periods });
        }


        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> CreateTimePeriodAsync([FromBody] TimePeriodsSchema timePeriodSchema)
        {
            var result = await _timePeriodsService.GetAllAsync(i => i.IsActive);

			foreach (var item in result)
			{
				item.IsActive = false;
				await _timePeriodsService.UpdateAsync(item);
			}

			var timePeriod = new TimePeriod
			{
				DateEnd = timePeriodSchema.DateEnd,
				DateStart = timePeriodSchema.DateStart,
				IsActive = true,
			};

			var timePeriodResult = await _timePeriodsService.AddAsync(timePeriod);

			if (timePeriodResult == null) return BadRequest(new BaseResponse<bool> { Data = false, Error = ResponseErrors.ServerDataBaseErrorUpdating });

			return Ok(new BaseResponse<bool> { Data = true });
        }
    }
}
