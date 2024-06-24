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
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace PiHire.API.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class BlogsController : BaseController
    {
        readonly ILogger<BlogsController> logger;
        private readonly AppSettings _appSettings;
        private readonly IBlogsRepository blogRepository;

        public BlogsController(ILogger<BlogsController> logger,
            AppSettings appSettings, IBlogsRepository blogRepository, ILoggedUserDetails loginUserDetails) : base(blogRepository, loginUserDetails)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appSettings = appSettings;
            this.blogRepository = blogRepository;
        }

        [HttpGet]
        public async Task<IActionResult> BlogsList()
        {
            try
            {
                var response = await blogRepository.BlogsList();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpGet("{BlogId:int}")]
        public async Task<IActionResult> GetBlog(int BlogId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await blogRepository.GetBlog(BlogId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpGet("GetActiveBlogs")]
        public async Task<IActionResult> GetActiveBlogsAsync()
        {
            try
            {
                var response = await blogRepository.GetActiveBlogs();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateBlogAsync(IFormCollection collection)
        {
            var createBlog = new CreateBlogModel();
            if (Request.Form != null)
            {
                createBlog.File = Request.Form.Files.FirstOrDefault(x => x.Name == "file");
            }
            await TryUpdateModelAsync(createBlog);
            try
            {
                var response = await blogRepository.CreateBlog(createBlog);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateBlogAsync(IFormCollection collection)
        {
            var updateBlogModel = new UpdateBlogModel();
            if (Request.Form != null)
            {
                updateBlogModel.File = Request.Form.Files.FirstOrDefault(x => x.Name == "file");
            }
            await TryUpdateModelAsync(updateBlogModel);
            try
            {
                var response = await blogRepository.UpdateBlog(updateBlogModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPut("UpdateBlogStatus")]
        public async Task<IActionResult> UpdateBlogStatusAysnc(UpdateStatusModel updateStatusModel)
        {
            try
            {
                var response = await blogRepository.UpdateBlogStatus(updateStatusModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
