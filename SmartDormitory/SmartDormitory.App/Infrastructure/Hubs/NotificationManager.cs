using Microsoft.AspNetCore.SignalR;
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
    }
}
