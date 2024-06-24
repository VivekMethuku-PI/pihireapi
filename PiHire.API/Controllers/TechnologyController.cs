using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PiHire.API.Common;
using PiHire.BAL.Common.Extensions;
using PiHire.BAL.IRepositories;
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
    public class TechnologyController : BaseController
    {
        readonly ILogger<TechnologyController> logger;
        private readonly AppSettings _appSettings;
        private readonly ITechnologyRepository technologyRepository;

        public TechnologyController(ILogger<TechnologyController> logger,
            AppSettings appSettings,
            ITechnologyRepository technologyRepository, ILoggedUserDetails loginUserDetails) : base(technologyRepository, loginUserDetails)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appSettings = appSettings;
            this.technologyRepository = technologyRepository;
        }


        #region  Technology
        [HttpGet]
        public async Task<IActionResult> GetTechnologiesAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await technologyRepository.GetTechnologies();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("GetTechnologies")]
        public async Task<IActionResult> GetTechnologiesAsync(GetTechnologiesViewModel getTechnologiesViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await technologyRepository.GetTechnologies(getTechnologiesViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
             


        [HttpPost("CreateTechnology")]
        public async Task<IActionResult> CreateTechnologyAsync(CreateTechnologiesViewModel createTechnologiesViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await technologyRepository.CreateTechnology(createTechnologiesViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("UpdateTechnology")]
        public async Task<IActionResult> UpdateTechnologyAsync(UpdateTechnologiesViewModel updateTechnologiesViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await technologyRepository.UpdateTechnology(updateTechnologiesViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpDelete("Delete/{Id:int}")]
        public async Task<IActionResult> DeleteTechnologyAsync(int Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await technologyRepository.DeleteTechnology(Id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion


        #region Technology Group 

        [HttpGet("GetSkillProfileTechnologies/{id:int}")]
        public async Task<IActionResult> GetTechnologyGroupAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await technologyRepository.GetTechnologyGroup(id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("CreateSkillProfile")]
        public async Task<IActionResult> CreateTechnologyGroupAsync(CreateSkillProfileViewModel createSkillProfileViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await technologyRepository.CreateTechnologyGroup(createSkillProfileViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("UpdateSkillProfile")]
        public async Task<IActionResult> UpdateTechnologyGroupAsync(UpdateSkillProfileViewModel updateSkillProfileViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await technologyRepository.UpdateTechnologyGroup(updateSkillProfileViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("SkillProfiles")]
        public async Task<IActionResult> TechnologyGroupsAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await technologyRepository.TechnologyGroups();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpDelete("{Id:int}")]
        public async Task<IActionResult> DeleteTechnologyGroupAsync(int Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await technologyRepository.DeleteTechnologyGroup(Id);
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
