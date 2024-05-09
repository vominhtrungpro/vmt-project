using Azure.Identity;
using Confluent.Kafka;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using vmt_project.models.Request.Signalr;

namespace vmt_project.Hubs
{
    public interface IMessageHub
    {
        Task SendMessage(string message);
    }
}
