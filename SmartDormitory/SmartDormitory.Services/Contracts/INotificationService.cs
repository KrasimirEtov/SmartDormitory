using SmartDormitory.Data.Models;
using SmartDormitory.Services.Models.Notifications;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartDormitory.Services.Contracts
{
    public interface INotificationService
    {
        Task<IEnumerable<Notification>> CreateAlarmNotifications(IEnumerable<Sensor> sensors);

        Task<IEnumerable<InboxServiceModel>> GetLastUnseenByUserId(string userId, int count = 5);

        Task<int> GetUnseenCount(string userId);

        Task<IEnumerable<InboxServiceModel>> GetAllByUserId(string userId, int seen = 0, int page = 1, int pageSize = 10);

        Task<int> TotalCountByCriteria(string userId, int seen = 0, int page = 1, int pageSize = 10);

        Task Delete(string id);

        Task ReadAll(string userId);
    }
}
