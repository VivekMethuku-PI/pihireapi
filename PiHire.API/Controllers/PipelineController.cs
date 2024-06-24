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
namespace PiHire.API.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class PipelineController : BaseController
    {
        readonly ILogger<PipelineController> logger;
        private readonly AppSettings _appSettings;
        private readonly IAutomationRepository automationRepository;

        public PipelineController(ILogger<PipelineController> logger,
            AppSettings appSettings,
            IAutomationRepository automationRepository, ILoggedUserDetails loginUserDetails) : base(automationRepository, loginUserDetails)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appSettings = appSettings;
            this.automationRepository = automationRepository;
        }


        #region -  Candidate Status

        [HttpGet("CandidateStatus")]
        public async Task<IActionResult> GetCandidateStatusAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await automationRepository.GetCandidateStatus();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("CreateCandidateStatus")]
        public async Task<IActionResult> CreateCandidateStatusAsync(CreateCandidateStatusViewModel createCandidateStatusViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await automationRepository.CreateCandidateStatus(createCandidateStatusViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut("EditCandidateStatus")]
        public async Task<IActionResult> EditCandidateStatusAsync(EditCandidateStatusViewModel editCandidateStatusViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await automationRepository.EditCandidateStatus(editCandidateStatusViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut("UpdateCandidateStatus")]
        public async Task<IActionResult> UpdateCandidateStatusAsync(UpdateCandidateStatusViewModel updateCandidateStatusViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await automationRepository.UpdateCandidateStatus(updateCandidateStatusViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }


        #endregion

        #region - Opening Status

        [HttpGet("OpeningStatus")]
        public async Task<IActionResult> GetOpeningStatusAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await automationRepository.GetOpeningStatus();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("CreateOpeningStatus")]
        public async Task<IActionResult> CreateOpeningStatusAsync(CreateOpeningStatusViewModel createOpeningStatusViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await automationRepository.CreateOpeningStatus(createOpeningStatusViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut("EditOpeningStatus")]
        public async Task<IActionResult> EditOpeningStatusAsync(EditOpeningStatusViewModel editOpeningStatusViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await automationRepository.EditOpeningStatus(editOpeningStatusViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut("UpdateOpeningStatus")]
        public async Task<IActionResult> UpdateOpeningStatusAsync(UpdateOpeningStatusViewModel updateOpeningStatusViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await automationRepository.UpdateOpeningStatus(updateOpeningStatusViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region  - Hire Pipeline 

        [HttpPost("CreateStage")]
        public async Task<IActionResult> CreateStageAsync(CreateStageViewModel createStageViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await automationRepository.CreateStage(createStageViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut("EditStage")]
        public async Task<IActionResult> EditStageAsync(EditStageViewModel editStageViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await automationRepository.EditStage(editStageViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpDelete("Stage/{Id:int}")]
        public async Task<IActionResult> DeleteStageAsync(int Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await automationRepository.DeleteStage(Id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("MapCandidateStatus")]
        public async Task<IActionResult> MapCandidateStatusAsync(MapCandidateStatusViewModel mapCandidateStatusViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await automationRepository.MapCandidateStatus(mapCandidateStatusViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpDelete("StageCandidateStatus/{Id:int}")]
        public async Task<IActionResult> DeleteStageCandidateStatusAsync(int Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await automationRepository.DeleteStageCandidateStatus(Id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Stages")]
        public async Task<IActionResult> StagesAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await automationRepository.Stages();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("StageCandidateStatus")]
        public async Task<IActionResult> StageCandidateStatusAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await automationRepository.StageCandidateStatus();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region  - Display Rules 

        [HttpGet("DisplayRules")]
        public async Task<IActionResult> DisplayRulesAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await automationRepository.DisplayRules();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }       

        [HttpPost("CreateDisplayRule")]
        public async Task<IActionResult> CreateDisplayRuleAsync(CreateDisplayRuleViewmodel createDisplayRuleViewmodel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await automationRepository.CreateDisplayRule(createDisplayRuleViewmodel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut("UpdateDisplayRule")]
        public async Task<IActionResult> UpdateDisplayRuleAsync(UpdateDisplayRuleViewmodel updateDisplayRuleViewmodel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await automationRepository.UpdateDisplayRule(updateDisplayRuleViewmodel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut("EditDisplayRule")]
        public async Task<IActionResult> EditDisplayRuleAsync(EditDisplayRuleViewmodel editDisplayRuleViewmodel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await automationRepository.EditDisplayRule(editDisplayRuleViewmodel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpDelete("DisplayRule/{Id:int}")]
        public async Task<IActionResult> DeleteDisplayRuleAsync(int Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await automationRepository.DeleteDisplayRule(Id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("GetNextCandidateStatus/{StatusId:int}")]
        public async Task<IActionResult> GetNextCandidateStatusAsync(int StatusId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await automationRepository.GetNextCandidateStatus(StatusId);
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