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
using static PiHire.BAL.Common.Types.AppConstants;

namespace PiHire.API.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class WorkSheduleController : BaseController
    {
        private readonly IHubContext<NotificationHub> hubContext;
        readonly ILogger<WorkSheduleController> logger;
        private readonly AppSettings _appSettings;
        private readonly IWorkSheduleRepository workSheduleRepository;
        private readonly INotificationRepository notificationRepository;


        public WorkSheduleController(ILogger<WorkSheduleController> logger,
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
        public async Task<IActionResult> GetWorkShiftsAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await workSheduleRepository.GetWorkShifts();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetWorkShifAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await workSheduleRepository.GetWorkShift(id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateWorkShiftsAsync(CreateWorkShiftDtlsViewModel createWorkShiftDtlsViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await workSheduleRepository.CreateWorkShifts(createWorkShiftDtlsViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost("UpdateWorkShift")]
        public async Task<IActionResult> UpdateWorkShiftAsync(UpdateWorkShiftDtlsViewModel updateWorkShiftDtlsViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await workSheduleRepository.UpdateWorkShifts(updateWorkShiftDtlsViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpDelete("WorkShift/InActive/{Id:int}")]
        public async Task<IActionResult> UpdateWorkShiftToInActiveAsync(int Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await workSheduleRepository.UpdateWorkShiftStatus(Id,(byte)RecordStatus.Inactive);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }

        }


        [HttpDelete("WorkShift/Active/{Id:int}")]
        public async Task<IActionResult> UpdateWorkShiftToActiveAsync(int Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await workSheduleRepository.UpdateWorkShiftStatus(Id, (byte)RecordStatus.Active);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        
        [HttpPost("GetWorkScheduleDtls")]
        public async Task<IActionResult> GetWorkScheduleDtlsAsync(WorkScheduleSearchViewModel workScheduleSearchViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await workSheduleRepository.GetWorkScheduleDtls(workScheduleSearchViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
