using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PiHire.API.Common;
using PiHire.BAL.Common.Extensions;
using PiHire.BAL.IRepositories;
using PiHire.BAL.Repositories;
using PiHire.BAL.ViewModels;
using PiHire.BAL.ViewModels.ApiBaseModels;
using PiHire.DAL.Models;
using PiHire.o_SD.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PiHire.API.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportController : BaseController
    {
        readonly ILogger<ReportController> logger;
        private readonly AppSettings _appSettings;
        private readonly IReportRepository reportRepository;

        public ReportController(ILogger<ReportController> logger,
            AppSettings appSettings, IReportRepository reportRepository, ILoggedUserDetails loginUserDetails) : base(reportRepository, loginUserDetails)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appSettings = appSettings;
            this.reportRepository = reportRepository;
        }


        #region OLD Dashboard

        [HttpPost("Dashboard/CandidateInterviews/ResultDue")]
        public async Task<IActionResult> GetCandidateInterviewResultDueAsync(DashboardCandidateInterviewFilterViewModel filterViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                filterViewModel = filterViewModel ?? new DashboardCandidateInterviewFilterViewModel() { };
                var response = await reportRepository.GetCandidateInterviewAsync(filterViewModel, 2);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("Dashboard/CandidateInterviews/Active")]
        public async Task<IActionResult> GetCandidateInterviewActiveAsync(DashboardCandidateInterviewFilterViewModel filterViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                filterViewModel = filterViewModel ?? new DashboardCandidateInterviewFilterViewModel() { };
                var response = await reportRepository.GetCandidateInterviewAsync(filterViewModel, 1);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("Dashboard/CandidateInterviews/Hold")]
        public async Task<IActionResult> GetCandidateInterviewHoldAsync(DashboardCandidateInterviewFilterViewModel filterViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                filterViewModel = filterViewModel ?? new DashboardCandidateInterviewFilterViewModel() { };
                var response = await reportRepository.GetCandidateInterviewAsync(filterViewModel, 3);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("Dashboard/JobStatus")]
        public async Task<IActionResult> GetDashboardJobStageAsync(DashboardCandidateInterviewFilterViewModel filterViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetDashboardJobStageAsync(filterViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Dashboard/JobStatus/{jobId}")]
        public async Task<IActionResult> GetDashboardJobRecruiterStageAsync(int jobId)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetDashboardJobRecruiterStageAsync(jobId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("Dashboard/JobStatus/{stageId}/candidates")]
        public async Task<IActionResult> GetDashboardJobRecruiterStageCandidatesAsync(int stageId, DashboardCandidateInterviewFilterViewModel filterViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetDashboardJobStageCandidatesAsync(stageId, filterViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Dashboard/JobStatus/{jobId}/{stageId}/candidates")]
        public async Task<IActionResult> GetDashboardJobRecruiterStageCandidatesAsync(int jobId, int stageId)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetDashboardJobStageCandidatesAsync(jobId, stageId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet("Dashboard/JobStatus/{jobId}/timespan")]
        public async Task<IActionResult> GetDashboardRecruiterAnalyticsAsync(int jobId)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetDashboardJobTimeAsync(jobId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("Dashboard/RecruiterStatus/Active")]
        public async Task<IActionResult> GetDashboardRecruiterStatusActiveAsync(DashboardFilterViewModel filterViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetDashboardRecruiterStatusAsync(filterViewModel, false);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost("Dashboard/RecruiterStatus/leave")]
        public async Task<IActionResult> GetDashboardRecruiterStatusLeaveAsync(DashboardFilterViewModel filterViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetDashboardRecruiterStatusAsync(filterViewModel, true);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost("Dashboard/DaywiseStatus/Recruiters")]
        public async Task<IActionResult> GetDashboardRecruiterDaywiseAsync(DashboardRecruiterDaywiseFilterViewModel filterViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetDashboardRecruiterDaywiseAsync(filterViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost("Dashboard/DaywiseStatus/Recruiter/{recruiterId}/jobs")]
        public async Task<IActionResult> GetDashboardJobsDaywisePipelineAsync_recruiter(int recruiterId, DashboardFilterPaginationViewModel filterViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetDashboardJobsDaywisePipelineAsync(BAL.Common.Types.AppConstants.UserType.Recruiter, recruiterId, filterViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost("Dashboard/DaywiseStatus/Recruiter/{recruiterId}/jobs/History")]
        public async Task<IActionResult> GetDashboardRecruiterDaywiseHistoryAsync(int recruiterId, DashboardRecruiterDaywiseHistoryFilterViewModel filterViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetDashboardJobRecruiterDaywiseHistoryAsync(null, recruiterId, filterViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost("Dashboard/DaywiseStatus/Recruiter/{recruiterId}/assignments/history")]
        public async Task<IActionResult> GetDashboardRecruiterAssignmentHistoryAsync(int recruiterId, DashboardFilterPaginationViewModel filterViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetDashboardRecruiterAssignmentHistoryAsync(null, recruiterId, filterViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost("Dashboard/DaywiseStatus/Recruiter/{recruiterId}/job/{jobId}/assignments/history")]
        public async Task<IActionResult> GetDashboardRecruiterAssignmentHistoryAsync(int recruiterId, int jobId, DashboardFilterPaginationViewModel filterViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetDashboardRecruiterAssignmentHistoryAsync(jobId, recruiterId, filterViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost("Dashboard/DaywiseStatus/Bdms")]
        public async Task<IActionResult> GetDashboardBdmRecruiterStatusActiveAsync(DashboardRecruiterDaywiseFilterViewModel filterViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetDashboardBdmDaywisePipelineAsync(filterViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost("Dashboard/DaywiseStatus/Bdm/{bdmId}/jobs")]
        public async Task<IActionResult> GetDashboardJobsDaywisePipelineAsync_BDM(int bdmId, DashboardFilterPaginationViewModel filterViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetDashboardJobsDaywisePipelineAsync(BAL.Common.Types.AppConstants.UserType.BDM, bdmId, filterViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost("Dashboard/DaywiseStatus/Bdm/{bdmId}/jobs/History")]
        public async Task<IActionResult> GetDashboardBdmDaywiseHistoryAsync(int bdmId, DashboardRecruiterDaywiseHistoryFilterViewModel filterViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetDashboardBdmDaywiseHistoryAsync(bdmId, filterViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost("Dashboard/DaywiseStatus/Job/{jobId}/Recruiters")]
        public async Task<IActionResult> GetDashboardJobRecruitersDaywisePipelineAsync(int jobId, DashboardFilterPaginationViewModel filterViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetDashboardJobRecruitersDaywisePipelineAsync(jobId, filterViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost("Dashboard/DaywiseStatus/Job/{jobId}/Recruiters/assigned")]
        public async Task<IActionResult> GetDashboardDaywiseJobRecruitersAsync_assigned(int jobId, DashboardFilterPaginationViewModel filterViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetDashboardDaywiseJobRecruitersAsync_assigned(jobId, filterViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost("Dashboard/DaywiseStatus/Job/{jobId}/Recruiters/deassigned")]
        public async Task<IActionResult> GetDashboardDaywiseJobRecruitersAsync_deassigned(int jobId, DashboardFilterPaginationViewModel filterViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetDashboardDaywiseJobRecruitersAsync_deassigned(jobId, filterViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost("Dashboard/DaywiseStatus/Job/{jobId}/Recruiters/hireSuggest")]
        public async Task<IActionResult> GetDashboardDaywiseJobRecruitersAsync_hireSuggest(int jobId, DashboardFilterPaginationViewModel filterViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetDashboardDaywiseJobRecruitersAsync_hireSuggest(jobId, filterViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost("Dashboard/DaywiseStatus/Job/{jobId}/Recruiter/{recruiterId}/hireSuggest/similarJobs")]
        public async Task<IActionResult> GetDashboardDaywiseJobRecruitersAsync_hireSuggest_similarJobs(int jobId, int recruiterId, DashboardFilterPaginationViewModel filterViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetDashboardDaywiseJobRecruitersAsync_hireSuggest_similarJobs(jobId, recruiterId, filterViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost("Dashboard/DaywiseStatus/Job/{jobId}/Recruiters/notAssigned")]
        public async Task<IActionResult> GetDashboardDaywiseJobRecruitersAsync_notAssigned(int jobId, DashboardFilterPaginationViewModel filterViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetDashboardDaywiseJobRecruitersAsync_notAssigned(jobId, filterViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost("Dashboard/DaywiseStatus/Job/{jobId}/Recruiter/{recruiterId}/History")]
        public async Task<IActionResult> GetDashboardJobRecruitersDaywiseHistoryAsync(int jobId, int recruiterId, DashboardRecruiterDaywiseHistoryFilterViewModel filterViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetDashboardJobRecruiterDaywiseHistoryAsync(jobId, recruiterId, filterViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("Dashboard/HireAdmin")]
        public async Task<IActionResult> GetDashboardHireAdminAsync(DashboardCandidateInterviewFilterViewModel filterViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetDashboardHireAdminAsync(filterViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost("Dashboard/Bdm")]
        public async Task<IActionResult> GetDashboardBdmAsync(DashboardBdmFilterViewModel filterViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetDashboardBdmAsync(filterViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost("Dashboard/Recruiter")]
        public async Task<IActionResult> GetDashboardRecruiterAsync(DashboardRecruiterFilterViewModel filterViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetDashboardRecruiterAsync(filterViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet("Dashboard/Recruiter/JobCategory/last14Days")]
        public async Task<IActionResult> GetDashboardRecruiterJobCatgLast14DaysAsync()
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetDashboardRecruiterJobCatgLast14DaysAsync();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost("Dashboard/Recruiter/JobCategory/Present")]
        public async Task<IActionResult> GetDashboardRecruiterJobCatgPresentAsync(DashboardRecruiterFilterViewModel filterViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetDashboardRecruiterJobCatgPresentAsync(filterViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost("Dashboard/Recruiter/JobCategory/prev")]
        public async Task<IActionResult> GetDashboardRecruiterJobCatgPastAsync(DashboardRecruiterFilterViewModel filterViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetDashboardRecruiterJobCatgPastAsync(filterViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost("Dashboard/Recruiter/JobCategory/all")]
        public async Task<IActionResult> GetDashboardRecruiterJobCatgAllAsync(DashboardRecruiterFilterViewModel filterViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetDashboardRecruiterJobCatgAllAsync(filterViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost("Dashboard/Recruiter/{recruiterId}/analytics")]
        public async Task<IActionResult> GetDashboardRecruiterAnalyticsAsync(int recruiterId, DashboardRecruiterFilterViewModel filterViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetDashboardAdminRecruiterAnalyticAsync(recruiterId, filterViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet("Dashboard/Recruiter/{recruiterId}/avgHireDays")]
        public async Task<IActionResult> GetDashboardRecruiterAvgHireDaysAsync(int recruiterId)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetDashboardRecruiterAvgHireDaysAsync(recruiterId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("Dashboard/Recruiter/Candidates/Present")]
        public async Task<IActionResult> GetDashboardRecruiterCandidatesPresentAsync(DashboardRecruiterFilterViewModel filterViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetDashboardRecruiterCandidatesPresentAsync(filterViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost("Dashboard/Recruiter/Candidates/prev")]
        public async Task<IActionResult> GetDashboardRecruiterCandidatesPastAsync(DashboardRecruiterFilterViewModel filterViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetDashboardRecruiterCandidatesPastAsync(filterViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }


        #endregion


        #region New Dashboard 
        [HttpGet("BroughtBy/{boughtBy}/JobClientNames")]
        public async Task<IActionResult> GetBroughtByJobClientNamesAsync(int boughtBy)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetBroughtByJobClientNamesAsync(boughtBy);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet("BroughtBy/{boughtBy}/JobClientNames/{puId}")]
        public async Task<IActionResult> GetBroughtByJobClientNamesAsync(int boughtBy, int puId)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetBroughtByJobClientNamesAsync(boughtBy, puId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost("BroughtBy/{boughtBy}/Daywise/JobClientNames")]
        public async Task<IActionResult> GetBroughtByDayWiseJobClientNamesAsync(int boughtBy, DayWiseBoughtByAccountSearchModel model)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetBroughtByDayWiseJobClientNamesAsync(boughtBy, model);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet("BroughtBy/{boughtBy}/InterviewClientNames")]
        public async Task<IActionResult> GetBroughtByInterviewClientNamesAsync(int boughtBy)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetBroughtByInterviewClientNamesAsync(boughtBy);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("Dashboard/HireAssignments")]
        public async Task<IActionResult> JobsListAsync(HireAssignmentsSearchViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await reportRepository.JobsList(model);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("Dashboard/DayWiseAssignment")]
        public async Task<IActionResult> DayWiseAssignmentJobsSync(DayWiseAssignmentsJobsSearchViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await reportRepository.DayWiseJobsList(model);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("Dashboard/Interviews")]
        public async Task<IActionResult> GetCandidateInterviewsAsync(CandidateInterviewSearchViewModel filterViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                filterViewModel = filterViewModel ?? new CandidateInterviewSearchViewModel() { };
                var response = await reportRepository.GetCandidateInterviewsAsync(filterViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Dashboard/{JobId}/Candidate/{CandProfId}/StageWiseInfo")]
        public async Task<IActionResult> GetCandidateStageWiseInfoAsync(int JobId, int CandProfId)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetCandidateStageWiseInfo(JobId, CandProfId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("Dashboard/Job/Candidates/ProfileStatus")]
        public async Task<IActionResult> GetJobCandidatesBasedOnProfileStatusAsync(CandidateProfileStatusSearchViewModel filterViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                filterViewModel = filterViewModel ?? new CandidateProfileStatusSearchViewModel() { };
                var response = await reportRepository.GetJobCandidatesBasedOnProfileStatusAsync(filterViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Dashboard/JobDescriptionWithPipeline/{jobId}")]
        public async Task<IActionResult> GetGetJobDescriptionWithPipelineAsync(int jobId)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetJobDescriptionWithPipeline(jobId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut("Dashboard/Job/Priority")]
        public async Task<IActionResult> JobPriorityUpdateAsync(PriorityChangeViewModel filterViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                filterViewModel = filterViewModel ?? new PriorityChangeViewModel() { };
                var response = await reportRepository.JobPriorityUpdateAsync(filterViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut("Dashboard/Job/Recruiter/Priority")]
        public async Task<IActionResult> JobRecruiterPriorityUpdateAsync(PriorityChangeViewModel filterViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                filterViewModel = filterViewModel ?? new PriorityChangeViewModel() { };
                var response = await reportRepository.JobRecruiterPriorityUpdateAsync(filterViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("Dashboard/{bdmId}/{stageId}/candidates")]
        public async Task<IActionResult> GetDashboardJobBDMStageCandidatesAsync(int bdmId, int stageId)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetDashboardJobBDMStageCandidatesAsync(bdmId, stageId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion


        #region Reports 


        [HttpPost("BDMSOverview")]
        public async Task<IActionResult> GetBDMSOverviewAsync(ReportRequestViewModel reportRequestViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetBDMsOverview(reportRequestViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("BDMOverview")]
        public async Task<IActionResult> GetBDMOverviewAsync(ReportRequestViewModel reportRequestViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetBDMOverview(reportRequestViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("BDMOpeningOverview")]
        public async Task<IActionResult> GetBDMOpeningOverviewAsync(ReportRequestViewModel reportRequestViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetBDMOpeningOverview(reportRequestViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("RecruitersOverview")]
        public async Task<IActionResult> GetRecruitersOverviewAsync(ReportRequestViewModel reportRequestViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetRecruitersOverview(reportRequestViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("RecruiterOverview")]
        public async Task<IActionResult> RecruiterOverviewAsync(ReportRequestViewModel reportRequestViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetRecruiterOverview(reportRequestViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("RecruiterOpeningOverview")]
        public async Task<IActionResult> RecruiterOpeningOverviewAsync(ReportRequestViewModel reportRequestViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetRecruiterOpeningOverview(reportRequestViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("CandidatesSourceOverview")]
        public async Task<IActionResult> CandidatesSourceOverviewAsync(ReportRequestViewModel reportRequestViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetCandidatesSourceOverview(reportRequestViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("SourcedCandidates")]
        public async Task<IActionResult> SourcedCandidatesAsync(ReportRequestViewModel reportRequestViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetSourcedCandidates(reportRequestViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("SourcedWebsiteCandidates")]
        public async Task<IActionResult> SourcedWebsiteCandidatesAsync(ReportRequestViewModel reportRequestViewModel)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await reportRepository.GetSourcedWebsiteCandidates(reportRequestViewModel);
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
