using SmartDormitory.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartDormitory.Services.Contracts
{
    public interface INotificationService
    {
        Task CreateAlarmNotifications(IEnumerable<Sensor> sensors);
    }
}
