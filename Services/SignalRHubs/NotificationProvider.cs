using Contracts.SignalRHubs;
using Microsoft.AspNetCore.SignalR;
using Models.SignalRHubs;

namespace Services.SignalRHubs
{
    public class NotificationProvider(IHubContext<NotificationHub, INotificationHub> hubContext) : INotificationProvider
    {
        public async Task JoinGroup(string groupName)
        {
            // TODO: Find connectionId
            // https://learn.microsoft.com/en-us/aspnet/core/signalr/hubcontext?view=aspnetcore-6.0
            // When client methods are called from outside of the Hub class, there's no caller associated
            // with the invocation. Therefore, there's no access to the ConnectionId, Caller, and Others
            // properties.
            var connectionId = string.Empty;
            await hubContext.Groups.AddToGroupAsync(connectionId, groupName);
            await hubContext.Clients.Group(groupName).JoinGroup(groupName);
        }

        public async Task LeaveGroup(string groupName)
        {
            // TODO: Find connectionId
            var connectionId = string.Empty;
            await hubContext.Clients.Group(groupName).LeaveGroup(groupName);
            await hubContext.Groups.RemoveFromGroupAsync(connectionId, groupName);
        }

        public async Task Send(NotificationItem item)
        {
            var groupName = $"{nameof(NotificationItem.UserId)}_{item.UserId}";
            await hubContext.Clients.Group(groupName).Send(item);
        }
    }

}
