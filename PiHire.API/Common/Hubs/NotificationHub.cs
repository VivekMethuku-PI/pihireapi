using Microsoft.AspNetCore.SignalR;
using PiHire.BAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PiHire.API.Common.Hubs
{

    /// <summary>
    /// This hub is responsible for chat, notification and indicator(current status)
    /// This hub is use for only pushing the data and triggering the event on client side
    /// </summary>
    public class NotificationHub : Hub<INotificationHub>
    {
        public async Task BroadcastAsync(NotificationMessage message)
        {
            await Clients.All.MessageReceivedFromHub(message);
        }

       
        public override async Task OnConnectedAsync()
        {
            await Clients.All.NewUserConnected("a new user connectd");
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            //await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnDisconnectedAsync(exception);
        }
    }

    public interface INotificationHub
    {
        Task MessageReceivedFromHub(NotificationMessage message);

        Task NewUserConnected(string message);

        
    }

    public class NotificationMessage
    {
        public string Text { get; set; }
        public string ConnectionId { get; set; }
        public DateTime DateTime { get; set; }
    }
}
