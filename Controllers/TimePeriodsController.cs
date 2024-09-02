using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UniSportUAQ_API.Data.Interfaces;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Services;
using UniSportUAQ_API.Data.Consts;
using System.Linq;

namespace UniSportUAQ_API.Controllers
{

    [ApiController]
    [Route("api/v1/timeperiod")]
    public class TimePeriodsController: Controller
    {
        private readonly ITimePeriodsService _timePeriodsService;

        public TimePeriodsController(ITimePeriodsService timePeriodsService) {

            this._timePeriodsService = timePeriodsService;

        }


        //get id
        [HttpGet]
        [Route("/id/{id}")]
        [Authorize]

        public async Task<IActionResult> GetTimePeriodById(string id) {

            if (!Guid.TryParse(id, out _)) return BadRequest(new DataResponse {Data= null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _timePeriodsService.GetByIdAsync(id);

            if(result == null) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND});

            //change to dto
            return Ok(new DataResponse { Data = result.Dictionary, ErrorMessage = null});
        }

        //get period
        [HttpGet]
        [Route("/period/{period}")]
        [Authorize]
        public async Task<IActionResult> GetTimePeriodByPeriod(string period) { 

            if(string.IsNullOrEmpty(period)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _timePeriodsService.GetAllAsync(a => a.Period == period);


            var Data = new List<Dictionary<string, object>>();

            if (result.Count() < 1)  return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            foreach (var kvp in result) {

                Data.Add(kvp.Dictionary);

            }

            return Ok(new DataResponse { Data = Data, ErrorMessage = null });
        }


        //get type
        [HttpGet]
        [Route("/type/{type}")]
        [Authorize]

        public async Task<IActionResult> GetTimePeriodByType(string type) {

            if (string.IsNullOrEmpty(type)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            
            var result =await  _timePeriodsService.GetAllAsync(a => a.Type == type);

            var Data = new List<Dictionary<string, object>>();

            if(result.Count() < 1) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            foreach (var kvp in result) {

                Data.Add(kvp.Dictionary);

            }

            return Ok(new DataResponse { Data = Data, ErrorMessage = null });
        }

        //get date start
        [HttpGet]
        [Route("/datestart")]
        [Authorize]

        public async Task<IActionResult> GetTimePeriodByDateStart(string date) { 

            if(!DateTime.TryParse(date, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = "date: "+ResponseMessages.BAD_REQUEST });

            DateTime dateTime = DateTime.Parse(date).Date;

            var result = await _timePeriodsService.GetAllAsync(a => a.DateStart.Date == dateTime);

            var Data = new List<Dictionary<string, object>>();

            if (result.Count() < 1) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            foreach (var kvp in result)
            {

                Data.Add(kvp.Dictionary);

            }

            return Ok(new DataResponse { Data = Data, ErrorMessage = null });

        }
        //get date end

        //post 
        //put
        //delete




    }
}
