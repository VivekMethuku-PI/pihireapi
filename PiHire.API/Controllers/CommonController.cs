using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PiHire.API.Common;
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
    public class CommonController : BaseController
    {
        readonly ILogger<CommonController> logger;
        private readonly AppSettings _appSettings;
        private readonly ICommonRepository commonRepository;

        public CommonController(ILogger<CommonController> logger,
            AppSettings appSettings,
            ICommonRepository commonRepository, ILoggedUserDetails loginUserDetails) : base(commonRepository, loginUserDetails)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appSettings = appSettings;
            this.commonRepository = commonRepository;
        }

        #region PU & BU

        [HttpGet]
        [Route("GetPU")]
        public async Task<IActionResult> GetPU()
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await commonRepository.GetPUAsync();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("GetBU")]
        public async Task<IActionResult> GetBU(GetBURequestVM getBURequestVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await commonRepository.GetBUAsync(getBURequestVM);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpGet]
        [Route("GetUserPuList")]
        public async Task<IActionResult> UserPuList()
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await commonRepository.UserPuListAsync();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("GetUserBuList")]
        public async Task<IActionResult> GetPU(GetBURequestVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await commonRepository.UserBuListAsync(model);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        [AllowAnonymous]
        [ResponseCache(Duration = 0)]
        [HttpGet]
        [Route("JobStatus")]
        public async Task<IActionResult> GetJobStatusAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await commonRepository.GetJobStatusAsync();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region User Remarks      

        [HttpPost("GetUserRemarks")]
        public async Task<IActionResult> ListUserRemarksAsync(UserRemarksRequestModel userRemarksRequestModel)
        {

            try
            {
                var response = await commonRepository.GetUserRemarksList(userRemarksRequestModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost("CreateUserRemark")]
        public async Task<IActionResult> CreateUserRemarkAsync(CreateUserRemarksModel createUserRemarksModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await commonRepository.CreateUserRemark(createUserRemarksModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut("UpdateUserRemark")]
        public async Task<IActionResult> UpdateUserRemarkAsync(UpdateUserRemarksModel updateUserRemarksModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await commonRepository.UpdateUserRemark(updateUserRemarksModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpDelete("DeleteUserRemark/{Id:int}")]
        public async Task<IActionResult> DeleteUserRemarkAsync(int Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await commonRepository.DeleteUserRemark(Id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Timezones
        [AllowAnonymous]
        [HttpGet]
        [Route("timezones/google")]
        public async Task<IActionResult> googleTimeZones()
        {
            try
            {
                var response = await commonRepository.getGoogleTimeZones();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("timezones/team")]
        public async Task<IActionResult> teamTimeZones()
        {
            try
            {
                var response = await commonRepository.getTeamTimeZones();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Enum properties 

        [AllowAnonymous]
        [HttpGet("Types")]
        public IActionResult GetTypes()
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = commonRepository.GetTypes();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }




        #endregion

        #region Ref Master 

        [AllowAnonymous]
        [HttpPost("RefData")]
        public async Task<IActionResult> GetRefData([FromBody] int[] GroupId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await commonRepository.GetRefData(GroupId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost("DocTypes")]
        public async Task<IActionResult> GetDocTypes([FromBody] int[] GroupId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await commonRepository.DocTypes(GroupId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }


        #endregion

        #region Countries & Cities 

        [AllowAnonymous]
        [HttpGet("GetCountries")]
        public async Task<IActionResult> GetCountriesAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await commonRepository.GetCountries();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpGet("GetCities/{CountryId:int}")]
        public async Task<IActionResult> GetCitiesAsync(int CountryId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await commonRepository.GetCities(CountryId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpPost("GetCities")]
        public async Task<IActionResult> GetCitiesAsync([FromBody] int?[] CountryId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await commonRepository.GetCities(CountryId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
        #region Salary calc

        [HttpGet("SalaryCalculator/FromCountries")]
        public async Task<IActionResult> SalaryCalculatorFromCountriesAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await commonRepository.SalaryCalculatorCountriesAsync(IsFrom: true);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet("SalaryCalculator/ToCountries")]
        public async Task<IActionResult> SalaryCalculatorToCountriesAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await commonRepository.SalaryCalculatorCountriesAsync(IsTo: true);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet("SalaryCalculator/{fromCountryId}/ToCountries")]
        public async Task<IActionResult> SalaryCalculatorToCountriesAsync(int fromCountryId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await commonRepository.SalaryCalculatorCountriesAsync(FromCountryId: fromCountryId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        #region Citywise benefits
        [HttpGet("City/{CityId}/Benefits")]
        public async Task<IActionResult> GetCityWiseBenefitsAsync(int CityId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await commonRepository.GetCityWiseBenefitsAsync(CityId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet("City/{CityId}/BenefitList")]
        public async Task<IActionResult> GetCityWiseBenefitListAsync(int CityId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await commonRepository.GetCityWiseBenefitListAsync(CityId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost("City/Benefit")]
        public async Task<IActionResult> GetCityWiseBenefitListAsync(CityWiseBenefitViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await commonRepository.SetCityWiseBenefitsAsync(model);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        #region Countrywise benefits
        [HttpGet("Country/{CountryId}/Benefits")]
        public async Task<IActionResult> GetCountryWiseBenefitsAsync(int CountryId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await commonRepository.GetCountryWiseBenefitsAsync(CountryId, null);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet("Country/{CountryId}/Benefits/Salary")]
        public async Task<IActionResult> GetCountryWiseBenefitsAsync_salary(int CountryId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await commonRepository.GetCountryWiseBenefitsAsync(CountryId, true);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet("Country/{CountryId}/BenefitList")]
        public async Task<IActionResult> GetCountryWiseBenefitListAsync(int CountryId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await commonRepository.GetCountryWiseBenefitListAsync(CountryId, null);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet("Country/{CountryId}/BenefitList/Salary")]
        public async Task<IActionResult> GetCountryWiseBenefitListAsync_salary(int CountryId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await commonRepository.GetCountryWiseBenefitListAsync(CountryId, true);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost("Country/Benefit")]
        public async Task<IActionResult> GetCountryWiseBenefitListAsync(CountryWiseBenefitViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await commonRepository.SetCountryWiseBenefitsAsync(model);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        #region Countrywise allowance
        [HttpGet("Country/{CountryId}/Allowances")]
        public async Task<IActionResult> GetCountryWiseAllowancesAsync(int CountryId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await commonRepository.GetCountryWiseAllowancesAsync(CountryId, null);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet("Country/{CountryId}/Allowances/CitizenWise/{isCitizenWise}")]
        public async Task<IActionResult> GetCountryWiseAllowancesAsync_salary(int CountryId, bool isCitizenWise)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await commonRepository.GetCountryWiseAllowancesAsync(CountryId, isCitizenWise);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet("Country/{CountryId}/AllowanceList")]
        public async Task<IActionResult> GetCountryWiseAllowanceListAsync(int CountryId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await commonRepository.GetCountryWiseAllowanceListAsync(CountryId, null);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost("Country/Allowance")]
        public async Task<IActionResult> SetCountryWiseAllowancesAsync(CountryWiseAllowanceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await commonRepository.SetCountryWiseAllowancesAsync(model);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Clients 

        [HttpGet("GetClients")]
        public async Task<IActionResult> GetClientsAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await commonRepository.GetClients();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("GetSpocs/{ClientId:int}")]
        public async Task<IActionResult> GetClientsAsync(int ClientId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await commonRepository.GetSpocs(ClientId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region  Assessments 

        [HttpGet("GetAssessments")]
        public async Task<IActionResult> GetAssessmentsAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await commonRepository.GetAssessments();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Media Gallery  

        [HttpPost("UploadMediaFile")]
        public async Task<IActionResult> UploadMediaFileAsync(IFormCollection collection)
        {

            var createMediaViewModel = new CreateMediaViewModel();
            if (Request.Form != null)
            {
                createMediaViewModel.File = Request.Form.Files.Where(x => x.Name == "File").FirstOrDefault();
            }

            await TryUpdateModelAsync(createMediaViewModel);

            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await commonRepository.UploadMediaFile(createMediaViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpGet("GetMediaFiles")]
        public async Task<IActionResult> GetMediaFilesAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await commonRepository.GetMediaFiles();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region SMPT 

        [HttpPost("ConfigureUserMail")]
        public async Task<IActionResult> ConfigureUserMailAsync(ConfigureSmptMailViewModel configureSmptMailViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await commonRepository.ConfigureUserMail(configureSmptMailViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("GetConfiguredMailDetails/{UserId:int}")]
        public async Task<IActionResult> GetConfiguredMailDetailsAsync(int UserId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await commonRepository.GetConfiguredUserMailDetails(UserId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// validate smtp cred
        /// </summary>
        /// <param name="userMailConfigureSuccessViewModel"></param>
        /// <returns></returns>
        //[AllowAnonymous]
        //[HttpPost("UserMailConfigureSuccess")]
        //public async Task<IActionResult> UserMailConfigureSuccess(UserMailConfigureSuccessViewModel userMailConfigureSuccessViewModel)
        //{

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequestCustom(ModelState);
        //    }
        //    try
        //    {
        //        var response = await commonRepository.UserMailConfigureSuccess(userMailConfigureSuccessViewModel);
        //        return Ok(response);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        //[HttpGet("outlook/token")]
        //public async Task<IActionResult> GetOutlookTokenAsync()
        //{
        //    try
        //    {
        //        var usr = LoggedUser.Usr;
        //        var response = await commonRepository.GetOutlookTokenAsync();
        //        return Ok(response);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        #endregion

        #region Listing Searchs model

        [AllowAnonymous]
        [HttpGet("SearchListModel")]
        public IActionResult GetSearchListModelAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = commonRepository.GetSearchListModel();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpGet("GetSkillDtls/{Id:Int}")]
        public IActionResult GetSkillDtlsAsync(int Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = commonRepository.GetSkillDtls(Id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Audits and Activies    


        [HttpPost("GetAuditList")]
        public async Task<IActionResult> GetAuditListAsync(AuditListSearchViewModel audit)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await commonRepository.GetAuditList(audit);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }




        [HttpPost("GetActivitiesList")]
        public async Task<IActionResult> GetActivitiesListAsync(ActivityListSearchViewModel activity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await commonRepository.GetActivitiesList(activity);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }



        #endregion

        #region Integrations

        [HttpGet("GetIntegrationList/{Category:int}")]
        public async Task<IActionResult> GetIntegrationsAsync(byte Category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await commonRepository.GetIntegrationList(Category);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPut("Integration/Active")]
        public async Task<IActionResult> ActiveIntegrationAsync([FromBody] int[] Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await commonRepository.UpdateIntegrationStatus(Id, RecordStatus.Active);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPut("Integration/InActive")]
        public async Task<IActionResult> InActiveIntegrationAsync([FromBody] int[] Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await commonRepository.UpdateIntegrationStatus(Id, RecordStatus.Inactive);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet("{category}/{subCategory}/ReqStatus")]
        public async Task<IActionResult> IntegrationReqStatus(IntegrationCategory category, int subCategory)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var ob = await commonRepository.GetUpdateIntegrationStatus(category, subCategory);
                return Ok(ob);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Google Meet
        [AllowAnonymous]
        [HttpGet("GoogleMeetSettings")]
        public IActionResult GoogleMeetSettings()
        {
            try
            {
                var obj = BAL.Common.Meeting.GoogleMeet.GetSettings();
                if (obj != null)
                    return Ok(new { obj.web.redirect_uris, obj.web.client_id });
                else
                    return BadRequest();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpGet("GoogleMeetResp")]
        public async Task<IActionResult> GoogleMeetResp(string access_token, string token_type, int? expires_in,
            string code, string scope, string id_token, string state, int? usrId, int? accountId, string error)
        {
            try
            {
                logger.LogDebug("access_token:" + access_token + ",token_type:" + token_type + ",expires_in:" + expires_in
                    + ",code:" + code + ",scope:" + scope + ",id_token:" + id_token + ",state:" + state
                    + ",usrId:" + usrId + ",accountId:" + accountId + ",error:" + error);
                if (string.IsNullOrEmpty(access_token) == false || string.IsNullOrEmpty(code) == false)
                {
                    string rsp = await commonRepository.SetGoogleMeetToken(access_token, code, state);
                    return Ok();
                }
                return BadRequest(new { msg = error == "access_denied" ? "Access denied" : "" });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("GoogleMeetTokenExist")]
        public async Task<IActionResult> GoogleMeetTokenExist()
        {
            try
            {
                var usr = LoggedUser.Usr;
                var tknObj = await commonRepository.CheckGoogleMeetCalendarTokenExist();

                return Ok(tknObj);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Team calendar
        [AllowAnonymous]
        [HttpGet("TeamCalendarAuthResp")]
        public async Task<IActionResult> TeamCalendarAuthResp(string code, string state, string session_state, string error, string error_description)
        {
            try
            {
                logger.LogDebug("code:" + code + ",state:" + state + ",session_state:" + session_state + ",error:" + error + ",error_description:" + error_description);

                string rsp = await commonRepository.SetTeamCalendarToken(code, state, session_state, error);
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet("TeamCalendarTokenExist")]
        public async Task<IActionResult> TeamCalendarToken()
        {
            try
            {
                var usr = LoggedUser.Usr;
                var tknObj = await commonRepository.CheckTeamCalendarTokenExist();
                return Ok(tknObj);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #endregion



    }
}