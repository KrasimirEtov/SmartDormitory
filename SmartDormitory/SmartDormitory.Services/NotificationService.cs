using Microsoft.EntityFrameworkCore;
using SmartDormitory.App.Data;
using SmartDormitory.Data.Models;
using SmartDormitory.Services.Abstract;
using SmartDormitory.Services.Contracts;
using SmartDormitory.Services.Models.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDormitory.Services
{
    public class NotificationService : BaseService, INotificationService
    {
        public NotificationService(SmartDormitoryContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Notification>> CreateAlarmNotifications(IEnumerable<Sensor> sensors)
        {
            var notifications = new List<Notification>();

            foreach (var sensor in sensors)
            {
                notifications.Add(new Notification
                {
                    ReceiverId = sensor.UserId,
                    Title = $"Alarm! Sensor: <{sensor.Name}> out of range!",
                    Message = $"<{sensor.Name}> just got {sensor.CurrentValue} value thats out of range [{sensor.MinRangeValue}-{sensor.MaxRangeValue}]!", //TODO: better msg
                    CreatedOn = DateTime.Now,
                    SensorId = sensor.Id,
                    AlarmValue = sensor.CurrentValue
                    //add notification type Alarm, Info etc
                });
            }

            await this.Context.AddRangeAsync(notifications);

            return notifications;
            //await this.Context.SaveChangesAsync(); calling it at hangfire job method
            //send list notifications to signalR hub to send messages
        }

        public async Task<IEnumerable<Notification>> GetAllByUserId(string userId)
            => await this.Context
                         .Notifications
                         .Where(n => !n.IsDeleted && n.ReceiverId == userId)
                         .OrderByDescending(n => n.CreatedOn)
                         .ToListAsync();

        public async Task<IEnumerable<InboxServiceModel>> GetLastUnseenByUserId(string userId, int count = 5)
            => await this.Context
                         .Notifications
                         .Where(n => !n.IsDeleted && !n.Seen && n.ReceiverId == userId)
                         .OrderByDescending(n => n.CreatedOn)
                         .Take(count)
                         .Select(n => new InboxServiceModel
                         {
                             AlarmValue = n.AlarmValue,
                             Title = n.Title,
                             CreatedOn = (DateTime)n.CreatedOn,
                         })
                         .ToListAsync();

        public async Task<int> GetUnseenCount(string userId)
          => await this.Context
                       .Notifications
                       .CountAsync(n => !n.IsDeleted && !n.Seen && n.ReceiverId == userId);
    }
}
