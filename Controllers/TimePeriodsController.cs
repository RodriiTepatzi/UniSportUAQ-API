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
    [Route("api/v1/timeperiod")]
    public class TimePeriodsController: Controller
    {
        private readonly ITimePeriodsService _timePeriodsService;
        

        public TimePeriodsController(ITimePeriodsService timePeriodsService) {

            _timePeriodsService = timePeriodsService;


        }


        //get id
        [HttpGet]
        [Route("id/{id}")]
        [Authorize]

        public async Task<IActionResult> GetTimePeriodById(string id)
        {

            if (!Guid.TryParse(id, out _)) return BadRequest(new BaseResponse<TimePeriodDTO> { Error = ResponseErrors.DataNotFound });

            var result = await _timePeriodsService.GetByIdAsync(id);

            if (result == null) return Ok(new BaseResponse<TimePeriodDTO> { Error = ResponseErrors.DataNotFound });

            var timePeriodDTO = new TimePeriodDTO
            {
                Id = result.Id,
                Period = result.Period,
                Type = result.Type,
                DateStart = result.DateStart,
                DateEnd = result.DateEnd,
            };

            //change to dto
            return Ok(new BaseResponse<TimePeriodDTO> { Data = timePeriodDTO });
        }

        //get period
        [HttpGet]
        [Route("period/{period}")]
        [Authorize]
        public async Task<IActionResult> GetTimePeriodByPeriod(string period)
        {

            if (string.IsNullOrEmpty(period)) return BadRequest(new BaseResponse<List<TimePeriodDTO>> { Error = ResponseErrors.AttributeEmptyOrNull });

            var result = await _timePeriodsService.GetAllAsync(a => a.Period == period);


            var Data = new List<TimePeriodDTO>();

            if (result.Count() < 1) return Ok(new BaseResponse<bool> { Data = false, Error = ResponseErrors.EntityNotExist });

            foreach (var kvp in result)
            {
                var timePeriodDTO = new TimePeriodDTO
                {
                    Id = kvp.Id,
                    Period = kvp.Period,
                    Type = kvp.Type,
                    DateStart = kvp.DateStart,
                    DateEnd = kvp.DateEnd,
                };
                Data.Add(timePeriodDTO);
            }

            return Ok(new BaseResponse<List<TimePeriodDTO>> { Data = Data });
        }

        //get type
        [HttpGet]
        [Route("type/{type}")]
        [Authorize]

        public async Task<IActionResult> GetTimePeriodByType(string type)
        {

            if (string.IsNullOrEmpty(type)) return BadRequest(new BaseResponse<List<TimePeriodDTO>> { Error = ResponseErrors.AttributeEmptyOrNull });

            var result = await _timePeriodsService.GetAllAsync(a => a.Type == type);

            var Data = new List<TimePeriodDTO>();

            if (result.Count() < 1) return Ok(new BaseResponse<bool> { Data = false, Error = ResponseErrors.EntityNotExist });

            foreach (var kvp in result)
            {
                var timePeriodDTO = new TimePeriodDTO
                {
                    Id = kvp.Id,
                    Period = kvp.Period,
                    Type = kvp.Type,
                    DateStart = kvp.DateStart,
                    DateEnd = kvp.DateEnd,
                };

                Data.Add(timePeriodDTO);
            }

            return Ok(new BaseResponse<List<TimePeriodDTO>> { Data = Data});
        }


        [HttpGet]
        [Route("datestart/{date}")]
        [Authorize]

        public async Task<IActionResult> GetTimePeriodByDateStart(string date)
        {

            if (!DateTime.TryParse(date, out _)) return BadRequest(new BaseResponse<TimePeriodDTO> { Error = ResponseErrors.DataNotFound });

            DateTime dateTime = DateTime.Parse(date).Date;

            var result = await _timePeriodsService.GetAllAsync(a => a.DateStart.Date == dateTime);

            var Data = new List<TimePeriodDTO>();

            if (result.Count() < 1) return Ok(new BaseResponse<bool> { Data = false, Error = ResponseErrors.EntityNotExist });

            foreach (var kvp in result)
            {
                var timePeriodDTO = new TimePeriodDTO
                {
                    Id = kvp.Id,
                    Period = kvp.Period,
                    Type = kvp.Type,
                    DateStart = kvp.DateStart,
                    DateEnd = kvp.DateEnd,
                };

                Data.Add(timePeriodDTO);

            }

            return Ok(new BaseResponse<List<TimePeriodDTO>> { Data = Data });
        }

        //get date end
        //get date end
        [HttpGet]
        [Route("dateend/{date}")]
        [Authorize]

        public async Task<IActionResult> GetTimePeriodByDateEnd(string date)
        {

            if (!DateTime.TryParse(date, out _)) return BadRequest(new BaseResponse<TimePeriodDTO> { Error = ResponseErrors.DataNotFound });

            DateTime dateTime = DateTime.Parse(date).Date;

            var result = await _timePeriodsService.GetAllAsync(a => a.DateEnd.Date == dateTime);

            var Data = new List<TimePeriodDTO>();

            if (result.Count() < 1) return Ok(new BaseResponse<bool> { Data = false, Error = ResponseErrors.EntityNotExist });

            foreach (var kvp in result)
            {

                var timePeriodDTO = new TimePeriodDTO
                {
                    Id = kvp.Id,
                    Period = kvp.Period,
                    Type = kvp.Type,
                    DateStart = kvp.DateStart,
                    DateEnd = kvp.DateEnd,
                };

                Data.Add(timePeriodDTO);

            }

            return Ok(new BaseResponse<List<TimePeriodDTO>> { Data = Data });
        }

        //get current time period
        [HttpGet]
        [Route("isinscriptionperiod")]
        [Authorize]
        public async Task<IActionResult> GetCurrentTimePeriod(){

            var periods = await _timePeriodsService.GetAllAsync(i =>
                i.DateEnd > DateTime.Now &&
                i.DateStart <= DateTime.Now
            );

            if(periods.Any()) return Ok(new BaseResponse<bool> { Data = true });

            return Ok(new BaseResponse<bool> { Data = false });
        }


        //post 
        [HttpPost]
        [Route("create")]
        [Authorize]
        public async Task<IActionResult> CreateTimePeriod([FromBody] TimePeriodsSchema period)
        {

            //validate register attrributes
            if (period == null) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeSchemaEmpty });
            if (string.IsNullOrEmpty(period.Period)) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeNameEmpty });
            if (string.IsNullOrEmpty(period.Type)) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeTypeEmpty });
            //revew period start end no colition
            if (period.DateStart >= period.DateEnd) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.CourseHorarioConfict });


            //check exist
            var result = await _timePeriodsService.GetAllAsync(i => 
            i.Period!.ToLower() == period.Period.ToLower() ||
            i.DateStart < period.DateStart &&
            i.DateEnd > period.DateStart);

            if (result.Any())  return Ok(new BaseResponse<bool> { Error = ResponseErrors.CourseHorarioConfict });

            //if (result.Any()) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.ENTITY_EXISTS });

            var newPeriod = new TimePeriod
            {
                Id = Guid.NewGuid().ToString(),
                Period = period.Period,
                Type = period.Type,
                DateStart = period.DateStart,
                DateEnd = period.DateEnd,

            };

            var periodAdded = await _timePeriodsService.AddAsync(newPeriod);

            if (periodAdded  == null) return Ok(new BaseResponse<bool> { Error = ResponseErrors.AttributeEmptyOrNull });

            return Ok(new BaseResponse<bool> { Data = true });
        }

        //put

        [HttpPut]
        [Route("update")]
        [Authorize]
        public async Task<IActionResult> UpdateTimePeriodAsync([FromBody] TimePeriodsSchema timePeriodSchema)
        {
            //validation
            if (string.IsNullOrEmpty(timePeriodSchema.Id)) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeEmptyOrNull });
            if (string.IsNullOrEmpty(timePeriodSchema.Period)) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeEmptyOrNull });
            if (string.IsNullOrEmpty(timePeriodSchema.Type)) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeEmptyOrNull });

            //search exist entity and not conflict with other schedules
            var result = await _timePeriodsService.GetAllAsync(i =>
            i.Id != timePeriodSchema.Id &&
            i.Period!.ToLower() == timePeriodSchema.Period.ToLower() ||
            i.DateStart < timePeriodSchema.DateStart &&
            i.DateEnd > timePeriodSchema.DateStart);

            if (result.Any()) return Ok(new BaseResponse<bool> { Error = ResponseErrors.EntityExist});

            var oldTimePeriod = await _timePeriodsService.GetByIdAsync(timePeriodSchema.Id);

            if (oldTimePeriod == null) return Ok(new BaseResponse<bool> { Error = ResponseErrors.EntityNotExist });


            //update existing entity without creating a new instance
            oldTimePeriod.Period = timePeriodSchema.Period;
            oldTimePeriod.Type = timePeriodSchema.Type;
            oldTimePeriod.DateStart = timePeriodSchema.DateStart;
            oldTimePeriod.DateEnd = timePeriodSchema.DateEnd;

            //update entity
            var newPeriod = await _timePeriodsService.UpdateAsync(oldTimePeriod);

            if (newPeriod is null) return Ok(new BaseResponse<bool> { Error = ResponseErrors.SysErrorPromoting });

            return Ok(new BaseResponse<bool> { Data = true });
        }
        private bool IsScheduleConflict(DateTime existingStart, DateTime existingEnd, DateTime newStart, DateTime newEnd)
        {
                if (existingStart < newEnd && newStart < existingEnd)
                {
                    return true; // Conflict in schedule
                }   
            return false; // No  Conflict in schedule
        }
    }
}
