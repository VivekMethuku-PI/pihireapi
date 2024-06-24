using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PiHire.API.Common;
using PiHire.API.Common.Hubs;
using PiHire.API.Security;
using PiHire.BAL.Common.Extensions;
using PiHire.BAL.IRepositories;
using PiHire.BAL.ViewModels;
using PiHire.o_SD.Controllers;

namespace PiHire.API.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AccountController : BaseController
    {
        private readonly IHubContext<NotificationHub> hubContext;
        readonly ILogger<AccountController> logger;
        private readonly AppSettings _appSettings;
        private readonly IAccountRepository accountRepository;
        JWTAutorization _autorization;
        public AccountController(ILogger<AccountController> logger,
            AppSettings appSettings,
            IAccountRepository accountRepository, IHubContext<NotificationHub> hubContext, ILoggedUserDetails loginUserDetails, JWTAutorization autorization, INotificationRepository notificationRepository) : base(accountRepository, loginUserDetails, hubContext, notificationRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appSettings = appSettings;
            this.accountRepository = accountRepository;
            _autorization = autorization;
            this.hubContext = hubContext;
        }

        [AllowAnonymous]
        [HttpPost("LogoutDevices")]
        public async Task<IActionResult> LogoutDevices(LogoutDevicesVm model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var user = await accountRepository.LogoutFromAllDevices(model.Token, model.Email);
                if (!user.status)
                    return BadRequest(user.message);
                if (user.deviceId != null)
                {
                    await this.hubContext.Clients.Clients(user.deviceId).SendAsync("logoutFromAllDevices", "");
                }
                return Ok(user.status);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> authenticate(AccountAuthenticate model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var user = await _autorization.Authenticate(model, null);
                if (user.Item1 == null)
                    return BadRequest(user.Item2 ?? "Username or password is incorrect");

                await updateOnlineStatus(user.Item1.Id, user.Item1.UserDetails.Email, true);
                return Ok(user.Item1);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpPost("authenticate/google")]
        public async Task<IActionResult> AuthenticateGoogle(AccountAuthenticateGoogle model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var user = await _autorization.Authenticate(model, null);
                if (user.Item1 == null)
                    return BadRequest(user.Item2 ?? "Username or password is incorrect");

                await updateOnlineStatus(user.Item1.Id, user.Item1.UserDetails.Email, true);
                return Ok(user.Item1);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpPost("authenticate/facebook")]
        public async Task<IActionResult> AuthenticateFacebook(AccountAuthenticateFacebook model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var user = await _autorization.Authenticate(model, null);
                if (user.Item1 == null)
                    return BadRequest(user.Item2 ?? "Username or password is incorrect");

                await updateOnlineStatus(user.Item1.Id, user.Item1.UserDetails.Email, true);
                return Ok(user.Item1);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpPost("candidate/register")]
        public async Task<IActionResult> CandidateRegister(CandidateRegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var register = await accountRepository.CandidateRegistration(model);
                return Ok(register);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpPost("candidate/register/google")]
        public async Task<IActionResult> CandidateRegisterGoogle(CandidateRegistrationGoogleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var register = await accountRepository.CandidateRegistrationGoogle(model);
                return Ok(register);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpPost("candidate/register/facebook")]
        public async Task<IActionResult> CandidateRegisterFacebook(CandidateRegistrationFacebookViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var register = await accountRepository.CandidateRegistrationFacebook(model);
                return Ok(register);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpPost("emailverify")]
        public async Task<IActionResult> EmailVerify(EmailVerifyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var varify = await accountRepository.EmailVarify(model.Token);
                return Ok(varify);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest model)
        {
            try
            {
                var response = await accountRepository.ForgotPassword(model);
                return Ok(new { status = response.status, message = response.message });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest model)
        {
            try
            {
                var response = await accountRepository.ResetPassword(model);
                return Ok(new { status = response.status, message = response.message });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("GetUserCurrentStatus")]
        public async Task<IActionResult> GetUserCurrentStatus(OnlineStatusFilterViewModel filterViewModel)
        {
            try
            {
                //if (UserIds.Count > 50)
                //{
                //    return BadRequest("user list contains more than 50 id");
                //}
                var response = await accountRepository.GetUserCurrentStatus(filterViewModel);
                if (!response.status)
                {
                    return BadRequest(response.message);
                }
                return Ok(response.data);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}