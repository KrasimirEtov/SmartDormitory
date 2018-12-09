using SmartDormitory.App.Data;
using SmartDormitory.Data.Models;
using SmartDormitory.Services.Abstract;
using SmartDormitory.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartDormitory.Services
{
    public class NotificationService : BaseService, INotificationService
    {
        public NotificationService(SmartDormitoryContext context) : base(context)
        {
        }

        public async Task BuildNotifications(IEnumerable<Sensor> sensors)
        {
            var notifications = new List<Notification>();
            foreach (var sensor in sensors)
            {
                notifications.Add(new Notification
                {
                    ReceiverId = sensor.UserId,
                    Message = $"Alarm! Sensor {sensor.Name} out of range!", //TODO: better msg
                    CreatedOn = DateTime.Now
                });
            }

            await this.Context.AddRangeAsync(notifications);
            await this.Context.SaveChangesAsync();
            //send list notifications to signalR hub to send messages
        }
    }
}
