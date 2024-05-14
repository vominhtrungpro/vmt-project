using Microsoft.AspNetCore.SignalR;
using vmt_project.models.Request.Signalr;

namespace vmt_project.Hubs
{
    public class MessageHub : Hub
    {
        public async Task SendMessage(HubMessage message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
