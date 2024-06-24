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
using PiHire.API.Security;
using PiHire.BAL.Common.Extensions;
using PiHire.BAL.IRepositories;
using PiHire.BAL.ViewModels;
using PiHire.o_SD.Controllers;
using PiHire.Utilities.ViewModels.Communications.Emails;

namespace PiHire.API.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MailSupportController : BaseController
    {
        readonly ILogger<MailSupportController> logger;
        private readonly AppSettings _appSettings;
        private readonly IMailSupportRepository mailSupport;
        JWTAutorization _autorization;
        public MailSupportController(ILogger<MailSupportController> logger,
            AppSettings appSettings, IHubContext<NotificationHub> hubContext,
            IMailSupportRepository mailSupport, ILoggedUserDetails loginUserDetails, JWTAutorization autorization, INotificationRepository notificationRepository) : base(mailSupport, loginUserDetails, hubContext, notificationRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appSettings = appSettings;
            this.mailSupport = mailSupport;
            _autorization = autorization;
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
        [HttpPost("InfoBip/Webhook")]
        public async Task<IActionResult> InfoBip_webhook(Utilities.Communications.Emails.InfoBip.InfoBipNotifyReport model)
        {
            var _tmp = Request.Headers.ToList();
            logger.LogDebug("InfoBip/Webhook->Header:" + JsonConvert.SerializeObject(_tmp));

            //var reader = new System.IO.StreamReader(Request.Body);
            //reader.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
            //var rawMessage = reader.ReadToEnd();
            //logger.LogDebug("InfoBip/Webhook->rawMessage:" + rawMessage);
            logger.LogDebug("InfoBip/Webhook->model:" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
            await this.mailSupport.InfoBip_Webhook(model);
            try
            {
                using (var reader = new System.IO.StreamReader(Request.Body, System.Text.Encoding.UTF8))
                {
                    string value = reader.ReadToEnd();
                    logger.LogDebug("InfoBip/Webhook->value:" + value);
                }

            }
            catch (Exception e)
            {
                logger.LogError(e, "InfoBip/Webhook->value error:");
            }
            try
            {
                var chk = true;
                while (chk)
                {
                    var pipeReadResult = await Request.BodyReader.ReadAsync();
                    var buffer = pipeReadResult.Buffer;

                    try
                    {
                        logger.LogDebug("InfoBip/Webhook->value2:" + buffer.Length.ToString());

                        if (pipeReadResult.IsCompleted)
                        {
                            chk = false;
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, "InfoBip/Webhook->value2 error:");
                    }
                    finally
                    {
                        Request.BodyReader.AdvanceTo(buffer.End);
                    }
                }

            }
            catch (Exception e)
            {
                logger.LogError(e, "InfoBip/Webhook->value2 error:");
            }
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("SendGrid/Webhook")]
        public async Task<IActionResult> SendGrid_webhook(List<SendGrid_WebhookViewModel> model)
        {
            var _tmp = Request.Headers.ToList();
            logger.LogDebug("SendGrid/Webhook->model:" + JsonConvert.SerializeObject(model) + ",Header:" + JsonConvert.SerializeObject(_tmp));
            await this.mailSupport.SendGrid_Webhook(model);
            return Ok();
        }
        [AllowAnonymous]
        [HttpPost("MailChimp/Webhook")]
        public async Task<IActionResult> MailChimp_webhook(List<MailChimp_WebhookViewModel> model)
        {
            var _tmp = Request.Headers.ToList();
            logger.LogDebug("MailChimp/Webhook->model:" + JsonConvert.SerializeObject(model) + ",Header:" + JsonConvert.SerializeObject(_tmp));
            await this.mailSupport.MailChimp_Webhook(model);
            return Ok();
        }
    }
}