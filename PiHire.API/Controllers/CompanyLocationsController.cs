using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PiHire.API.Common;
using PiHire.BAL.Common.Extensions;
using PiHire.BAL.IRepositories;
using PiHire.o_SD.Controllers;

namespace PiHire.API.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class CompanyLocationsController : BaseController
    {

        readonly ILogger<CompanyLocationsController> logger;
        private readonly AppSettings _appSettings;
        private readonly ICompanyRepository companyRepository;

        public CompanyLocationsController(ILogger<CompanyLocationsController> logger,
            AppSettings appSettings,
            ICompanyRepository companyRepository, ILoggedUserDetails loginUserDetails) : base(companyRepository, loginUserDetails)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appSettings = appSettings;
            this.companyRepository = companyRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanyLocationsAsync([FromQuery] int Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await companyRepository.GetCompanyLocations(Id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

     
        [HttpPost("GetCompanyLocations")]
        public async Task<IActionResult> GetCompanyLocationsAsync([FromBody] int[] Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await companyRepository.GetCompanyLocations(Id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpGet("GetCompanyLocation")]
        public async Task<IActionResult> GetCompanyLocationAsync([FromQuery] int cId, [FromQuery] int locId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await companyRepository.GetCompanyLocation(cId, locId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }



        [HttpGet("Recruiter/all")]
        public async Task<IActionResult> GetRecruiterLocationsAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await companyRepository.GetUserTypeLocationsAsync(BAL.Common.Types.AppConstants.UserType.Recruiter);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet("BDM/all")]
        public async Task<IActionResult> GetBDMLocationsAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await companyRepository.GetUserTypeLocationsAsync(BAL.Common.Types.AppConstants.UserType.BDM);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}