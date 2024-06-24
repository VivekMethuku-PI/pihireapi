using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PiHire.API.Common;
using PiHire.API.Common.Hubs;
using PiHire.BAL.Common.Extensions;
using PiHire.BAL.IRepositories;
using PiHire.BAL.ViewModels;
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
    public class BGVController : BaseController
    {
        private readonly IHubContext<NotificationHub> hubContext;
        readonly ILogger<BGVController> logger;
        private readonly AppSettings _appSettings;
        private readonly IBGVRepository bgvRepository;
        private readonly INotificationRepository notificationRepository;

        public BGVController(ILogger<BGVController> logger,
            AppSettings appSettings,
            IBGVRepository bgvRepository, ILoggedUserDetails loginUserDetails, IHubContext<NotificationHub> hubContext, INotificationRepository notificationRepository) : base(bgvRepository, loginUserDetails, hubContext, notificationRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appSettings = appSettings;
            this.bgvRepository = bgvRepository;
            this.notificationRepository = notificationRepository;
            this.hubContext = hubContext;
        }



        [HttpGet("GetCandidate/Info")]
        public async Task<IActionResult> GetCandidateInfoAsync([FromQuery] string EmailId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await bgvRepository.GetCandidateInfo(EmailId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("UpdateCandidate/Info")]
        public async Task<IActionResult> UpdateCandidateInfoAsync(IFormCollection collection)
        {
            var updateCandidateInfoViewModel = new UpdateCandidateInfoViewModel();
            if (Request.Form != null)
            {
                updateCandidateInfoViewModel.ProfilePhoto = Request.Form.Files.FirstOrDefault(x => x.Name == "Photo");
            }

            await TryUpdateModelAsync(updateCandidateInfoViewModel);

            // Getting parameters
            var dict = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
            if (dict.Count > 0)
            {
                var CandidatesSkillViewModel = dict.Where(x => x.Key == "CandidatesSkillViewModel").ToList();
                //assigning skills
                if (CandidatesSkillViewModel.Count > 0)
                {
                    string Value = CandidatesSkillViewModel[0].Value;
                    if (!string.IsNullOrEmpty(Value))
                    {
                        updateCandidateInfoViewModel.CandidatesSkillViewModel = JsonConvert.DeserializeObject<List<GetCandidateSkillViewModel>>(Value);
                    }
                }

                // assing social reference 
                var CandidateSocialReferenceViewModel = dict.Where(x => x.Key == "CandidateSocialReferenceViewModel").ToList();
                if (CandidateSocialReferenceViewModel.Count > 0)
                {
                    string Value = CandidateSocialReferenceViewModel[0].Value;
                    if (!string.IsNullOrEmpty(Value))
                    {
                        updateCandidateInfoViewModel.CandidateSocialReferenceViewModel = JsonConvert.DeserializeObject<List<CandidateSocialReferenceViewModel>>(Value);
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await bgvRepository.UpdateCandidateInfo(updateCandidateInfoViewModel);
                if (response.Item1.Count > 0)
                {
                    await PushNotificationToClientAsync(response.Item1);
                }
                return Ok(response.Item2);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpGet("GetRecruiterDetails/{CanPrfId:int}/{jobId:int}")]
        public async Task<IActionResult> GetRecruiterDetailsAsync(int CanPrfId, int jobId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await bgvRepository.GetRecruiterDetails(CanPrfId, jobId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveCandidateBGVAsync(SaveCandidateBGVViewModel saveCandidateBGVViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await bgvRepository.SaveCandidateBGV(saveCandidateBGVViewModel);
                if (response.Item1.Count > 0)
                {
                    await PushNotificationToClientAsync(response.Item1);
                }
                return Ok(response.Item2);
            }
            catch (Exception)
            {
                throw;
            }
        }
     
        [HttpPost("SaveCandidateEmpBGV")]
        public async Task<IActionResult> SaveCandidateEmpBGVAsync(SaveCandidateBGVEmpViewModel saveCandidateBGVEmpViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await bgvRepository.SaveCandidateEmpBGV(saveCandidateBGVEmpViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("SaveCandidateEduBGV")]
        public async Task<IActionResult> SaveCandidateEduBGVAsync(SaveCandidateBGVEduViewModel saveCandidateBGVEduViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await bgvRepository.SaveCandidateEduBGV(saveCandidateBGVEduViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("{CanProfId:int}/{JobId:int}")]
        public async Task<IActionResult> GetCandidateBGVDtlsAsync(int CanProfId, int JobId)
        {
            try
            {
                var response = await bgvRepository.GetCandidateBGVDtls(CanProfId, JobId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        [HttpGet("GetCandidateEmpEduCertDtls/{CanProfId:int}")]
        public async Task<IActionResult> GetCandidateEmpEduCertDtlsAsync(int CanProfId)
        {
            try
            {
                var response = await bgvRepository.GetCandidateEmpEduCertVDtls(CanProfId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
       
        [HttpGet("DownloadCandidateBGVForm/{CandId:int}/{JobId:int}")]
        public async Task<IActionResult> DownloadCandidateBGVFormAsync(int CandId, int JobId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await bgvRepository.DownloadCandidateBGVForm(CandId, JobId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }      
       
        [HttpPost("DownloadCandidateAcknowledgementForm")]
        public async Task<IActionResult> DownloadCandidateAcknowledgementFormAsync(AcknowledgementDwndViewModel acknowledgementDwndViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await bgvRepository.DownloadCandidateAcknowledgementForm(acknowledgementDwndViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("AcceptCandidateBGV")]
        public async Task<IActionResult> AcceptCandidateBGVAsync(AcceptCandidateBGVViewModel acceptCandidateBGVViewModel)
        {
            try
            {
                var response = await bgvRepository.AcceptCandidateBGV(acceptCandidateBGVViewModel);
                if (response.Item1.Count > 0)
                {
                    await PushNotificationToClientAsync(response.Item1);
                }
                return Ok(response.Item2);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //[AllowAnonymous]
        [HttpGet("ConvertToEmployee/{CanProfId:int}/{JobId:int}")]
        public async Task<IActionResult> ConvertToEmployeeAsync(int CanProfId, int JobId)
        {
            try
            {
                var response = await bgvRepository.ConvertToEmployee(CanProfId, JobId);
                if (response.Item1.Count > 0)
                {
                    await PushNotificationToClientAsync(response.Item1);
                }
                return Ok(response.Item2);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //[AllowAnonymous]
        //[HttpGet("Odoologin")]
        //public async Task<IActionResult> OdoologinAsync()
        //{
        //    try
        //    {
        //        var response = await bgvRepository.Odoologin();               
        //        return Ok(response);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}


    }
}
