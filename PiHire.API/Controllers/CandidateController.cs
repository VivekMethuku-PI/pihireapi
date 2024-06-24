using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
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
    public class CandidateController : BaseController
    {
        private readonly IHubContext<NotificationHub> hubContext;
        readonly ILogger<CandidateController> logger;
        private readonly AppSettings _appSettings;
        private readonly ICandidateRepository candidateRepository;
        private readonly INotificationRepository notificationRepository;

        public CandidateController(ILogger<CandidateController> logger,
            AppSettings appSettings,
            ICandidateRepository candidateRepository, ILoggedUserDetails loginUserDetails, IHubContext<NotificationHub> hubContext, INotificationRepository notificationRepository) : base(candidateRepository, loginUserDetails, hubContext, notificationRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appSettings = appSettings;
            this.candidateRepository = candidateRepository;
            this.notificationRepository = notificationRepository;
            this.hubContext = hubContext;
        }
        #region Candidate Portal
        [HttpGet("Portal/{Id:int}/Job/{JobId:int}")]
        public async Task<IActionResult> GetJobCandidateAsync(int Id, int JobId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.GetJobCandidateAsync(Id, JobId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPut("Portal/Job")]
        public async Task<IActionResult> UpdateCandidatePortalAsync(IFormCollection collection)
        {
            var updateCandidateViewModel = new UpdateJobCandidatePortalViewModel();
            if (Request.Form != null)
            {
                //updateCandidateViewModel.Photo = Request.Form.Files.FirstOrDefault(x => x.Name == "photo");
                updateCandidateViewModel.Resume = Request.Form.Files.FirstOrDefault(x => x.Name == "resume");
                updateCandidateViewModel.PaySlips = Request.Form.Files.Where(x => x.Name == "payslips").ToList();
                updateCandidateViewModel.CandVideoProfile = Request.Form.Files.FirstOrDefault(x => x.Name == "candVideoProfile");
            }
            await TryUpdateModelAsync(updateCandidateViewModel);
            //// Getting parameters
            var dict = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
            if (dict.Count > 0)
            {
                var candidateSkills = dict.Where(x => x.Key == "candidateSkills").ToList();
                //assigning skills
                if (candidateSkills.Count > 0)
                {
                    string Value = candidateSkills[0].Value;
                    if (!string.IsNullOrEmpty(Value))
                    {
                        updateCandidateViewModel.CandidateSkills = JsonConvert.DeserializeObject<List<UpdateJobCandidatePortalViewModel_skillRating>>(Value);
                    }
                }
                {
                    var JobDesirableSpecializations = dict.Where(x => x.Key == "jobDesirableSpecializations").ToList();
                    if (JobDesirableSpecializations.Count > 0)
                    {
                        string Value = JobDesirableSpecializations[0].Value;
                        if (!string.IsNullOrEmpty(Value))
                        {
                            updateCandidateViewModel.JobDesirableSpecializations = JsonConvert.DeserializeObject<List<UpdateJobCandidatePortalViewModel_skill>>(Value);
                        }
                    }

                    var JobDesirableImplementations = dict.Where(x => x.Key == "jobDesirableImplementations").ToList();
                    if (JobDesirableImplementations.Count > 0)
                    {
                        string Value = JobDesirableImplementations[0].Value;
                        if (!string.IsNullOrEmpty(Value))
                        {
                            updateCandidateViewModel.JobDesirableImplementations = JsonConvert.DeserializeObject<List<UpdateJobCandidatePortalViewModel_skill>>(Value);
                        }
                    }

                    var JobDesirableDesigns = dict.Where(x => x.Key == "jobDesirableDesigns").ToList();
                    if (JobDesirableDesigns.Count > 0)
                    {
                        string Value = JobDesirableDesigns[0].Value;
                        if (!string.IsNullOrEmpty(Value))
                        {
                            updateCandidateViewModel.JobDesirableDesigns = JsonConvert.DeserializeObject<List<UpdateJobCandidatePortalViewModel_skill>>(Value);
                        }
                    }

                    var JobDesirableDevelopments = dict.Where(x => x.Key == "jobDesirableDevelopments").ToList();
                    if (JobDesirableDevelopments.Count > 0)
                    {
                        string Value = JobDesirableDevelopments[0].Value;
                        if (!string.IsNullOrEmpty(Value))
                        {
                            updateCandidateViewModel.JobDesirableDevelopments = JsonConvert.DeserializeObject<List<UpdateJobCandidatePortalViewModel_skill>>(Value);
                        }
                    }

                    var JobDesirableSupports = dict.Where(x => x.Key == "jobDesirableSupports").ToList();
                    if (JobDesirableSupports.Count > 0)
                    {
                        string Value = JobDesirableSupports[0].Value;
                        if (!string.IsNullOrEmpty(Value))
                        {
                            updateCandidateViewModel.JobDesirableSupports = JsonConvert.DeserializeObject<List<UpdateJobCandidatePortalViewModel_skill>>(Value);
                        }
                    }

                    var JobDesirableQualities = dict.Where(x => x.Key == "jobDesirableQualities").ToList();
                    if (JobDesirableQualities.Count > 0)
                    {
                        string Value = JobDesirableQualities[0].Value;
                        if (!string.IsNullOrEmpty(Value))
                        {
                            updateCandidateViewModel.JobDesirableQualities = JsonConvert.DeserializeObject<List<UpdateJobCandidatePortalViewModel_skill>>(Value);
                        }
                    }

                    var JobDesirableDocumentations = dict.Where(x => x.Key == "jobDesirableDocumentations").ToList();
                    if (JobDesirableDocumentations.Count > 0)
                    {
                        string Value = JobDesirableDocumentations[0].Value;
                        if (!string.IsNullOrEmpty(Value))
                        {
                            updateCandidateViewModel.JobDesirableDocumentations = JsonConvert.DeserializeObject<List<UpdateJobCandidatePortalViewModel_skill>>(Value);
                        }
                    }
                }

                var CandidateQualificationModel = dict.Where(x => x.Key == "candidateQualificationModel").ToList();
                if (CandidateQualificationModel.Count > 0)
                {
                    string Value = CandidateQualificationModel[0].Value;
                    if (!string.IsNullOrEmpty(Value))
                    {
                        updateCandidateViewModel.CandidateQualificationModel = JsonConvert.DeserializeObject<List<UpdateJobCandidatePortalViewModel_CandidateEducation>>(Value);
                    }
                }
                var CandidateCertificationModel = dict.Where(x => x.Key == "candidateCertificationModel").ToList();
                if (CandidateCertificationModel.Count > 0)
                {
                    string Value = CandidateCertificationModel[0].Value;
                    if (!string.IsNullOrEmpty(Value))
                    {
                        updateCandidateViewModel.CandidateCertificationModel = JsonConvert.DeserializeObject<List<UpdateJobCandidatePortalViewModel_CandidateCertification>>(Value);
                    }
                }

                var OpeningQualifications = dict.Where(x => x.Key == "openingQualifications").ToList();
                if (OpeningQualifications.Count > 0)
                {
                    string Value = OpeningQualifications[0].Value;
                    if (!string.IsNullOrEmpty(Value))
                    {
                        updateCandidateViewModel.OpeningQualifications = JsonConvert.DeserializeObject<List<UpdateJobCandidatePortalViewModel_OpeningEducation>>(Value);
                    }
                }
                var OpeningCertifications = dict.Where(x => x.Key == "openingCertifications").ToList();
                if (OpeningCertifications.Count > 0)
                {
                    string Value = OpeningCertifications[0].Value;
                    if (!string.IsNullOrEmpty(Value))
                    {
                        updateCandidateViewModel.OpeningCertifications = JsonConvert.DeserializeObject<List<UpdateJobCandidatePortalViewModel_CandidateCertification>>(Value);
                    }
                }

                {
                    var CandidatePrefRegion = dict.Where(x => x.Key == "candidatePrefRegion").ToList();
                    if (CandidatePrefRegion.Count > 0)
                    {
                        string Value = CandidatePrefRegion[0].Value;
                        if (!string.IsNullOrEmpty(Value))
                        {
                            updateCandidateViewModel.CandidatePrefRegion = JsonConvert.DeserializeObject<UpdateJobCandidatePortalViewModel_value<int?>>(Value);
                        }
                    }

                    var JobDesirableDomain = dict.Where(x => x.Key == "jobDesirableDomain").ToList();
                    if (JobDesirableDomain.Count > 0)
                    {
                        string Value = JobDesirableDomain[0].Value;
                        if (!string.IsNullOrEmpty(Value))
                        {
                            updateCandidateViewModel.JobDesirableDomain = JsonConvert.DeserializeObject<UpdateJobCandidatePortalViewModel_value<int?>>(Value);
                        }
                    }

                    var JobDesirableCategory = dict.Where(x => x.Key == "jobDesirableCategory").ToList();
                    if (JobDesirableCategory.Count > 0)
                    {
                        string Value = JobDesirableCategory[0].Value;
                        if (!string.IsNullOrEmpty(Value))
                        {
                            updateCandidateViewModel.JobDesirableCategory = JsonConvert.DeserializeObject<UpdateJobCandidatePortalViewModel_value<int?>>(Value);
                        }
                    }

                    var JobDesirableTenure = dict.Where(x => x.Key == "jobDesirableTenure").ToList();
                    if (JobDesirableTenure.Count > 0)
                    {
                        string Value = JobDesirableTenure[0].Value;
                        if (!string.IsNullOrEmpty(Value))
                        {
                            updateCandidateViewModel.JobDesirableTenure = JsonConvert.DeserializeObject<UpdateJobCandidatePortalViewModel_value<int?>>(Value);
                        }
                    }

                    var JobDesirableWorkPattern = dict.Where(x => x.Key == "jobDesirableWorkPattern").ToList();
                    if (JobDesirableWorkPattern.Count > 0)
                    {
                        string Value = JobDesirableWorkPattern[0].Value;
                        if (!string.IsNullOrEmpty(Value))
                        {
                            updateCandidateViewModel.JobDesirableWorkPattern = JsonConvert.DeserializeObject<UpdateJobCandidatePortalViewModel_value<int?>>(Value);
                        }
                    }

                    var JobDesirableTeamRole = dict.Where(x => x.Key == "jobDesirableTeamRole").ToList();
                    if (JobDesirableTeamRole.Count > 0)
                    {
                        string Value = JobDesirableTeamRole[0].Value;
                        if (!string.IsNullOrEmpty(Value))
                        {
                            updateCandidateViewModel.JobDesirableTeamRole = JsonConvert.DeserializeObject<UpdateJobCandidatePortalViewModel_value<int?>>(Value);
                        }
                    }

                    var CandidatePrefLanguage = dict.Where(x => x.Key == "candidatePrefLanguage").ToList();
                    if (CandidatePrefLanguage.Count > 0)
                    {
                        string Value = CandidatePrefLanguage[0].Value;
                        if (!string.IsNullOrEmpty(Value))
                        {
                            updateCandidateViewModel.CandidatePrefLanguage = JsonConvert.DeserializeObject<UpdateJobCandidatePortalViewModel_value<int?>>(Value);
                        }
                    }

                    var CandidatePrefVisaPreference = dict.Where(x => x.Key == "candidatePrefVisaPreference").ToList();
                    if (CandidatePrefVisaPreference.Count > 0)
                    {
                        string Value = CandidatePrefVisaPreference[0].Value;
                        if (!string.IsNullOrEmpty(Value))
                        {
                            updateCandidateViewModel.CandidatePrefVisaPreference = JsonConvert.DeserializeObject<UpdateJobCandidatePortalViewModel_value<int?>>(Value);
                        }
                    }
                }
                

                //var PaySlipsModel = dict.Where(x => x.Key == "paySlipsModel").ToList();
                ////Assigning payslips
                //if (PaySlipsModel.Count > 0)
                //{
                //    string Value = PaySlipsModel[0].Value;
                //    if (!string.IsNullOrEmpty(Value))
                //    {
                //        updateCandidateViewModel.PaySlips = JsonConvert.DeserializeObject<List<IFormFile>>(Value);
                //    }
                //}
            }

            //ModelState.Remove("CurrEmplFlag");
            //ModelState.Remove("ValidPpflag");
            //ModelState.Remove("CurrDesignation");
            //ModelState.Remove("CandidateDOB");
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.UpdateJobCandidateAsync(updateCandidateViewModel);
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
        #endregion
        #region Candidate create & update

        [HttpPost]
        public async Task<IActionResult> CreateCandidateAsync(IFormCollection collection)
        {
            var createCandidateViewModel = new CreateCandidateViewModel();
            if (Request.Form != null)
            {
                createCandidateViewModel.Photo = Request.Form.Files.FirstOrDefault(x => x.Name == "photo");
                createCandidateViewModel.Resume = Request.Form.Files.FirstOrDefault(x => x.Name == "resume");
                createCandidateViewModel.PaySlips = Request.Form.Files.Where(x => x.Name == "payslips").ToList();
                createCandidateViewModel.CandVideoProfile = Request.Form.Files.FirstOrDefault(x => x.Name == "candVideoProfile");
            }

            await TryUpdateModelAsync(createCandidateViewModel);
            // Getting parameters
            var dict = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
            if (dict.Count > 0)
            {
                var CandidateSkillViewModel = dict.Where(x => x.Key == "createCandidateSkillViewModel").ToList();
                if (CandidateSkillViewModel.Count > 0)
                {
                    string Value = CandidateSkillViewModel[0].Value;
                    if (!string.IsNullOrEmpty(Value))
                    {
                        createCandidateViewModel.CreateCandidateSkillViewModel = JsonConvert.DeserializeObject<List<CreateCandidateSkillViewModel>>(Value);
                    }
                }

                var CandidateQualificationModel = dict.Where(x => x.Key == "candidateQualificationModel").ToList();
                if (CandidateQualificationModel.Count > 0)
                {
                    string Value = CandidateQualificationModel[0].Value;
                    if (!string.IsNullOrEmpty(Value))
                    {
                        createCandidateViewModel.CandidateQualificationModel = JsonConvert.DeserializeObject<List<CandidateQualificationModel>>(Value);
                    }
                }


                var CandidateCertificationModel = dict.Where(x => x.Key == "candidateCertificationModel").ToList();
                if (CandidateCertificationModel.Count > 0)
                {
                    string Value = CandidateCertificationModel[0].Value;
                    if (!string.IsNullOrEmpty(Value))
                    {
                        createCandidateViewModel.CandidateCertificationModel = JsonConvert.DeserializeObject<List<CandidateCertificationModel>>(Value);
                    }
                }

                var CandidateQuestionResponseModel = dict.Where(x => x.Key == "candidateQuestionResponseModel").ToList();
                if (CandidateQuestionResponseModel.Count > 0)
                {
                    string Value = CandidateQuestionResponseModel[0].Value;
                    if (!string.IsNullOrEmpty(Value))
                    {
                        createCandidateViewModel.CandidateQuestionResponseModel = JsonConvert.DeserializeObject<List<CandidateQuestionResponseModel>>(Value);
                    }
                }


                var PaySlipsModel = dict.Where(x => x.Key == "paySlipsModel").ToList();
                //assigning payslips
                if (PaySlipsModel.Count > 0)
                {
                    string Value = PaySlipsModel[0].Value;
                    if (!string.IsNullOrEmpty(Value))
                    {
                        createCandidateViewModel.PaySlipsModel = JsonConvert.DeserializeObject<List<PaySlipsModel>>(Value);
                    }
                }
            }

            ModelState.Remove("CurrEmplFlag");
            ModelState.Remove("ValidPpflag");
            ModelState.Remove("CurrDesignation");
            ModelState.Remove("CandidateDOB");
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.CreateCandidate(createCandidateViewModel);
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

        [HttpPut]
        public async Task<IActionResult> UpdateCandidateAsync(IFormCollection collection)
        {
            var updateCandidateViewModel = new UpdateCandidateViewModel();
            if (Request.Form != null)
            {
                updateCandidateViewModel.Photo = Request.Form.Files.FirstOrDefault(x => x.Name == "photo");
                updateCandidateViewModel.Resume = Request.Form.Files.FirstOrDefault(x => x.Name == "resume");
                updateCandidateViewModel.PaySlips = Request.Form.Files.Where(x => x.Name == "payslips").ToList();
                updateCandidateViewModel.CandVideoProfile = Request.Form.Files.FirstOrDefault(x => x.Name == "candVideoProfile");
            }
            await TryUpdateModelAsync(updateCandidateViewModel);
            // Getting parameters
            var dict = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
            if (dict.Count > 0)
            {
                var CandidateSkillViewModel = dict.Where(x => x.Key == "updateCandidateSkillViewModel").ToList();
                //assigning skills
                if (CandidateSkillViewModel.Count > 0)
                {
                    string Value = CandidateSkillViewModel[0].Value;
                    if (!string.IsNullOrEmpty(Value))
                    {
                        updateCandidateViewModel.UpdateCandidateSkillViewModel = JsonConvert.DeserializeObject<List<UpdateCandidateSkillViewModel>>(Value);
                    }
                }

                var CandidateQualificationModel = dict.Where(x => x.Key == "candidateQualificationModel").ToList();
                if (CandidateQualificationModel.Count > 0)
                {
                    string Value = CandidateQualificationModel[0].Value;
                    if (!string.IsNullOrEmpty(Value))
                    {
                        updateCandidateViewModel.CandidateQualificationModel = JsonConvert.DeserializeObject<List<CandidateQualificationModel>>(Value);
                    }
                }


                var CandidateCertificationModel = dict.Where(x => x.Key == "candidateCertificationModel").ToList();
                if (CandidateCertificationModel.Count > 0)
                {
                    string Value = CandidateCertificationModel[0].Value;
                    if (!string.IsNullOrEmpty(Value))
                    {
                        updateCandidateViewModel.CandidateCertificationModel = JsonConvert.DeserializeObject<List<CandidateCertificationModel>>(Value);
                    }
                }

                var CandidateQuestionResponseModel = dict.Where(x => x.Key == "candidateQuestionResponseModel").ToList();
                if (CandidateQuestionResponseModel.Count > 0)
                {
                    string Value = CandidateQuestionResponseModel[0].Value;
                    if (!string.IsNullOrEmpty(Value))
                    {
                        updateCandidateViewModel.CandidateQuestionResponseModel = JsonConvert.DeserializeObject<List<CandidateQuestionResponseModel>>(Value);
                    }
                }

                var PaySlipsModel = dict.Where(x => x.Key == "paySlipsModel").ToList();
                //Assigning payslips
                if (PaySlipsModel.Count > 0)
                {
                    string Value = PaySlipsModel[0].Value;
                    if (!string.IsNullOrEmpty(Value))
                    {
                        updateCandidateViewModel.PaySlipsModel = JsonConvert.DeserializeObject<List<PaySlipsModel>>(Value);
                    }
                }
            }

            ModelState.Remove("CurrEmplFlag");
            ModelState.Remove("ValidPpflag");
            ModelState.Remove("CurrDesignation");
            ModelState.Remove("CandidateDOB");
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.UpdateCandidate(updateCandidateViewModel);
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

        #endregion

        #region Canidate listing action

        [HttpPost("CandidateList")]
        public async Task<IActionResult> CandidateListAsync(CandidateListSearchViewModel candidateListSearchViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                if (candidateListSearchViewModel.JobId == null || candidateListSearchViewModel.JobId == 0)
                {
                    var response = await candidateRepository.CandidateList(candidateListSearchViewModel);
                    return Ok(response);
                }
                else
                {
                    var response = await candidateRepository.JobCandidateList(candidateListSearchViewModel);
                    return Ok(response);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet("CandidateListFilterData")]
        public async Task<IActionResult> CandidateListFilterDataAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.JobCandidateListFilterData();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet("CandidateListFilterData/{JobId:int}")]
        public async Task<IActionResult> CandidateListFilterDataAsync(int JobId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.JobCandidateListFilterData(JobId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet("SuggestCandidateListFilterData/{JobId:int}")]
        public async Task<IActionResult> SuggestCandidateListFilterDataAsync(int JobId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.SuggestCandidateListFilterData(JobId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("SuggestCandidateList")]
        public async Task<IActionResult> SuggestCandidateListAsync(SuggestCandidateListSearchViewModel suggestCandidateListSearchViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.SuggestCandidateList(suggestCandidateListSearchViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("MapCandidateToJob")]
        public async Task<IActionResult> MapCandidateToJobAsync(MapCandidateViewModel mapCandidateViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.MapCandidateToJob(mapCandidateViewModel);
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

        [HttpPost("UpdateCandidatePrfStatus")]
        public async Task<IActionResult> UpdateCandidatePrfStatusAsync(UpdateCandidatePrfStatusViewModel updateCandidatePrfStatusViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.UpdateCandidatePrfStatus(updateCandidatePrfStatusViewModel);
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

        [HttpPost("UpdateCandidateCVStatus")]
        public async Task<IActionResult> UpdateCandidateCVStatusAsync(UpdateCandidateCVStatusViewModel updateCandidateCVStatusViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.UpdateCandidatePrfStatus(updateCandidateCVStatusViewModel);
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

        [HttpPost("UpdateCandidateStatus")]
        public async Task<IActionResult> UpdateCandidateStatusAsync(CandidateOtherStatusViewModel candidateOtherStatusViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.UpdateCandidateOtherStatus(candidateOtherStatusViewModel);
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

        [HttpPost("CandidateBackout")]
        public async Task<IActionResult> CandidateBackoutAsync(CandidateOtherStatusViewModel candidateOtherStatusViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.UpdateCandidateOtherStatus(candidateOtherStatusViewModel);
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

        [HttpPost("CandidatesExportToExcel")]
        public async Task<IActionResult> CandidatesExportToExcelAsync(CandidateExportSearchViewModel candidateExportSearchViewModel)
        {
            try
            {
                string WebRootFolder = Path.GetTempPath();
                var response = await candidateRepository.GetExportCandidates(candidateExportSearchViewModel);
                // filename with customer Id and timestamp 
                string FileName = DateTime.UtcNow.ToString("dd_MM_yyyy_hh_mm_ss_fff") + ".xlsx";

                var memory = new MemoryStream();
                // creating dynamic file to append data 
                using (var fs = new FileStream(Path.Combine(WebRootFolder, FileName.Replace(" ", "_")), FileMode.Create, FileAccess.Write))
                {
                    IWorkbook workbook;
                    workbook = new XSSFWorkbook();
                    XSSFCellStyle txtStyle = (XSSFCellStyle)workbook.CreateCellStyle();
                    XSSFDataFormat dataFormat = (XSSFDataFormat)workbook.CreateDataFormat();
                    txtStyle.SetDataFormat(dataFormat.GetFormat("@"));

                    XSSFCellStyle numStyle = (XSSFCellStyle)workbook.CreateCellStyle();
                    numStyle.SetDataFormat(dataFormat.GetFormat("00"));

                    // sheet one  
                    var CandidateSheet = workbook.CreateSheet("Candidates");

                    var rowIndex = 0;
                    var CandidateSheetHeaderRow = CandidateSheet.CreateRow(rowIndex);
                    XSSFCellStyle headerStyle = (XSSFCellStyle)workbook.CreateCellStyle();
                    CandidateSheetHeaderRow.RowStyle = headerStyle;
                    headerStyle.WrapText = false;

                    CandidateSheetHeaderRow.CreateCell(0).SetCellValue("CandProfID");
                    CandidateSheetHeaderRow.CreateCell(1).SetCellValue("CandName");
                    CandidateSheetHeaderRow.CreateCell(2).SetCellValue("EmailID");
                    CandidateSheetHeaderRow.CreateCell(3).SetCellValue("SourceID");
                    CandidateSheetHeaderRow.CreateCell(4).SetCellValue("NoticePeriod");
                    CandidateSheetHeaderRow.CreateCell(5).SetCellValue("Gender");
                    CandidateSheetHeaderRow.CreateCell(6).SetCellValue("ContactNo");
                    CandidateSheetHeaderRow.CreateCell(7).SetCellValue("Experience");
                    CandidateSheetHeaderRow.CreateCell(8).SetCellValue("RelevantExperience");
                    CandidateSheetHeaderRow.CreateCell(9).SetCellValue("ExperienceInMonths");
                    CandidateSheetHeaderRow.CreateCell(10).SetCellValue("ReleExpeInMonths");
                    CandidateSheetHeaderRow.CreateCell(11).SetCellValue("CPCurrency");
                    CandidateSheetHeaderRow.CreateCell(12).SetCellValue("CPTakeHomeSalPerMonth");
                    CandidateSheetHeaderRow.CreateCell(13).SetCellValue("CPGrossPayPerAnnum");
                    CandidateSheetHeaderRow.CreateCell(14).SetCellValue("CurrOrganization");
                    CandidateSheetHeaderRow.CreateCell(15).SetCellValue("CurrLocation");
                    CandidateSheetHeaderRow.CreateCell(16).SetCellValue("SelfRating");
                    CandidateSheetHeaderRow.CreateCell(17).SetCellValue("DOB");
                    CandidateSheetHeaderRow.CreateCell(18).SetCellValue("MaritalStatus");
                    CandidateSheetHeaderRow.CreateCell(19).SetCellValue("EPCurrency");
                    CandidateSheetHeaderRow.CreateCell(20).SetCellValue("EPTakeHomePerMonth");

                    // Adding Customer Details to sheet one 
                    rowIndex = 0;
                    for (int i = 0; i < response.Result.Count; i++)
                    {
                        rowIndex += 1;
                        var CandidateSheetDtls = CandidateSheet.CreateRow(rowIndex);
                        XSSFCellStyle headerStyleData = (XSSFCellStyle)workbook.CreateCellStyle();
                        CandidateSheetDtls.RowStyle = headerStyleData;
                        headerStyle.WrapText = false;

                        CandidateSheetDtls.CreateCell(0).SetCellValue(response.Result[i].CandProfID.ToString());
                        CandidateSheetDtls.CreateCell(1).SetCellValue(response.Result[i].CandName);
                        CandidateSheetDtls.CreateCell(2).SetCellValue(response.Result[i].EmailID);
                        CandidateSheetDtls.CreateCell(3).SetCellValue(response.Result[i].SourceID.ToString());
                        CandidateSheetDtls.CreateCell(4).SetCellValue(response.Result[i].NoticePeriod.ToString());
                        CandidateSheetDtls.CreateCell(5).SetCellValue(response.Result[i].Gender);
                        CandidateSheetDtls.CreateCell(6).SetCellValue(response.Result[i].ContactNo);
                        CandidateSheetDtls.CreateCell(7).SetCellValue(response.Result[i].Experience);
                        CandidateSheetDtls.CreateCell(8).SetCellValue(response.Result[i].RelevantExperience);
                        CandidateSheetDtls.CreateCell(9).SetCellValue(response.Result[i].ExperienceInMonths.ToString());
                        CandidateSheetDtls.CreateCell(10).SetCellValue(response.Result[i].ReleExpeInMonths.ToString());
                        CandidateSheetDtls.CreateCell(11).SetCellValue(response.Result[i].CPCurrency);
                        CandidateSheetDtls.CreateCell(12).SetCellValue(response.Result[i].CPTakeHomeSalPerMonth.ToString());
                        CandidateSheetDtls.CreateCell(13).SetCellValue(response.Result[i].CPGrossPayPerAnnum.ToString());
                        CandidateSheetDtls.CreateCell(14).SetCellValue(response.Result[i].CurrOrganization);
                        CandidateSheetDtls.CreateCell(15).SetCellValue(response.Result[i].CurrLocation);
                        CandidateSheetDtls.CreateCell(16).SetCellValue(response.Result[i].SelfRating.ToString());
                        CandidateSheetDtls.CreateCell(17).SetCellValue(response.Result[i].DOB.ToString());
                        CandidateSheetDtls.CreateCell(18).SetCellValue(response.Result[i].MaritalStatus);
                        CandidateSheetDtls.CreateCell(19).SetCellValue(response.Result[i].EPCurrency);
                        CandidateSheetDtls.CreateCell(20).SetCellValue(response.Result[i].EPTakeHomePerMonth.ToString());
                    }

                    workbook.Write(fs);
                }

                // to returning bytes 
                byte[] bytes = System.IO.File.ReadAllBytes(WebRootFolder + "\\" + FileName);

                string fileBytes = Convert.ToBase64String(bytes);

                // deleting uploaded file
                if ((System.IO.File.Exists(WebRootFolder + "\\" + FileName)))
                {
                    System.IO.File.Delete(WebRootFolder + "\\" + FileName);
                }
                return File(bytes, "application/ms-excel", FileName);

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("CandidatesExportToCSV")]
        public async Task<IActionResult> CandidatesExportToCSVAsync(CandidateExportSearchViewModel candidateExportSearchViewModel)
        {
            try
            {

                var comlumHeadrs = new string[]
                {
            "CandProfID",
            "CandName",
            "EmailID",
            "SourceID",
            "NoticePeriod",
            "Gender",
            "ContactNo",
            "Experience",
            "RelevantExperience",
            "ExperienceInMonths",
            "ReleExpeInMonths",
            "CPCurrency",
            "CPTakeHomeSalPerMonth",
            "CPGrossPayPerAnnum",
            "CurrOrganization",
            "CurrLocation",
            "SelfRating",
            "DOB",
            "MaritalStatus",
            "EPCurrency",
            "EPTakeHomePerMonth"
            };

                var response = await candidateRepository.GetExportCandidates(candidateExportSearchViewModel);

                var records = (from data in response.Result
                               select new object[]
                                       {
            data.CandProfID,
            data.CandName,
            data.EmailID,
            data.SourceID,
            data.NoticePeriod,
            data.Gender,
            data.ContactNo,
            data.Experience,
            data.RelevantExperience,
            data.ExperienceInMonths,
            data.ReleExpeInMonths,
            data.CPCurrency,
            data.CPTakeHomeSalPerMonth,
            data.CPGrossPayPerAnnum,
            data.CurrOrganization,
            data.CurrLocation,
            data.SelfRating,
            data.DOB,
            data.MaritalStatus,
            data.EPCurrency,
            data.EPTakeHomePerMonth
                                       }).ToList();

                // Build the file content
                var candidateCSV = new StringBuilder();
                records.ForEach(line =>
                {
                    candidateCSV.AppendLine(string.Join(",", line));
                });

                byte[] buffer = Encoding.ASCII.GetBytes($"{string.Join(",", comlumHeadrs)}\r\n{candidateCSV.ToString()}");
                return File(buffer, "text/csv", $"Candidates.csv");
            }
            catch (Exception)
            {
                throw;
            }

        }

        #endregion

        #region Candidate view action

        [HttpGet("{Id:int}/{JobId:int}")]
        public async Task<IActionResult> GetCandidateAsync(int Id, int JobId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.GetCandidate(Id, JobId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [AllowAnonymous]
        [HttpGet("GetCandidate/Overview/{CanPrfId:int}/{JobId:int}")]
        public async Task<IActionResult> GetCandidateOverviewAsync(int CanPrfId, int JobId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.GetCandidateOverview(CanPrfId, JobId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpGet("GetCandidate/Skills/{CanPrfId:int}/{JobId:int}")]
        public async Task<IActionResult> GetCandidateSkillsAsync(int CanPrfId, int JobId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.GetCandidateSkills(CanPrfId, JobId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("GetCandidate/Activities/{JobId:int}/{CanPrfId:int}")]
        public async Task<IActionResult> GetCandidateActivitiesAsync(int JobId,int CanPrfId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.GetCandidateActivities(JobId, CanPrfId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("Status/Review")]
        public async Task<IActionResult> CandidateStatusReviewAsync(CandidateStatusReviewViewModel candidate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.CandidateStatusReview(candidate);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Status/Review/List/{candProfId:int}/{jobId:int}")]
        public async Task<IActionResult> CandidateStatusReviewListAsync(int jobId,int candProfId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.CandidateStatusReviewList(jobId, candProfId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpGet("GetCandidate/TagJobs/{CanPrfId:int}")]
        public async Task<IActionResult> GetCandidateTagJobsAsync(int CanPrfId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.GetCandidateTagJobs(CanPrfId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("ValidateCandidateSalary")]
        public async Task<IActionResult> ValidateCandidateSalaryAsync(ValidateCanJobSalaryViewModel validateCanJobSalaryViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.ValidateCandidateSalary(validateCanJobSalaryViewModel);
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

        [HttpGet("CandidatePackageDtls/{candId:int}/{jobId:int}")]
        public async Task<IActionResult> CandidatePackageDtlsAsync(int candId, int jobId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.GetCandidateJobPackageDtls(candId, jobId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("Reoffer")]
        public async Task<IActionResult> ReofferAsync(ReofferPackageViewModel reofferPackageViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.ReofferPackage(reofferPackageViewModel);
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

        [HttpPost("ReOfferApprove")]
        public async Task<IActionResult> ReOfferApproveAsync(ReOfferApproveViewModel reOfferApproveViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.ReOfferApprove(reOfferApproveViewModel);
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

        [HttpGet("CandidateTags/{CandidateId:int}")]
        public async Task<IActionResult> GetCandidateTagsAsync(int CandidateId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.GetCandidateTags(CandidateId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("CandidateTag")]
        public async Task<IActionResult> CreateCandidateTagAsync(CreateTag createTag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.CreateCandidateTag(createTag);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpDelete("CandidateTag/{TagId:int}")]
        public async Task<IActionResult> DeleteCandidateTagAsync(int TagId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.DeleteCandidateTag(TagId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Candidate document action


        [AllowAnonymous]
        [HttpPost("GetCandidate/Resume")]
        public async Task<IActionResult> GetCandidateResumeAsync(ResumeViewModel resumeViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.GetCandidateResume(resumeViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpPost("GetCandidate/VideoProfile")]
        public async Task<IActionResult> GetCandidateVideoProfileAsync(ResumeViewModel resumeViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.GetCandidateVideoProfile(resumeViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("UploadCandidateFile")]
        public async Task<IActionResult> UploadCandidateFileAsync(IFormCollection collection)
        {

            var uploadCandidateFileViewModel = new UploadCandidateFileViewModel();
            if (Request.Form != null)
            {
                uploadCandidateFileViewModel.Files = Request.Form.Files.Where(x => x.Name == "Files").ToList();
            }

            await TryUpdateModelAsync(uploadCandidateFileViewModel);

            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.UploadCandidateFiles(uploadCandidateFileViewModel);
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

        [HttpPost("UpdateCandidateFile")]
        public async Task<IActionResult> UpdateCandidateFileAsync(IFormCollection collection)
        {

            var updateCandidateFileViewModel = new UpdateCandidateFileViewModel();
            if (Request.Form != null)
            {
                updateCandidateFileViewModel.Files = Request.Form.Files.Where(x => x.Name == "Files").ToList();
            }

            await TryUpdateModelAsync(updateCandidateFileViewModel);

            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.UpdateCandidateFile(updateCandidateFileViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("CandidateDocumentRequest")]
        public async Task<IActionResult> CandidateDocumentRequestAsync(CandidateDocumentRequestViewModel candidateDocumentRequestViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.CandidateDocumentRequest(candidateDocumentRequestViewModel);
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


        [HttpPost("CandidateMultiDocumentRequest")]
        public async Task<IActionResult> CandidateMultiDocumentRequestAsync(CandidateMultiDocumentRequestViewModel candidateMultiDocumentRequestViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.CandidateMultiDocumentRequest(candidateMultiDocumentRequestViewModel);
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

        [HttpPost("CandidateFileApproveReject")]
        public async Task<IActionResult> CandidateFileApproveRejectAsync(CandidateFileApproveRejectViewModel candidateFileApproveRejectViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.CandidateFileApproveReject(candidateFileApproveRejectViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost("CandidateFinalCVReject")]
        public async Task<IActionResult> CandidateFinalCVRejectAsync(CandidateFinalCVRejectViewModel candidateFinalCVRejectViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.CandidateFinalCVReject(candidateFinalCVRejectViewModel);
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
        [HttpGet("GetCandidate/Files/{CanPrfId:int}/{JobId:int}")]
        public async Task<IActionResult> GetCandidateFilesAsync(int CanPrfId, int JobId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.GetCandidateFiles(CanPrfId, JobId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("GetCandidateRequestedFiles/{CanPrfId:int}/{JobId:int}")]
        public async Task<IActionResult> GetCandidateRequestedFilesAsync(int CanPrfId, int JobId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.GetCandidateRequestedFiles(CanPrfId, JobId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Candidate qualifications and certifications and employment 

        [HttpPost("CreateCandidateCertification")]
        public async Task<IActionResult> CreateCandidateCertificationAsync(CreateCandidateCertificationModel createCandidateCertificationModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.CreateCandidateCertification(createCandidateCertificationModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Candidate rating action

        [AllowAnonymous]
        [HttpGet("GetCandidateRatingSummary/{JoId:int}/{CandProfId:int}")]
        public async Task<IActionResult> GetCandidateRatingSummaryAsync(int JoId, int CandProfId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.GetCandidateJobEvaluationSummary(JoId, CandProfId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpGet("GetTeamCandidateRating/{JoId:int}/{CandProfId:int}")]
        public async Task<IActionResult> GetCandidateRatingAsync(int JoId, int CandProfId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.GetTeamCandidateRating(JoId, CandProfId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpGet("GetClientCandidateRating/{JoId:int}/{CandProfId:int}")]
        public async Task<IActionResult> GetClientCandidateRatingAsync(int JoId, int CandProfId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.GetClientCandidateRating(JoId, CandProfId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpGet("GetCandidateJobEvaluationSummary/{JoId:int}/{CandProfId:int}")]
        public async Task<IActionResult> GetCandidateJobEvaluationSummaryAsync(int JoId, int CandProfId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.GetCandidateJobEvaluationSummary(JoId, CandProfId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpGet("GetCandidateOverallEvaluationSummary/{CandProfId:int}")]
        public async Task<IActionResult> GetCandidateOverallEvaluationSummaryAsync(int CandProfId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.GetCandidateOverallEvaluationSummary(CandProfId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpPost("UpdateCandidateRating")]
        public async Task<IActionResult> UpdateCandidateRatingAsync(UpdateCandidateRatingModel candidateRatingModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.UpdateCandidateRating(candidateRatingModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Candidate Recruiter 

      
        [HttpPost("UpdateCandidateRecruiter")]
        public async Task<IActionResult> UpdateCandidateRecruiterAsync(UpdateCandidateRecModel candidateRatingModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.UpdateCandidateRecruiter(candidateRatingModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("GetCandidateRecHistory/{JoId}/{CandProfId}")]
        public async Task<IActionResult> GetCandidateRecHistory(int JoId, int CandProfId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.GetCandidateRecHistory(JoId, CandProfId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Assessments

        [AllowAnonymous]
        [HttpPost("MapAssessmentResponse")]
        public async Task<IActionResult> MapAssessmentResponseAsync([FromBody] PostVM data)
        {
            try
            {
                var response = await candidateRepository.MapAssessmentResponse(data);
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
        [HttpGet("GetCandidate/Assessments/{CanPrfId:int}/{JobId:int}")]
        public async Task<IActionResult> CandidateAssessmentsAsync(int CanPrfId, int JobId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.CandidateAssessments(CanPrfId, JobId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("CreateCandidateAssessment")]
        public async Task<IActionResult> CreateCandidateAssessmentAsync(CreateCandidateAssessmentViewModel createCandidateAssessmentViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.CreateCandidateAssessment(createCandidateAssessmentViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Unsubscribe

        [AllowAnonymous]
        [HttpPost("Unsubscribe")]
        public async Task<IActionResult> UnsubscribeCandidateAsync(CandidateUnSubscribeRequestModel candidateUnSubscribeRequestModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.Unsubscribe(candidateUnSubscribeRequestModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Currency Exchange

        [AllowAnonymous]
        [HttpPost("CurrConv")]
        public IActionResult CurrConvAsync(CurrConvViewModel currConvViewModel)
        {
            try
            {
                var ob = candidateRepository.CurrConv(currConvViewModel);
                return Ok(ob);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Resend email       

        [HttpPost("ResendUserCred")]
        public async Task<IActionResult> ResendUserCredAsync(ResendUserCredViewModel resendUserCredViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await candidateRepository.ResendUserCred(resendUserCredViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}
