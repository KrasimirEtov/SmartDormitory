using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartDormitory.App.Data;
using SmartDormitory.App.Infrastructure.Hubs;
using SmartDormitory.Data.Models;
using SmartDormitory.Services.Contracts;
using SmartDormitory.Services.Models.JsonDtoModels;
using SmartDormitory.Services.Utils.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDormitory.App.Infrastructure.Hangfire
{
    public class HangfireJobsScheduler : IHangfireJobsScheduler
    {
        private readonly IServiceProvider serviceProvider;
        private readonly INotificationManager notificationManager;

        public HangfireJobsScheduler(IServiceProvider serviceProvider, INotificationManager notificationManager)
        {
            this.serviceProvider = serviceProvider;
            this.notificationManager = notificationManager;
        }

        public void StartingJobsQueue()
        {
            //RecurringJob.AddOrUpdate(() => this.UpdateIcbSensors(), Cron.Hourly());

            //delete all in queue
            var mon = JobStorage.Current.GetMonitoringApi();
            var scheduled = mon.ScheduledJobs(int.MinValue, int.MaxValue);
            var scheduledCount = mon.ScheduledCount();
            var queues = mon.Queues();
            var processingCount = mon.ProcessingCount();
            var processingJobs = mon.ProcessingJobs(int.MinValue, int.MaxValue);
            var FetchedCount = mon.FetchedCount("sensordata");
            var EnqueuedCount = mon.EnqueuedCount("sensordata");
            var jobs = mon.EnqueuedJobs("sensordata", 0, 99999999);
            jobs.ForEach(x =>
            {
                BackgroundJob.Delete(x.Key);
            });
            var afterEnqueuedCount = mon.EnqueuedCount("sensordata");

            scheduled.ForEach(x =>
            {
                BackgroundJob.Delete(x.Key);
            });
            //save jobId in db and then check?

            //add new to queue
            var jobId = BackgroundJob.Enqueue(() => UpdateSensorsData());
        }

        // seed icb sensors and check for new ones
        public async Task UpdateIcbSensors()
        {
            using (var scope = this.serviceProvider.CreateScope())
            {
                var icbSensorsService = scope.ServiceProvider.GetService<IIcbSensorsService>();
                //todo refactor
                //add inexisting icb sample sensor to db
                //var addedSensorsData = await icbSensorsService.AddSensorsAsync();

                await icbSensorsService.AddSensorsAsync();
            }
        }

        public async Task Magic()
        {
            // hard 10 seconds interval
            for (int i = 0; i < 5; i++)
            {
                await this.UpdateSensorsData();
                await Task.Delay(TimeSpan.FromSeconds(10));
            }

            await this.UpdateSensorsData();
        }

        [DisableConcurrentExecution(timeoutInSeconds: 10 * 60)]
        public async Task UpdateSensorsData()
        {
            using (var scope = this.serviceProvider.CreateScope())
            {
                var icbApi = scope.ServiceProvider.GetService<IIcbApiService>();
                var sensorsService = scope.ServiceProvider.GetService<ISensorsService>();
                var notificationService = scope.ServiceProvider.GetService<INotificationService>();
                var dbContext = scope.ServiceProvider.GetService<SmartDormitoryContext>();

                var userSensorsForUpdate = await dbContext
                        .Sensors
                        .Where(s => DateTime.Now.Subtract(s.LastUpdateOn).TotalSeconds >= s.PollingInterval)
                        .ToListAsync();

                // icbSensorId   
                var liveDataCache = new Dictionary<string, ApiSensorValueDTO>();
                var sensorsToUpdate = new List<Sensor>();
                var alarmsActivatedSensors = new List<Sensor>();

                foreach (var userSensor in userSensorsForUpdate)
                {
                    // caching all 13? api sensors data for current BackgroundJob
                    if (!liveDataCache.ContainsKey(userSensor.IcbSensorId))
                    {
                        var newApiData = await icbApi
                                                  .GetIcbSensorValueById(userSensor.IcbSensorId);

                        liveDataCache[userSensor.IcbSensorId] = newApiData;
                    }

                    var liveSensorData = liveDataCache[userSensor.IcbSensorId];

                    float newValue = ApiDataHelper.GetLastValue(liveSensorData.LastValue);

                    userSensor.CurrentValue = newValue;
                    userSensor.LastUpdateOn = liveSensorData.TimeStamp;

                    // populate list of sensors which data should be updated
                    sensorsToUpdate.Add(userSensor);

                    if (userSensor.AlarmOn &&
                            (newValue <= userSensor.MinRangeValue ||
                             newValue >= userSensor.MaxRangeValue))
                    {
                        // populate list of sensors with activated alarms
                        alarmsActivatedSensors.Add(userSensor);
                    }
                }

                //create and send alarm notifications
                var notifications = await notificationService.CreateAlarmNotifications(alarmsActivatedSensors);

                foreach (var notify in notifications)
                {
                    await this.notificationManager.SendNotification(notify.ReceiverId, notify.Title);
                }

                //update all user sensors data at once
                //await sensorsService.UpdateRange(sensorsToUpdate);

                //--- save everything at once
                dbContext.UpdateRange(sensorsToUpdate);
                await dbContext.SaveChangesAsync();               
            }
        }
    }
}
