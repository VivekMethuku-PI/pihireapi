using System;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PiHire.BAL.Common.Extensions;
using PiHire.BAL.IRepositories;
using PiHire.o_SD.Controllers;
using PiHire.BAL.ViewModels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using PiHire.API.Common;

namespace PiHire.API.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class TemplateController : BaseController
    {
        readonly ILogger<TemplateController> logger;
        private readonly AppSettings _appSettings;
        private readonly ITemplateRepository templateRepository;

        public TemplateController(ILogger<TemplateController> logger,
            AppSettings appSettings, ITemplateRepository templateRepository, ILoggedUserDetails loginUserDetails) : base(templateRepository, loginUserDetails)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appSettings = appSettings;
            this.templateRepository = templateRepository;
        }


        [HttpGet]
        public async Task<IActionResult> ListTemplates([FromQuery] byte MessageType, [FromQuery] int? IndustryId)
        {
            var templateSerachViewModel = new TemplateSerachViewModel
            {
                MessageType = MessageType,
                IndustryId = IndustryId
            };
            await TryUpdateModelAsync(templateSerachViewModel);
            try
            {
                var response = await templateRepository.GetTemplateList(templateSerachViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpGet("GetJobTemplate/{TemplateId:int}")]
        public async Task<IActionResult> GetTemplate(int TemplateId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response =await templateRepository.GetTemplate(TemplateId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateTemplateAsync(CreateJobTemplateViewModel createJobTemplateViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await templateRepository.CreateTemplate(createJobTemplateViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTemplateAsync(UpdateJobTemplateViewModel updateJobTemplateViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await templateRepository.UpdateTemplate(updateJobTemplateViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPut("UpdateTemplateStatus")]
        public async Task<IActionResult> UpdateTemplateStatusAsync(UpdateJobTemplateStatus updateJobTemplateStatus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await templateRepository.UpdateTemplateStatus(updateJobTemplateStatus);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPut("PublishTemplate")]
        public async Task<IActionResult> PublishTemplateAsync(UpdateJobTemplateStatus updateJobTemplateStatus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await templateRepository.PublishTemplate(updateJobTemplateStatus);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut("UnPublishTemplate")]
        public async Task<IActionResult> UnPublishTemplateAsync(UpdateJobTemplateStatus updateJobTemplateStatus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await templateRepository.UnPublishTemplate(updateJobTemplateStatus);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}