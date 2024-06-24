using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PiHire.API.Common;
using PiHire.API.Security;
using PiHire.BAL.Common.Extensions;
using PiHire.BAL.IRepositories;
using PiHire.BAL.ViewModel;
using PiHire.o_SD.Controllers;

namespace PiHire.API.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class CompanyController : BaseController
    {
        readonly ILogger<CompanyController> logger;
        private readonly AppSettings _appSettings;
        private readonly ICompanyRepository companyRepository;
        JWTAutorization _autorization;

        public CompanyController(ILogger<CompanyController> logger,
            AppSettings appSettings,
            ICompanyRepository companyRepository, JWTAutorization autorization, ILoggedUserDetails loginUserDetails) : base(companyRepository, loginUserDetails)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appSettings = appSettings;
            this.companyRepository = companyRepository;
            _autorization = autorization;
        }

        [HttpGet]
        public async Task<IActionResult> GetCompaniesAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await companyRepository.GetCompanies();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("GetCompany")]
        public async Task<IActionResult> GetCompanyAsync([FromQuery] int Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await companyRepository.GetCompany(Id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }


      
        [HttpPost("Odoo/SaveCompany")]
        public async Task<IActionResult> CreateUpdateCompany(CreateUpdateProcessUnitViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await companyRepository.CreateUpdateCompany(model);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [AllowAnonymous]
        [HttpPost("Odoo/SaveBusinessUnit")]
        public async Task<IActionResult> CreateUpdateCompanyBusinessUnit(CreateUpdateProcessUnitBusinessUnitViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await companyRepository.CreateUpdateCompanyBusinessUnit(model);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost("Odoo/SaveLocation")]
        public async Task<IActionResult> SaveCompanyLocation(CreateUpdateProcessUnitLocationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await companyRepository.CreateUpdateCompanyLocation(model);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}