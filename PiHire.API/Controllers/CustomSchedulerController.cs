using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PiHire.API.Common;
using PiHire.API.Common.Hubs;
using PiHire.BAL.Common.Extensions;
using PiHire.BAL.IRepositories;
using PiHire.BAL.Repositories;
using PiHire.o_SD.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PiHire.API.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomSchedulerController : BaseController
    {
        readonly ILogger<CustomSchedulerController> logger;
        private readonly AppSettings _appSettings;
        private readonly ICustomSchedulerRepository repository;

        public CustomSchedulerController(ILogger<CustomSchedulerController> logger,
            AppSettings appSettings,
            ICustomSchedulerRepository repository, ILoggedUserDetails loginUserDetails, IHubContext<NotificationHub> hubContext, INotificationRepository notificationRepository) : base(repository, loginUserDetails, hubContext, notificationRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appSettings = appSettings;
            this.repository = repository;
        }
        [HttpGet]
        public async Task<IActionResult> getSummaries()
        {
            return Ok(await repository.getSummariesAsync());
        }
        [HttpGet("{jobId}")]
        public async Task<IActionResult> getSummaries(int jobId)
        {
            return Ok(await repository.getSummariesAsync(jobId));
        }

        [HttpPost]
        public async Task<IActionResult> setBgTaskAsync([FromBody] BAL.ViewModels.BgJobSummaryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await repository.setCustomSchedulerAsync(model);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost("job")]
        public async Task<IActionResult> setJobBgTaskAsync([FromBody] BAL.ViewModels.CreateJobBgViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await repository.setCustomSchedulerAsync(model);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut("deactivate/{bgJobId}")]
        public async Task<IActionResult> deactivate(int bgJobId)
        {
            var response = await repository.deactiveCustomSchedulerAsync(bgJobId);
            return Ok(response);
        }
        [HttpPut("activate/{bgJobId}")]
        public async Task<IActionResult> activate(int bgJobId)
        {
            var response = await repository.activeCustomSchedulerAsync(bgJobId);
            return Ok(response);
        }
        [HttpDelete("{bgJobId}")]
        public async Task<IActionResult> delete(int bgJobId)
        {
            var response = await repository.deleteCustomSchedulerAsync(bgJobId);
            return Ok(response);
        }

    }
}
