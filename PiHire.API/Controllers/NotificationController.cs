using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PiHire.API.Common;
using PiHire.API.Common.Hubs;
using PiHire.BAL.Common.Extensions;
using PiHire.BAL.IRepositories;
using PiHire.o_SD.Controllers;

namespace PiHire.API.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class NotificationController : BaseController
    {
        private readonly IHubContext<NotificationHub> hubContext;
        readonly ILogger<NotificationController> logger;
        private readonly AppSettings _appSettings;
        private readonly INotificationRepository notificationRepository;
        public NotificationController(ILogger<NotificationController> logger,
            AppSettings appSettings, IHubContext<NotificationHub> hubContext, INotificationRepository notificationRepository, ILoggedUserDetails loginUserDetails) : base(notificationRepository, loginUserDetails)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appSettings = appSettings;
            this.notificationRepository = notificationRepository;
            this.hubContext = hubContext;
        }

        private readonly List<NotificationViewModel> notifications = new List<NotificationViewModel>
        {
            new NotificationViewModel
            {
                Id = 1, IsSeen = true, Notification = "PDF's are shown in the embed tag.", CreatedDate=DateTime.Now.AddMinutes(-2), receivedFrom= new NotificationUser{ FirstName = "John", LastName="K", UserId = 10}
            },
            new NotificationViewModel
            {
                Id = 2, IsSeen = true, Notification = "with the Google Drive viewer.", CreatedDate=DateTime.Now.AddMinutes(-5), receivedFrom= new NotificationUser{ FirstName = "Mark", LastName="L", UserId = 60}
            },
            new NotificationViewModel
            {
                Id = 3, IsSeen = true, Notification = "viewers that should be loaded in an iframe." , CreatedDate=DateTime.Now.AddMinutes(-7), receivedFrom= new NotificationUser{ FirstName = "Kevin", LastName="S", UserId = 52}
            },
            new NotificationViewModel
            {
                Id = 4, IsSeen = true, Notification = "l browsers you better use PDF.js" , CreatedDate=DateTime.Now.AddMinutes(-20), receivedFrom= new NotificationUser{ FirstName = "Peter", LastName="R", UserId = 20}
            },
            new NotificationViewModel
            {
                Id = 5, IsSeen = false, Notification = "For the angular/cli you would add the" , CreatedDate=DateTime.Now.AddMinutes(-45)
            }
        };

        [HttpGet("UpdateReadStatus")]
        public async Task<IActionResult> UpdateReadStatusAsync()
        {
            var status = await notificationRepository.UpdateReadStatus();
            return Ok(status.Status);
        }

        /// <summary>
        /// this will return the all login user notification
        /// </summary>
        /// <returns></returns>
       
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var notifications =await  notificationRepository.GetUserNotifications();
            return Ok(notifications);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("PushNotificationTest")]
        public async Task<IActionResult> PushNotificationTestAsync()
        {
            await this.hubContext.Clients.All.SendAsync("messageReceivedFromApi", "Hello World... This is pushed notification.");
            return Ok();
        }


    }
    public class NotificationViewModel
    {
        public int Id { get; set; }
        public string Notification { get; set; }
        public bool IsSeen { get; set; }
        public DateTime CreatedDate { get; set; }
        public NotificationUser receivedFrom { get; set; }

    }

    public class NotificationUser
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

    }

}