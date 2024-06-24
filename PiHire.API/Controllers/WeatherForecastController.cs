using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PiHire.API.Common.Hubs;
using PiHire.API.Common.TimerFeatures;
using PiHire.BAL.Common.Logging;
using PiHire.BAL.ViewModels;

namespace PiHire.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private IHubContext<NotificationHub> _hub;
        readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IHubContext<NotificationHub> hub)
        {
            _hub = hub;
            this._logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            int Id = 5;
            _logger.LogDebug(ControllerContext.ActionDescriptor.ActionName, (int)LoggingEvents.GetItem, "/Id:" + Id);
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        //[HttpGet("ChartData")]
        //public IActionResult GetData()
        //{
        //    var timerManager = new TimerManager(() => _hub.Clients.All.SendAsync("transferchartdata", DataManager.GetData()));

        //    return Ok(new { Message = "Request Completed" });
        //}
    }
}
