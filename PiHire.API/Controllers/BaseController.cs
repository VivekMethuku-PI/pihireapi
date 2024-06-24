using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using NPOI.SS.Formula.Functions;
using PiHire.API.Common;
using PiHire.API.Common.Hubs;
using PiHire.BAL.Common.Logging;
using PiHire.BAL.IRepositories;
using PiHire.BAL.Repositories;
using PiHire.BAL.ViewModels;
using PiHire.BAL.ViewModels.ApiBaseModels;
using System.Collections.Generic;

namespace PiHire.o_SD.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {

        public UserAuthorizationViewModel Usr { get; set; }
        public LoggedUserDetails _loggedUser;
        internal LoggedUserDetails LoggedUser
        {
            get
            {
                if (_loggedUser == null)
                {
                    _loggedUser = new LoggedUserDetails(HttpContext);
                }
                return _loggedUser ?? (_loggedUser = new LoggedUserDetails(HttpContext));
            }
        }
        private readonly IHubContext<NotificationHub> hubContext;
        private readonly INotificationRepository notificationRepository;
        public BaseController(IBaseRepository repository, ILoggedUserDetails login)
        {
            repository.Usr = login.Usr;
            Usr = login.Usr;
        }

        public BaseController(IBaseRepository repository, ILoggedUserDetails login, IHubContext<NotificationHub> hubContext, INotificationRepository notificationRepository)
        {
            repository.Usr = login.Usr;
            Usr = login.Usr;
            this.hubContext = hubContext;
            this.notificationRepository = notificationRepository;
        }

        ISecurityRepository securityRepository;
        [ApiExplorerSettings(IgnoreApi = true)]
        public BadRequestObjectResult BadRequestCustom(ModelStateDictionary modelState)
        {
            securityRepository = (ISecurityRepository)Request.HttpContext.RequestServices.GetService(typeof(ISecurityRepository));
            return _BadRequestCustom(modelState);
        }
        BadRequestObjectResult _BadRequestCustom(ModelStateDictionary modelState)
        {
            var resp = new GetResponseViewModel<string>();
            string msg = string.Empty;
            foreach (var item in ModelState)
            {
                if (item.Value?.Errors != null)
                {
                    foreach (var err in item.Value.Errors)
                    {
                        msg += "," + err.ErrorMessage;
                    }
                }
            }
            resp.Status = false;
            resp.Meta.SetError(ApiResponseErrorCodes.InvalidJsonFormat, msg.Length > 0 ? msg.Substring(1) : msg, true);
            resp.Result = null;

            if (securityRepository != null)
                securityRepository.LogMessage(LogLevel.Debug, LoggingEvents.MandatoryDataMissing, msg.Length > 0 ? msg.Substring(1) : msg ?? "", resp.Meta.RequestID);
            return BadRequest(resp);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async System.Threading.Tasks.Task PushNotificationToClientAsync(List<NotificationPushedViewModel> model)
        {
            var notificationStatus = await notificationRepository.SaveNotification(model);
            if (notificationStatus.Status)
            {
                foreach (var item in model)
                {
                    if (!string.IsNullOrEmpty(item.NoteDesc))
                    {
                        List<string> connectionsIds = await notificationRepository.GetConnectionsIds(item.PushedTo);
                        await this.hubContext.Clients.Clients(connectionsIds).SendAsync("notificationReceivedFromApi", Newtonsoft.Json.JsonConvert.SerializeObject(item));
                    }
                }
            }
        }




        internal async System.Threading.Tasks.Task updateOnlineStatus(int userId, string email, bool isLoggedIn)
        {
            List<string> connectionsIds = await notificationRepository.GetConnectionsIds(null);
            var MessageModel = new
            {
                userId = userId,
                isLoggedIn = isLoggedIn,
                email = email
            };
            await this.hubContext.Clients.Clients(connectionsIds).SendAsync("onlineStatusUpdateUser", Newtonsoft.Json.JsonConvert.SerializeObject(MessageModel));
        }
    }
}