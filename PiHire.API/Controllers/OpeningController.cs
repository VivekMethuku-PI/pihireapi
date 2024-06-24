using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PiHire.API.Common;
using PiHire.API.Common.Hubs;
using PiHire.BAL.Common.Extensions;
using PiHire.BAL.IRepositories;
using PiHire.BAL.Repositories;
using PiHire.BAL.ViewModels;
using PiHire.o_SD.Controllers;
using System;
using System.Threading.Tasks;

namespace PiHire.API.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class OpeningController : BaseController
    {
        private readonly IHubContext<NotificationHub> hubContext;
        readonly ILogger<OpeningController> logger;
        private readonly AppSettings _appSettings;
        private readonly IOpeningRepository openingRepository;
        private readonly INotificationRepository notificationRepository;

        public OpeningController(ILogger<OpeningController> logger,
            AppSettings appSettings,
            IOpeningRepository openingRepository, ILoggedUserDetails loginUserDetails, IHubContext<NotificationHub> hubContext, INotificationRepository notificationRepository) : base(openingRepository, loginUserDetails, hubContext, notificationRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appSettings = appSettings;
            this.openingRepository = openingRepository;
            this.notificationRepository = notificationRepository;
            this.hubContext = hubContext;
        }


        [HttpPost]
        public async Task<IActionResult> CreateJobsync([FromBody] CreateOpeningViewModel createOpeningViewModel)
        {
            /// <summary>
            /// Skill groupId - 186
            /// Skill Level - 90
            /// Currency - 13 
            /// Tenure - 37
            /// Priority - 34
            /// Notice Period - 25
            /// FieldCodes - 185
            /// OpeningType - 59
            /// </summary>
            /// <returns></returns>
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.CreateJobsync(createOpeningViewModel);
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
        public async Task<IActionResult> EditJobAsync(EditOpeningViewModel editOpeningViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.EditJob(editOpeningViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpGet("{Id}")]
        public async Task<IActionResult> GetJobAsync(int Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.GetJobAsync(Id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpGet("GetPortalJob/{Id}")]
        public async Task<IActionResult> GetPortalJobsAsync(int Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.GetPortalJobs(Id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("JobsList")]
        public async Task<IActionResult> JobsListAsync(OpeningListSearchViewModel openingListSearchViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.JobsList(openingListSearchViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("JobsListToAssignRecruiters")]
        public async Task<IActionResult> JobsListToAssignRecruitersAsync(JobsListToAssignRecruitersSearchViewModel jobsListToAssignRecruitersSearchViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.JobsListToAssignRecruiters(jobsListToAssignRecruitersSearchViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("Recruiter/TodayAssignments")]
        public async Task<IActionResult> RecruiterTodayAssignmentsAsync(JobsListToAssignRecruitersSearchViewModel jobsListToAssignRecruitersSearchViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.RecruiterTodayAssignmentsAsync(jobsListToAssignRecruitersSearchViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost("Recruiter/JobAssignment/DayWise/Search")]
        public async Task<IActionResult> RecruiterJobAssignmentDayWiseAsync(RecruiterJobAssignmentSearchViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.RecruiterJobAssignmentDayWiseAsync_search(model);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost("Recruiter/JobAssignment/DayWise")]
        public async Task<IActionResult> RecruiterJobAssignmentDayWiseAsync(RecruiterJobAssignmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.RecruiterJobAssignmentDayWiseAsync(model);
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

        [HttpPost("DeAssignJobToTeamMember")]
        public async Task<IActionResult> DeAssignJobToTeamMemberAsync(DeAssignJobViewmodel deAssignJobViewmodel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.DeAssignJobToTeamMember(deAssignJobViewmodel);
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


        //[HttpPost("MultipleAssignJobToTeamMember")]
        //public async Task<IActionResult> MultipleAssignJobToTeamMemberAsync(MultipleJobAssignmentMembersViewModel multipleJobAssignmentMembersViewModel)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequestCustom(ModelState);
        //    }
        //    try
        //    {
        //        var response = await openingRepository.AssignMultipleJobToTeamMember(multipleJobAssignmentMembersViewModel);
        //        if (response.Item1.Count > 0)
        //        {
        //            await PushNotificationToClientAsync(response.Item1);
        //        }
        //        return Ok(response.Item2);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}



        //[HttpGet("SlefAssignJob/{joId:int}")]
        //public async Task<IActionResult> SlefAssignJobAsync(int joId)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequestCustom(ModelState);
        //    }
        //    try
        //    {
        //        var response = await openingRepository.SlefAssignJob(joId);
        //        return Ok(response);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}



        [HttpGet("GetJobsListToTagCandidate")]
        public async Task<IActionResult> GetJobsListToTagCandidateAsync([FromQuery] string searchKey, [FromQuery] int? perPage, [FromQuery] int? currentPage, [FromQuery] int? CandidateId)
        {
            var model = new TagJobListSearchViewModel
            {
                PerPage = perPage ?? 100,
                CurrentPage = currentPage ?? 1,
                CandidateId = CandidateId,
                SearchKey = searchKey
            };
            await TryUpdateModelAsync(model);
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.GetJobsListToTagCandidate(model);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut("ReOpenJob")]
        public async Task<IActionResult> ReOpenJobAsync([FromBody] int Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.ReOpenJob(Id);
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

        [HttpPut("HoldJob")]
        public async Task<IActionResult> HoldJobAsync([FromBody] int Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.HoldJob(Id);
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

        [HttpPut("AddMoreCVPerJob")]
        public async Task<IActionResult> AddMoreCVPerJobAsync(MoreCVPerJobViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.AddMoreCVPerJob(model);
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

        [HttpPut("AddMoreCVPerJobRecruiter")]
        public async Task<IActionResult> AddMoreCVPerJobRecruiterAsync(MoreCVPerJobRecruiterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.AddMoreCVPerJobRecruiter(model);
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



        [HttpPut("UpdateJobStatus")]
        public async Task<IActionResult> UpdateJobStatusAsync(UpdateOpeningViewModel updateOpeningViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.UpdateJobStatus(updateOpeningViewModel);
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

        [HttpPut("CloseJob")]
        public async Task<IActionResult> CloseJobAsync([FromBody] int Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.CloseJob(Id);
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

        [HttpPost("CloneJob")]
        public async Task<IActionResult> CloneJobAsync([FromBody] int Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.CloneJob(Id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpGet("GetJobInfo/{Id}")]
        public async Task<IActionResult> GetJobInfoAsync(int Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.GetJobInfo(Id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("AssignedJobs")]
        public async Task<IActionResult> AssignedJobsAsync(AssignedJobsReqViewModel assignedJobsReqViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.AssignedJobs(assignedJobsReqViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Notes 

        [HttpPost("Note")]
        public async Task<IActionResult> AddJobNoteAsync(CreateJobNotesViewModel createJobNotesViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.AddJobNote(createJobNotesViewModel);
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

        [HttpGet("Note/{JobId:int}")]
        public async Task<IActionResult> GetJobNotesAsync(int JobId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.GetJobNotes(JobId, 0);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Note/{JobId:int}/{CandId:int}")]
        public async Task<IActionResult> GetJobCandNotesAsync(int JobId, int CandId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.GetJobNotes(JobId, CandId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpDelete("Note/{Id:int}")]
        public async Task<IActionResult> DeletJobNoteAsync(int Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.DeleteJobNote(Id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region  Job View 

        [HttpGet("JobDescription/{JobId:int}")]
        public async Task<IActionResult> GetJobDescriptionAsync(int JobId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.GetJobDescription(JobId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("GetJobAssessments/{JobId:int}")]
        public async Task<IActionResult> GetJobAssessmentsAsync(int JobId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.GetJobAssessments(JobId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("MapAssessmentToJob")]
        public async Task<IActionResult> MapAssessmenttoJobAsync(MapAssessmentToJobViewModel mapAssessmentToJobViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.MapAssessmenttoJob(mapAssessmentToJobViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("GetJobTeamMembers/{JobId:int}")]
        public async Task<IActionResult> GetJobTeamMembersAsync(int JobId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.GetJobTeamMembers(JobId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("AssignJobToTeamMember")]
        public async Task<IActionResult> AssignJobToTeamMemberAsync(JobAssignedMembersViewModel jobAssignedMembersViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.AssignJobToTeamMember(jobAssignedMembersViewModel);
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

        [HttpGet("GetJobAssociatedPannel/{JobId:int}")]
        public async Task<IActionResult> GetJobAssociatedPannelAsync(int JobId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.GetJobAssociatedPannel(JobId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion


        #region Pipeline

        [HttpGet("GetJobPipeline/{JobId:int}")]
        public async Task<IActionResult> GetJobPipelineAsync(int JobId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.GetJobPipeline(JobId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Activities

        [HttpGet("GetJobActivities/{JobId:int}")]
        public async Task<IActionResult> GetJobActivitiesAsync(int JobId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.GetJobActivities(JobId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Website

        [AllowAnonymous]
        [HttpPost("PortalJobsList")]
        public async Task<IActionResult> PortalJobsListAsync(PortalJobSearchViewModel portalJobSearchViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.GetPortalJobsList(portalJobSearchViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpGet("GetCountryWiseJobCounts")]
        public async Task<IActionResult> GetCountryWiseJobCountsAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.GetCountryWiseJobCounts();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpGet("GetLocationWiseJobCounts/{CountryId:int}")]
        public async Task<IActionResult> GetLocationWiseJobCountsAsync(int CountryId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.GetLocationWiseJobCounts(CountryId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpGet("GetCandidateActiveJobs/{CanPrfId:int}")]
        public async Task<IActionResult> GetCandidateActiveJobsAsync(int CanPrfId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.GetCandidateActiveArchivedJobs(CanPrfId, 1);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("GetCandidateArchivedJobs/{CanPrfId:int}")]
        public async Task<IActionResult> GetCandidateArchivedJobsAsync(int CanPrfId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.GetCandidateActiveArchivedJobs(CanPrfId, 2);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("GetCandidateSimilarJobs")]
        public async Task<IActionResult> GetCandidateSimilarJobsAsync([FromQuery] int? perPage, [FromQuery] int? currentPage, [FromQuery] int CandidateId, [FromQuery] string searchKey, [FromQuery] int? CountryId)
        {

            var model = new CandidateSimilarJobSearchViewModel
            {
                PerPage = perPage ?? 10,
                CurrentPage = currentPage ?? 1,
                CanPrfId = CandidateId,
                SearchKey = searchKey,
                CountryId = CountryId
            };
            await TryUpdateModelAsync(model);
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.GetCandidateSimilarJobs(model);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region 


        [HttpGet("GetCandidatesSharedToClient/{JobId:int}/{Type:int}")]
        public async Task<IActionResult> GetCandidatesSharedToClientAsync(int JobId, int Type)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.GetCandidatesSharedToClient(JobId, Type);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpGet("GetTodayJobAssignments")]
        public async Task<IActionResult> GetTodayJobAssignmentsAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await openingRepository.GetTodayJobAssignments();
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
