using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PiHire.API.Common;
using PiHire.API.Common.Hubs;
using PiHire.BAL.Common.Extensions;
using PiHire.BAL.IRepositories;
using PiHire.BAL.ViewModels;
using PiHire.o_SD.Controllers;

namespace PiHire.API.Controllers
{    

    [EnableCors("CorsPolicy")]
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class LeaveController : BaseController
    {
        private readonly IHubContext<NotificationHub> hubContext;
        readonly ILogger<WorkSheduleController> logger;
        private readonly AppSettings _appSettings;
        private readonly IWorkSheduleRepository workSheduleRepository;
        private readonly INotificationRepository notificationRepository;


        public LeaveController(ILogger<WorkSheduleController> logger,
          AppSettings appSettings,
          IWorkSheduleRepository workSheduleRepository, ILoggedUserDetails loginUserDetails, IHubContext<NotificationHub> hubContext, INotificationRepository notificationRepository) : base(workSheduleRepository, loginUserDetails, hubContext, notificationRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appSettings = appSettings;
            this.workSheduleRepository = workSheduleRepository;
            this.notificationRepository = notificationRepository;
            this.hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetLeavesAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await workSheduleRepository.GetLeaves();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetLeaveAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await workSheduleRepository.GetLeave(id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateLeaveAsync(CreateLeaveViewModel createLeaveViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await workSheduleRepository.CreateLeave(createLeaveViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("Instead")]
        public async Task<IActionResult> CreateLeaveInsteadAsync(CreateLeaveInsteadViewModel createLeaveViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await workSheduleRepository.CreateLeaveInstead(createLeaveViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }



        [HttpPost("UpdateLeave")]
        public async Task<IActionResult> UpdateLeaveAsync(UpdateLeaveViewModel updateLeaveViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await workSheduleRepository.UpdateLeave(updateLeaveViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

      


    }
}
