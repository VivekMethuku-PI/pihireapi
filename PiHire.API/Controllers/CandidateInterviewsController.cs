using System;
using System.Linq;
using System.Threading.Tasks;
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
using PiHire.BAL.ViewModels;
using PiHire.o_SD.Controllers;

namespace PiHire.API.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class CandidateInterviewsController : BaseController
    {
        private readonly IHubContext<NotificationHub> hubContext;
        readonly ILogger<CandidateInterviewsController> logger;
        private readonly AppSettings _appSettings;
        private readonly ICandidateInterviewRepository interviewRepository;
        private readonly INotificationRepository notificationRepository;

        public CandidateInterviewsController(ILogger<CandidateInterviewsController> logger,
            AppSettings appSettings,
            ICandidateInterviewRepository interviewRepository, ILoggedUserDetails loginUserDetails, IHubContext<NotificationHub> hubContext, INotificationRepository notificationRepository) : base(interviewRepository, loginUserDetails, hubContext, notificationRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appSettings = appSettings;
            this.interviewRepository = interviewRepository;
            this.notificationRepository = notificationRepository;
            this.hubContext = hubContext;
        }


        [HttpPost("ReScheduleCandidateInterview")]
        public async Task<IActionResult> ReScheduleCandidateInterviewAsync(ReScheduleScheduleCandidateInterview reScheduleScheduleCandidateInterview)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await interviewRepository.ReScheduleCandidateInterview(reScheduleScheduleCandidateInterview);
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

        [HttpPost("CancelInterviewInvitation")]
        public async Task<IActionResult> CancelInterviewInvitationAsync(CancelCandidateInterviewInterview cancelCandidateInterviewInterview)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await interviewRepository.CancelInterviewInvitation(cancelCandidateInterviewInterview);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("GetClientCandidatePreferences/{JobId:int}/{CandProfId:int}")]
        public async Task<IActionResult> GetClientCandidatePreferencesAsync(int JobId, int CandProfId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await interviewRepository.GetClientCandidatePreferences(JobId, CandProfId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpPost("ScheduleCandidateInterview")]
        public async Task<IActionResult> ScheduleCandidateInterviewAsync(ScheduleCandidateInterview scheduleCandidateInterview)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await interviewRepository.ScheduleCandidateInterview(scheduleCandidateInterview);
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

        [HttpPost("UpdateCandidateInterview")]
        public async Task<IActionResult> UpdateCandidateInterviewAsync(UpdateCandidateInterviewModel updateCandidateInterviewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await interviewRepository.UpdateCandidateInterview(updateCandidateInterviewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpPost("RejectCandidateInterview")]
        public async Task<IActionResult> RejectCandidateInterviewAsync(CandidateInterviewRejectModel candidateInterviewRejectModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await interviewRepository.RejectCandidateInterview(candidateInterviewRejectModel);
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
        [HttpGet("GetClientSharedProfiles")]
        public async Task<IActionResult> GetClientSharedProfilesAsync([FromQuery] string BatchNo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await interviewRepository.GetClientSharedProfiles(BatchNo);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("ShareProfilesToClient")]
        public async Task<IActionResult> ShareProfilesToClientAsync(CandidateShareProfileViewModel candidateShareProfileViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await interviewRepository.ShareProfilesToClient(candidateShareProfileViewModel);
                return Ok(response);

            }
            catch (Exception)
            {
                throw;
            }
        }
       
        [HttpPost("GetCandidateInterviewDtlsToReSchedule")]
        public async Task<IActionResult> GetCandidateInterviewToReScheduleAsync(CandidateInterviewDtlsRequestViewModel candidateInterviewDtlsRequestViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await interviewRepository.GetCandidateInterviewDtlsToReSchedule(candidateInterviewDtlsRequestViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpPost("GetCandidateInterviewDtls")]
        public async Task<IActionResult> GetCandidateInterviewDtlsAsync(BacthProfilesModel bacthProfilesModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await interviewRepository.GetCandidateInterviewDtls(bacthProfilesModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpGet("ShortlistCandidate/{ShareId:int}")]
        public async Task<IActionResult> ShortlistCandidateAsync(int ShareId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await interviewRepository.ShortlistCandidate(ShareId);
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
        [HttpPost("UpdateClientCandidatePreference")]
        public async Task<IActionResult> UpdateClientCandidatePreferenceAsync(UpdateShareProfileTimeViewModel updateShareProfileTimeViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await interviewRepository.UpdateClientCandidatePreference(updateShareProfileTimeViewModel);
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
        [HttpGet("UpdateCandidateProfileClientViewStatus/{ShareId:int}")]
        public async Task<IActionResult> UpdateCandidateProfileClientViewStatusAsync(int ShareId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await interviewRepository.UpdateCandidateProfileClientViewStatus(ShareId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("mail/credExist")]
        public async Task<IActionResult> GetMailCredExistAsync()
        {
            try
            {
                var response = await interviewRepository.getMailCredExistAsync();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet("mail/{JobId:int}/{candidateId:int}")]
        public async Task<IActionResult> GetMailAsync(int JobId, int candidateId)
        {
            try
            {
                var response = await interviewRepository.getJobMailsAsync(JobId, candidateId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet("mail/{JobId:int}/{candidateId:int}/count")]
        public async Task<IActionResult> GetMailCountAsync(int JobId, int candidateId)
        {
            try
            {
                var response = await interviewRepository.getJobMailsAsync(JobId, candidateId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost("mail/{JobId:int}/{candidateId:int}")]
        public async Task<IActionResult> sendMailAsync(int JobId, int candidateId, IFormCollection collection)
        {
            var files = new System.Collections.Generic.List<Microsoft.AspNetCore.Http.IFormFile>();
            BAL.Common._3rdParty.Microsoft.SendMail.SendMail_ViewModel model = null;
            if (Request.Form != null)
            {
                foreach (var item in Request.Form.Files)
                {
                    files.Add(item);
                }
            }

            // Getting parameters
            var dict = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
            if (dict.Count > 0)
            {
                var data = dict.Where(x => x.Key == "data").ToList();
                if (data.Count > 0)
                {
                    string Value = data[0].Value;
                    if (!string.IsNullOrEmpty(Value))
                    {
                        model = Newtonsoft.Json.JsonConvert.DeserializeObject<BAL.Common._3rdParty.Microsoft.SendMail.SendMail_ViewModel>(Value);
                    }
                }
            }

            await TryUpdateModelAsync(model);
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await interviewRepository.sendJobMailsAsync(JobId, candidateId, model, files);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPut("mail/{JobId:int}/{candidateId:int}/{messageId}")]
        public async Task<IActionResult> replyMailAsync(int JobId, int candidateId, string messageId, IFormCollection collection)
        {
            var files = new System.Collections.Generic.List<Microsoft.AspNetCore.Http.IFormFile>();
            BAL.Common._3rdParty.Microsoft.ReplyMail.ReplyMail_ViewModel model = null;
            if (Request.Form != null)
            {
                foreach (var item in Request.Form.Files)
                {
                    files.Add(item);
                }
            }

            // Getting parameters
            var dict = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
            if (dict.Count > 0)
            {
                var data = dict.Where(x => x.Key == "data").ToList();
                if (data.Count > 0)
                {
                    string Value = data[0].Value;
                    if (!string.IsNullOrEmpty(Value))
                    {
                        model = Newtonsoft.Json.JsonConvert.DeserializeObject<BAL.Common._3rdParty.Microsoft.ReplyMail.ReplyMail_ViewModel>(Value);
                    }
                }
            }

            await TryUpdateModelAsync(model);
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await interviewRepository.replyJobMailsAsync(JobId, candidateId, messageId, model, files);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost("mail/{JobId:int}/{candidateId:int}/{messageId}")]
        public async Task<IActionResult> replyAllMailAsync(int JobId, int candidateId, string messageId, IFormCollection collection)
        {
            var files = new System.Collections.Generic.List<Microsoft.AspNetCore.Http.IFormFile>();
            BAL.Common._3rdParty.Microsoft.ReplyAllMail.ReplyAllMail_ViewModel model = null;
            if (Request.Form != null)
            {
                foreach (var item in Request.Form.Files)
                {
                    files.Add(item);
                }
            }

            // Getting parameters
            var dict = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
            if (dict.Count > 0)
            {
                var data = dict.Where(x => x.Key == "data").ToList();
                if (data.Count > 0)
                {
                    string Value = data[0].Value;
                    if (!string.IsNullOrEmpty(Value))
                    {
                        model = Newtonsoft.Json.JsonConvert.DeserializeObject<BAL.Common._3rdParty.Microsoft.ReplyAllMail.ReplyAllMail_ViewModel>(Value);
                    }
                }
            }

            await TryUpdateModelAsync(model);
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await interviewRepository.replyAllJobMailsAsync(JobId, candidateId, messageId, model, files);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("mail/{messageId}/readed")]
        public async Task<IActionResult> replyAllMailAsync(string messageId)
        {
            try
            {
                var response = await interviewRepository.setMailStatusReadedAsync(messageId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
