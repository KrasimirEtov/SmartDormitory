using Microsoft.AspNetCore.SignalR;
using SmartDormitory.App.Infrastructure.Common;
using System.Threading.Tasks;

namespace SmartDormitory.App.Infrastructure.Hubs
{
    public class NotificationManager : INotificationManager
    {
        private readonly IHubContext<NotificationsHub> hub;

        public NotificationManager(IHubContext<NotificationsHub> hub)
        {
            this.hub = hub;
        }

        public async Task SendNotification(string userId, string message)
        {
            await this.hub.Clients.User(userId).SendAsync("ReceiveNotification", message);
        }

        public async Task SendRegularUsersAlert(string message)
        {
            await this.hub.Clients.Groups(WebConstants.RegularsGroup).SendAsync("ReceiveRegularUserAlert", message);
        }

        public async Task SendAdminsAlert(string message)
        {
            await this.hub.Clients.Groups(WebConstants.AdminsGroup).SendAsync("ReceiveAdminAlert", message);
        }
    }
}
