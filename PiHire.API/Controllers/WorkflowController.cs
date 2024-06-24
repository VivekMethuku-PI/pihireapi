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
    public class WorkflowController : BaseController
    {
        readonly ILogger<WorkflowController> logger;
        private readonly AppSettings _appSettings;
        private readonly IWorkflowRepository workflowRepository;

        public WorkflowController(ILogger<WorkflowController> logger,
            AppSettings appSettings,
            IWorkflowRepository workflowRepository, ILoggedUserDetails loginUserDetails) : base(workflowRepository, loginUserDetails)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appSettings = appSettings;
            this.workflowRepository = workflowRepository;
        }
               

        [HttpGet("WorkflowRule/{Id}")]
        public async Task<IActionResult> GetWorkflowRuleAsync(int Id)
        {
            try
            {
                var response = await workflowRepository.GetWorkflowRule(Id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpGet("WorkflowRules/{sort}")]
        public async Task<IActionResult> WorkflowRulesAsync(byte sort)
        {            
            try
            {
                var response = await workflowRepository.GetWorkflowRules(sort);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateWorkflowRuleAsync(CreateWorkflowRuleViewmodel WorkflowRuleViewmodel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await workflowRepository.CreateWorkflowRule(WorkflowRuleViewmodel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateWorkflowRuleAsync(EditWorkflowRuleViewmodel WorkflowRuleViewmodel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await workflowRepository.UpdateWorkflowRule(WorkflowRuleViewmodel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpDelete("WorkflowRule/{Id:int}")]
        public async Task<IActionResult> DeleteWorkflowRuleAsync(int Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await workflowRepository.DeleteWorkflowRule(Id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpDelete("WorkflowRuleDetails/{Id:int}")]
        public async Task<IActionResult> DeleteWorkflowDetailsAsync(int Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await workflowRepository.DeleteWorkflowDetails(Id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}