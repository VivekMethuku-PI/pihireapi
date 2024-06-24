using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using PiHire.API.Common;
using PiHire.API.Common.Hubs;
using PiHire.API.Security;
using PiHire.BAL.Common.Extensions;
using PiHire.BAL.IRepositories;
using PiHire.BAL.Repositories;
using PiHire.BAL.ViewModels;
using PiHire.o_SD.Controllers;
using static PiHire.BAL.Common.Types.AppConstants;

namespace PiHire.API.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : BaseController
    {
        readonly ILogger<UserController> logger;
        private readonly AppSettings _appSettings;
        private readonly IUserRespository userRepository;
        JWTAutorization _autorization;
        public UserController(ILogger<UserController> logger,
            AppSettings appSettings, IHubContext<NotificationHub> hubContext,
            IUserRespository userRepository, JWTAutorization autorization, ILoggedUserDetails loginUserDetails, INotificationRepository notificationRepository) : base(userRepository, loginUserDetails, hubContext, notificationRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appSettings = appSettings;
            this.userRepository = userRepository;
            _autorization = autorization;
        }


        [HttpGet]
        [Route("GetRoles")]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                var response = await userRepository.GetProfiles();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await userRepository.GetAllUsersAsync();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("GetUsers/{puId:int}")]
        public async Task<IActionResult> GetUsersAsync(int puId)
        {
            try
            {

                var response = await userRepository.GetUsers(puId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("GetUsersbyType")]
        public async Task<IActionResult> GetUsersbyType(UsersbyTypeRequestModel usersbyTypeRequestModel)
        {
            try
            {
                var response = await userRepository.GetUsersbyType(usersbyTypeRequestModel.UserTypes, usersbyTypeRequestModel.PuId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("GetSignatureAuthorityUsers")]
        public async Task<IActionResult> GetSignatureAuthorityUsers()
        {
            try
            {
                var response = await userRepository.GetSignatureAuthorityUsers();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //[HttpPost]
        //[Route("Create")]
        //public async Task<IActionResult> Create(CreateUserRequestViewModel model)
        //{
        //    try
        //    {
        //        var response = await userRepository.CreateUser(model);
        //        return Ok(response);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        [HttpPost]
        [Route("Odoo/SaveUser")]
        public async Task<IActionResult> SaveUser(OddoCreateUserRequestViewModel model)
        {
            try
            {
                var response = await userRepository.OdooCreateUpdateUser(model);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("Odoo/Activate/{userId}")]
        public async Task<IActionResult> ActivateUser(int UserId)
        {
            try
            {
                var response = await userRepository.UpdateUserStatus(UserId, RecordStatus.Active);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("Odoo/InActivate/{userId}")]
        public async Task<IActionResult> InActivateUser(int UserId)
        {
            try
            {
                var response = await userRepository.UpdateUserStatus(UserId, RecordStatus.Inactive);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("Odoo/Suspend/{userId}")]
        public async Task<IActionResult> SuspendUser(int UserId)
        {
            try
            {
                var response = await userRepository.UpdateUserStatus(UserId, RecordStatus.Delete);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }




        [HttpPost]
        [Route("Odoo/IsExists")]
        public async Task<IActionResult> IsUserExists(UserExistModel userExistModel)
        {
            try
            {
                var response = await userRepository.IsUserExists(userExistModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }



        [HttpPost]
        [Route("Update")]
        public async Task<IActionResult> Update(UpdateUserViewModel model)
        {
            try
            {
                var response = await userRepository.UpdateUser(model);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("updatetxnlog")]
        public async Task<IActionResult> UpdateLoginTxnLog(UpdateLogsViewModel model)
        {
            try
            {
                var usr = LoggedUser.Usr;
                model.UserId = usr.Id;
                model.SessionId = usr.SessionTxnId;
                var response = await userRepository.UpdateUserLogs(model);
                if (!response.status) return BadRequest(response.message);
                //await updateOnlineStatus(usr.Id, model.LoginStatus);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("DisconnectUser")]
        public async Task<IActionResult> DisconnectUser(DisconnectUserViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.ConnectionId))
                {
                    return BadRequest("Invalid connectionId");
                }
                var usr = LoggedUser.Usr;
                var response = await userRepository.DisconnectUser(model.ConnectionId);
                if (!response.status) return BadRequest(response.message);

                await updateOnlineStatus(usr.Id, usr.UserDetails.Email, false);
                {
                    var lst = await userRepository.DisconnectSessionOutUsers();
                    foreach (var item in lst)
                    {
                        await updateOnlineStatus(item.userId, item.email, false);
                    }
                }
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("GetProfileDetails/{profileId:int}")]
        public async Task<IActionResult> GetProfileDetails(int profileId)
        {
            try
            {
                var response = await userRepository.GetProfileDetailsAsync(profileId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("GetUserProfiles/{PuId:int}")]
        public async Task<IActionResult> GetUserProfilesAync(int PuId)
        {
            try
            {
                var response = await userRepository.GetUserProfiles(PuId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpGet("Modules")]
        public async Task<IActionResult> GetModules()
        {
            try
            {
                var response = await userRepository.GetModules();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpGet("ModuleTasks/{mId:int}")]
        public async Task<IActionResult> ModuleTasks(int mId)
        {
            try
            {
                var response = await userRepository.GetTasks(mId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpGet("Tasks")]
        public async Task<IActionResult> GetTasks()
        {
            try
            {
                var response = await userRepository.GetTasks(null);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            try
            {
                var response = await userRepository.CreateRoleAsync(model);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("UpdateProfilePermissions")]
        public async Task<IActionResult> UpdateProfilePermissions(UpdateRolePermissionsViewModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequestCustom(ModelState);
            //}
            try
            {
                var response = await userRepository.UpdateProfilePermissionsAsync(model);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("UpdateUserPermissions")]
        public async Task<IActionResult> UpdateUserPermissions(UpdateUserPermissionsVM model)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequestCustom(ModelState);
            //}
            try
            {
                var response = await userRepository.UpdateUserPermissionsAsync(model);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("GetUserDetails/{userId:int}")]
        public async Task<IActionResult> GetUserDetails(int userId)
        {
            try
            {
                var response = await userRepository.GetUserDetails(userId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("UpdateUserStatus")]
        public async Task<IActionResult> UpdateUserStatus(UpdateUserStatusViewModel model)
        {
            try
            {
                var usr = LoggedUser.Usr;
                var response = await userRepository.UpdateUserStatus(model);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("UpdateUserProfilePhoto")]
        public async Task<IActionResult> UpdateUserProfilePhotoAsync(IFormCollection collection)
        {
            try
            {
                var userProfilePhotoViewModel = new UserProfilePhotoViewModel();
                if (Request.Form != null)
                {
                    userProfilePhotoViewModel.Photo = Request.Form.Files.FirstOrDefault(x => x.Name == "photo");
                }
                await TryUpdateModelAsync(userProfilePhotoViewModel);

                var response = await userRepository.UpdateUserProfilePhoto(userProfilePhotoViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> authenticate([FromBody] LoginViewModel model)
        {
            try
            {
                var user = await _autorization.Authenticate(model.username, model.password, null);
                if (user.Item1 == null)
                    return BadRequest(user.Item2 ?? "Username or password is incorrect");

                await updateOnlineStatus(user.Item1.Id, user.Item1.UserDetails.Email, true);
                return Ok(new { user = user.Item1, user.permissions, user.pubuLists });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpGet("authenticate/outlook")]
        public async Task<IActionResult> authenticateMicrosoftRequest()
        {
            try
            {
                var response = userRepository.authenticateMicrosoftRequest();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [AllowAnonymous]
        [HttpGet("authenticate/outlook/{RequestCode}/status")]
        public async Task<IActionResult> authenticateMicrosoft(string RequestCode)
        {
            try
            {
                var user = await _autorization.authenticateMicrosoft(RequestCode, null);
                if (user.IsCompleted == false)
                    return Ok(new { user.IsCompleted, user.IsSuccess, user.msg });
                else if (user.IsCompleted && user.IsSuccess == false)
                    return Ok(new { user.IsCompleted, user.IsSuccess, msg = user.msg ?? "Username is incorrect" });

                await updateOnlineStatus(user.data.Id, user.data.UserDetails.Email, true);
                return Ok(new { user.IsCompleted, user.IsSuccess, msg = string.Empty, user = user.data, user.MicrosoftToken, user.permissions, user.pubuLists });
            }
            catch (Exception)
            {
                throw;
            }
        }
        [AllowAnonymous]
        [HttpGet("authenticate/outlook/authResp")]
        public async Task<IActionResult> authenticateMicrosoftSetToken(string code, string state, string session_state, string error, string error_description)
        {
            try
            {
                logger.LogDebug("code:" + code + ",state:" + state + ",session_state:" + session_state + ",error:" + error + ",error_description:" + error_description);

                string rsp = await userRepository.authenticateMicrosoftSetToken(code, state, session_state, error);
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }


        [AllowAnonymous]
        [HttpPost("Odoo/authenticate")]
        public async Task<IActionResult> OdooAuthenticate([FromBody] LoginViewModel model)
        {
            try
            {
                var user = await _autorization.OdooAuthenticate(model.username, model.password, null);
                if (user.Item1 == null)
                    return BadRequest(user.Item2 ?? "Username or password is incorrect");

                return Ok(user.Item1);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [HttpGet("GetRecruiters")]
        public async Task<IActionResult> GetRecruiters()
        {
            try
            {
                var users = await userRepository.GetRecruiters();
                return Ok(users);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("GetAppUsers")]
        public async Task<IActionResult> GetAppUsers()
        {
            try
            {
                var users = await userRepository.GetAppUsers();
                return Ok(users);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("UpdateSignature")]
        public async Task<IActionResult> UpdateSignature(UpdateSignatureRequestViewModel model)
        {
            try
            {
                var users = await userRepository.UpdateSignature(model);
                return Ok(users);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpGet("GetUserPUBU/{userId:int}")]
        public async Task<IActionResult> GetUserPUBU(int userId)
        {
            try
            {
                var pu = await userRepository.GetUserPUBU(userId);
                return Ok(pu);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest model)
        {
            try
            {
                var response = await userRepository.ForgotPassword(model);
                return Ok(new { status = response.status, message = response.message });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("PasswordReset/{empId:int}")]
        public async Task<IActionResult> PasswordReset(int empId)
        {
            try
            {
                var response = await userRepository.PasswordReset(empId);
                return Ok(new { status = response.status, message = response.message });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest model)
        {
            try
            {
                var response = await userRepository.ResetPassword(model);
                return Ok(new { status = response.status, message = response.message });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}