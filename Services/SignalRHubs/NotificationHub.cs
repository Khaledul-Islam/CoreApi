using Contracts.SignalRHubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Models.SignalRHubs;

namespace Services.SignalRHubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class NotificationHub : Hub<INotificationHub>
    {
        public Task Send(NotificationItem item)
        {
            var groupName = $"{nameof(NotificationItem.UserId)}_{item.UserId}";
            return Clients.Group(groupName).Send(item);
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).JoinGroup(groupName);
        }

        public async Task LeaveGroup(string groupName)
        {
            await Clients.Group(groupName).LeaveGroup(groupName);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
    }
}
