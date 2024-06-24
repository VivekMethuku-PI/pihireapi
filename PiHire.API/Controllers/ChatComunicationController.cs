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
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using PiHire.API.Common;
using PiHire.API.Common.Hubs;
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
    public class ChatComunicationController : BaseController
    {
        private readonly IHubContext<NotificationHub> hubContext;
        readonly ILogger<ChatComunicationController> logger;
        private readonly AppSettings _appSettings;
        private readonly IChatComunicationRepository chatRepository;
        private readonly INotificationRepository notificationRepository;

        public ChatComunicationController(ILogger<ChatComunicationController> logger,
            AppSettings appSettings,
            IChatComunicationRepository chatRepository, ILoggedUserDetails loginUserDetails, IHubContext<NotificationHub> hubContext, INotificationRepository notificationRepository) : base(chatRepository, loginUserDetails, hubContext, notificationRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appSettings = appSettings;
            this.chatRepository = chatRepository;
            this.notificationRepository = notificationRepository;
            this.hubContext = hubContext;
        }
        /// <summary>
        /// This API is save the message and file, and push the data to room user 
        /// </summary>
        /// <param name="formData"></param>
        /// <returns></returns>
        [HttpPost("SendChatMessages")]
        public async Task<IActionResult> SendChatMessages([FromForm]SendMessageModel formData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var model = JsonConvert.DeserializeObject<SendMessageViewModel>(formData.Model);
                var messageTime = DateTime.UtcNow;
                var messages = await chatRepository.SaveChatMessage(model, formData.File, messageTime);
                if (messages)
                {
                    List<string> connectionsIds = await notificationRepository.GetConnectionsIds(new int[] { model.ReceiverId, Usr.Id });
                    var MessageModel = new GetChatComunicationTempViewModel
                    {
                        fileName = null,
                        message = model.Message,
                        readStatus = 0, 
                        receiverId = model.ReceiverId,
                        receiverName = model.ReceiverName,
                        senderId = Usr.Id,
                        senderName = Usr.Name,
                        roomId = model.ChatRoomId,
                        createdDate = messageTime
                    };
                    await this.hubContext.Clients.Clients(connectionsIds).SendAsync("chatMessageReceived", JsonConvert.SerializeObject(MessageModel));
                }
                
                return Ok(messages);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        /// <summary>
        /// This will return the media files list based on roomId
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        [HttpGet("GetRoomMedia/{roomId}")]
        public async Task<IActionResult> GetRoomMedia(int roomId)
        {
            try
            {
                var media = await chatRepository.GetRoomMedia(roomId);
                return Ok(media);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        /// <summary>
        /// Get all messages between candidate and employee for perticular job
        /// If receiver and sender dont have room it will get create and return the empty list.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("GetChatMessages")]
        public async Task<IActionResult> GetChatMessages(GetChatMessagesViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var messages = await chatRepository.GetChatMessages(model);
                if (!messages.status)
                    return BadRequest("Something went wrong, Not able to get messages");
                return Ok(messages.data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// it will return the room based on the job id and login user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("GetRoomsByJob")]
        public async Task<IActionResult> GetRoomsByJob(GetRoomReuestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var rooms = await chatRepository.GetRoomsByJob(model);
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("UpdateReadStatus")]
        public async Task<IActionResult> UpdateReadStatus(UpdateReadStatusViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var status = await chatRepository.UpdateReadStatus(model);
                return Ok(status);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// it will all messages not based on room
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllChatMessages")]
        public async Task<IActionResult> GetAllChatMessages()
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var messages = await chatRepository.GetAllChatMessages();
                if (!messages.status)
                    return BadRequest("Something went wrong, Not able to get messages");
                return Ok(messages.data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        /// <summary>
        /// return the login user rooms Id with unread message count
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetUnreadCount")]
        public async Task<IActionResult> GetUnreadCount()
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var count = await chatRepository.GetUnreadCount();
                return Ok(count);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [HttpGet("Rooms/{PageCount}/{SkipCount}")]
        public async Task<IActionResult> GetRoomsAsync(int PageCount, int SkipCount)
        {
            try
            {
                var data = await chatRepository.GetRoomsAsync(PageCount, SkipCount);
                return Ok(data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpGet("Rooms/{ChatRoomId}/Chat")]
        public async Task<IActionResult> GetRoomsChatAsync(int ChatRoomId)
        {
            try
            {
                var data = await chatRepository.GetRoomChatAsync(ChatRoomId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}