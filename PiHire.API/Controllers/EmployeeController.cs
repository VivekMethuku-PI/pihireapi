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
    public class EmployeeController : BaseController
    {
        readonly ILogger<EmployeeController> logger;
        private readonly AppSettings _appSettings;
        private readonly IEmployeeRepository employeeRepository;

        public EmployeeController(ILogger<EmployeeController> logger,
            AppSettings appSettings,
            IEmployeeRepository employeeRepository, ILoggedUserDetails loginUserDetails) : base(employeeRepository, loginUserDetails)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appSettings = appSettings;
            this.employeeRepository = employeeRepository;
        }


        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            try
            {
                var response = await employeeRepository.GetEmployees();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("GetOfficeContact/{empId:int}")]
        public async Task<IActionResult> GetEmployees(int empId)
        {
            try
            {
                var response = await employeeRepository.GetEmployeeContactInfo(empId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}