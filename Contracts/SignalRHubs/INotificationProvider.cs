﻿using Models.SignalRHubs;

namespace Contracts.SignalRHubs
{
    public interface INotificationProvider
    {
        Task Send(NotificationItem item);

        Task JoinGroup(string groupName);

        Task LeaveGroup(string groupName);
    }
}
