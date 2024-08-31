using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniSportUAQ_API.Data.Consts;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;
using UniSportUAQ_API.Data.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;

namespace UniSportUAQ_API.Controllers

{

    //api controller and route
    [ApiController]
    [Route("api/timeperiods")]

    //inheritance from Controller
    public class TimePeriodsController :Controller
    {
        private readonly ITimePeriodsService _timePeriodsService;

        //constructor
        public TimePeriodsController(ITimePeriodsService timePeriodsService)
        {
            _timePeriodsService = timePeriodsService;
        }




    }
}
