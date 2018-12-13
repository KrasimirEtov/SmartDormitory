using Microsoft.AspNetCore.SignalR;
using SmartDormitory.App.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartDormitory.App.Infrastructure.Hubs
{
    public class NotificationsHub : Hub
    {
        public NotificationsHub()
        {
            this.ConnectedUsers = new Dictionary<string, string>();
        }

        public Dictionary<string, string> ConnectedUsers { get; set; }

        public override async Task OnConnectedAsync()
        {
            var currentUser = this.Context.User;
            var connectionId = this.Context.ConnectionId;

            if (!this.ConnectedUsers.ContainsKey(currentUser.Identity.Name))
            {
                this.ConnectedUsers.Add(currentUser.Identity.Name, connectionId);
            }

            this.ConnectedUsers[currentUser.Identity.Name] = connectionId;

            if (currentUser.IsInRole(WebConstants.AdminRole))
            {
                await this.Groups.AddToGroupAsync(this.Context.ConnectionId, WebConstants.AdminsGroup);
            }
            else
            {
                await this.Groups.AddToGroupAsync(this.Context.ConnectionId, WebConstants.RegularsGroup);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var currentUser = this.Context.User;

            if (this.ConnectedUsers.ContainsKey(currentUser.Identity.Name))
            {
                this.ConnectedUsers.Remove(currentUser.Identity.Name, out string garbage);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
