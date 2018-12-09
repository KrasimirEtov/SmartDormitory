using SmartDormitory.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartDormitory.Services.Contracts
{
    public interface INotificationService
    {
        Task BuildNotifications(IEnumerable<Sensor> sensors);
    }
}
