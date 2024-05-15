using Microsoft.AspNetCore.SignalR;
using vmt_project.models.Request.Signalr;

namespace vmt_project.Hubs
{
    public class MessageHub : Hub
    {
        //public async Task SendMessage(HubMessage message)
        //{
        //    try
        //    {
        //        await Clients.All.SendAsync("ReceiveMessage", message);
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}
        public Task SendMessage(HubMessage message) =>
                Clients.All.SendAsync("ReceiveMessage", message);

    }
}
