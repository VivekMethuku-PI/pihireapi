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
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace PiHire.API.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class TestimonialsController : BaseController
    {
        readonly ILogger<TestimonialsController> logger;
        private readonly AppSettings _appSettings;
        private readonly ITestimonialRepository testimonialRepository;

        public TestimonialsController(ILogger<TestimonialsController> logger,
            AppSettings appSettings, ITestimonialRepository testimonialRepository, ILoggedUserDetails loginUserDetails) : base(testimonialRepository, loginUserDetails)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appSettings = appSettings;
            this.testimonialRepository = testimonialRepository;
        }


        [HttpGet]
        public async Task<IActionResult> TestimonialsList()
        {
            try
            {
                var response = await testimonialRepository.TestimonialsList();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpGet("{TestimonialId:int}")]
        public async Task<IActionResult> GetTestimonial(int TestimonialId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await testimonialRepository.GetTestimonial(TestimonialId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpGet("GetActiveTestimonials")]
        public async Task<IActionResult> GetActiveTestimonialsAsync()
        {
            try
            {
                var response = await testimonialRepository.GetActiveTestimonials();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateTestimonialAsync(IFormCollection collection)
        {
            var createTestimonialsModel = new CreateTestimonialModel();
            if (Request.Form != null)
            {
                createTestimonialsModel.File = Request.Form.Files.FirstOrDefault(x => x.Name == "file");
            }
            await TryUpdateModelAsync(createTestimonialsModel);
            try
            {
                var response = await testimonialRepository.CreateTestimonial(createTestimonialsModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTestimonialAsync(IFormCollection collection)
        {
            var updateTestimonialsModel = new UpdateTestimonialModel();
            if (Request.Form != null)
            {
                updateTestimonialsModel.File = Request.Form.Files.FirstOrDefault(x => x.Name == "file");
            }
            await TryUpdateModelAsync(updateTestimonialsModel);
            try
            {
                var response = await testimonialRepository.UpdateTestimonial(updateTestimonialsModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPut("UpdateTestimonialStatus")]
        public async Task<IActionResult> UpdateTestimonialStatusAysnc(UpdateStatusModel updateStatusModel)
        {          
            try
            {
                var response = await testimonialRepository.UpdateTestimonialStatus(updateStatusModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
