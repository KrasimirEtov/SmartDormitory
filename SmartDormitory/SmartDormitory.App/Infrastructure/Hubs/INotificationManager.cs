using System.Threading.Tasks;

namespace SmartDormitory.App.Infrastructure.Hubs
{
    public interface INotificationManager
    {
        Task SendNotification(string userId, string message);

        Task SendRegularUsersAlert(string message);

        Task SendAdminsAlert(string message);
    }
}
