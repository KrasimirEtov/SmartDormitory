using Microsoft.EntityFrameworkCore;
using SmartDormitory.App.Data;
using SmartDormitory.Data.Models;
using SmartDormitory.Services.Abstract;
using SmartDormitory.Services.Contracts;
using SmartDormitory.Services.Exceptions;
using SmartDormitory.Services.Models.Notifications;
using SmartDormitory.Services.Utils;
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

        public async Task<IEnumerable<InboxServiceModel>> GetAllByUserId(string userId, int seen = 0, int page = 1, int pageSize = 10)
        {
            Validator.ValidateNull(userId);
            Validator.ValidateGuid(userId);

            var notifications = this.Context
                                    .Notifications
                                    .Where(n => !n.IsDeleted && n.ReceiverId == userId);

            if (seen != -1 && (seen == 0 || seen == 1))
            {
                bool isSeen = seen != 0;
                notifications = notifications.Where(n => n.Seen == isSeen);
            }

            return await notifications
                          .OrderByDescending(n => n.CreatedOn)
                          .Skip((page - 1) * pageSize)
                          .Take(pageSize)
                          .Select(n => new InboxServiceModel
                          {
                              Id = n.Id,
                              AlarmValue = n.AlarmValue,
                              Title = n.Title,
                              Message = n.Message,
                              CreatedOn = (DateTime)n.CreatedOn,
                              Seen = n.Seen,
                              SensorName = n.Sensor.Name,
                              SensorId = n.SensorId,
                              MeasureUnit = n.Sensor.IcbSensor.MeasureType.MeasureUnit
                          })
                          .ToListAsync();
        }

        public async Task<int> GetUnseenCount(string userId)
        {
            Validator.ValidateNull(userId);
            Validator.ValidateGuid(userId);

            return await this.Context
                        .Notifications
                        .CountAsync(n => !n.IsDeleted && !n.Seen && n.ReceiverId == userId);
        }

        public async Task<int> TotalCountByCriteria(string userId, int seen = 0, int page = 1, int pageSize = 10)
        {
            Validator.ValidateNull(userId);
            Validator.ValidateGuid(userId);

            var notifications = this.Context
                             .Notifications
                             .Where(n => !n.IsDeleted && n.ReceiverId == userId);

            if (seen != -1 && (seen == 0 || seen == 1))
            {
                bool isSeen = seen != 0;
                notifications = notifications.Where(n => n.Seen == isSeen);
            }

            return await notifications.CountAsync();
        }

        public async Task Delete(string id)
        {
            Validator.ValidateNull(id);
            Validator.ValidateGuid(id);

            var notification = await this.Context.Notifications.FirstOrDefaultAsync(n => n.Id == id);

            if (notification == null)
            {
                throw new EntityDoesntExistException("Notification doesnt exists!");
            }

            this.Context.Notifications.Remove(notification);
            await this.Context.SaveChangesAsync();
        }

        public async Task ReadAll(string userId)
        {
            Validator.ValidateNull(userId);
            Validator.ValidateGuid(userId);

            var user = await this.Context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new EntityDoesntExistException("User doesnt exists!");
            }

            await this.Context
                      .Notifications
                      .Where(n => n.ReceiverId == user.Id)
                      .ForEachAsync(n => n.Seen = true);

            await this.Context.SaveChangesAsync();
        }
    }
}
