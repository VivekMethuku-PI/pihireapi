using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PiHire.API.Common;
using PiHire.API.Security;
using PiHire.BAL.Common.Extensions;
using PiHire.BAL.Common.Types;
using PiHire.BAL.IRepositories;
using PiHire.BAL.ViewModels;
using PiHire.o_SD.Controllers;
using System;
using System.Net;
using System.Threading.Tasks;

namespace PiHire.API.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class RefController : BaseController
    {
        readonly ILogger<RefController> logger;
        private readonly AppSettings _appSettings;
        private readonly IRefRepository refRepository;
        JWTAutorization _autorization;

        public RefController(ILogger<RefController> logger,
            AppSettings appSettings,
            IRefRepository refRepository, JWTAutorization autorization, ILoggedUserDetails loginUserDetails) : base(refRepository, loginUserDetails)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appSettings = appSettings;
            this.refRepository = refRepository;
            _autorization = autorization;
        }

        [HttpGet]
        public async Task<IActionResult> GetRefListAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await refRepository.GetRefList();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpPost("RefData")]
        public async Task<IActionResult> GetRefDataAsync([FromBody] int[] GroupId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await refRepository.GetRefData(GroupId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("GetRefData/{id:int}")]
        public async Task<IActionResult> GetRefDataAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await refRepository.GetRefData(id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateRefValueAsync(CreateRefValuesViewModel createRefValuesViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await refRepository.CreateRefValue(createRefValuesViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRefValueAsync(UpdateRefValuesViewModel updateRefValuesViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await refRepository.UpdateRefValue(updateRefValuesViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpDelete("{Id:int}")]
        public async Task<IActionResult> DeleteRefValueAsync(int Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await refRepository.UpdateTemplateStatus(Id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Job opening data
        #region Get
        [HttpGet("Job/Tenures")]
        public async Task<IActionResult> GetRefDataAsync_JobTenures()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await refRepository.GetRefData(ReferenceGroupType.JobTenure);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet("Job/Domains")]
        public async Task<IActionResult> GetRefDataAsync_JobDomains()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await refRepository.GetRefData(ReferenceGroupType.JobDomain);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet("Job/TeamRoles")]
        public async Task<IActionResult> GetRefDataAsync_JobTeamRoles()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await refRepository.GetRefData(ReferenceGroupType.JobTeamRole);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet("Job/WorkPatterns")]
        public async Task<IActionResult> GetRefDataAsync_JobWorkPatterns()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await refRepository.GetRefData(ReferenceGroupType.JobWorkPattern);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet("Job/Qualifications")]
        public async Task<IActionResult> GetRefDataAsync_JobQualification()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await refRepository.GetRefData(ReferenceGroupType.JobQualification);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet("Job/Certifications")]
        public async Task<IActionResult> GetRefDataAsync_JobCertifications()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await refRepository.GetRefData(ReferenceGroupType.JobCertification);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [AllowAnonymous]
        [HttpGet("JobCandiate/Genders")]
        public async Task<IActionResult> GetRefDataAsync_JobCandidateGender()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await refRepository.GetRefData(ReferenceGroupType.JobCandidateGender);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [AllowAnonymous]
        [HttpGet("JobCandiate/MaritalStatus")]
        public async Task<IActionResult> GetRefDataAsync_JobCandidateMaritalStatus()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await refRepository.GetRefData(ReferenceGroupType.JobCandidateMaritalStatus);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet("JobCandiate/LanguagePreferences")]
        public async Task<IActionResult> GetRefDataAsync_JobCandidateLanguagePreferences()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await refRepository.GetRefData(ReferenceGroupType.JobCandidateLanguagePreference);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet("JobCandiate/VisaPreferences")]
        public async Task<IActionResult> GetRefDataAsync_JobCandidateVisaPreference()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await refRepository.GetRefData(ReferenceGroupType.JobCandidateVisaPreference);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet("JobCandiate/Regions")]
        public async Task<IActionResult> GetRefDataAsync_JobCandidateRegion()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await refRepository.GetRefData(ReferenceGroupType.JobCandidateRegion);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet("JobCandiate/DrivingLicenses")]
        public async Task<IActionResult> GetRefDataAsync_JobCandidateDrivingLicense()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await refRepository.GetRefData(ReferenceGroupType.JobCandidateDrivingLicense);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet("JobCandiate/EducationCertifications")]
        public async Task<IActionResult> GetRefDataAsync_CandidateEducationCertifications()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await refRepository.GetRefData(ReferenceGroupType.CandidateEducationCertification);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet("JobCandiate/EducationQualifications/Graduation")]
        public async Task<IActionResult> GetCandidateEducationQualifications_Graduations()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await refRepository.GetRefData(ReferenceGroupType.CandidateEducationQualification_Graduation);
                //var response = await refRepository.GetCandidateEducationQualifications_Graduations();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet("JobCandiate/EducationQualifications/Graduation/{GraduationId}/Specialization")]
        public async Task<IActionResult> GetCandidateEducationQualifications_GraduationSpecializations(int GraduationId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await refRepository.GetRefData(ReferenceGroupType.CandidateEducationQualification_Course);
                //var response = await refRepository.GetCandidateEducationQualifications_GraduationSpecializations(GraduationId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        #region Set
        [HttpPost("Job/Domain")]
        public async Task<IActionResult> SetRefDataAsync_JobDomain(CreateReferenceViewModel model)
        {
            return await SetRefDataAsync_(ReferenceGroupType.JobDomain, model);
        }
        [HttpPost("Job/TeamRole")]
        public async Task<IActionResult> SetRefDataAsync_JobTeamRole(CreateReferenceViewModel model)
        {
            return await SetRefDataAsync_(ReferenceGroupType.JobTeamRole, model);
        }
        [HttpPost("JobCandiate/LanguagePreference")]
        public async Task<IActionResult> SetRefDataAsync_JobCandidateLanguagePreference(CreateReferenceViewModel model)
        {
            return await SetRefDataAsync_(ReferenceGroupType.JobCandidateLanguagePreference, model);
        }
        [HttpPost("JobCandiate/VisaPreference")]
        public async Task<IActionResult> SetRefDataAsync_JobCandidateVisaPreference(CreateReferenceViewModel model)
        {
            return await SetRefDataAsync_(ReferenceGroupType.JobCandidateVisaPreference, model);
        }
        [HttpPost("JobCandiate/Region")]
        public async Task<IActionResult> SetRefDataAsync_JobCandidateRegion(CreateReferenceViewModel model)
        {
            return await SetRefDataAsync_(ReferenceGroupType.JobCandidateRegion, model);
        }

        async Task<IActionResult> SetRefDataAsync_(ReferenceGroupType type, CreateReferenceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var response = await refRepository.CreateRefValue(new CreateRefValuesViewModel
                {
                    GroupId = (int)type,
                    Rmtype = type + "",
                    Rmvalue = model.Rmvalue,
                    Rmdesc = model.Rmdesc ?? ""
                });
                return Ok(response);
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
