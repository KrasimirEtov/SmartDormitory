using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using SmartDormitory.Data.Models;
using SmartDormitory.Services.Contracts;
using SmartDormitory.Services.Models.JsonDtoModels;
using SmartDormitory.Services.Utils.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartDormitory.App.Infrastructure.Hangfire
{
    public class HangfireJobsScheduler : IHangfireJobsScheduler
    {
        private readonly IServiceProvider serviceProvider;

        public HangfireJobsScheduler(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void StartingJobsQueue()
        {
            RecurringJob.AddOrUpdate(() => this.UpdateIcbSensors(), Cron.Hourly());

            BackgroundJob.Enqueue(() => UpdateSensorsData());
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

                // 
                await icbSensorsService.AddSensorsAsync();

                // setup recurring job for every new IcbSensor inserted in Db
                //foreach (var (Id, PollingInterval) in addedSensorsData)
                //{
                //    await SetupSensorUpdateReccuringJob(Id, PollingInterval);
                //}
            }
        }

        public async Task UpdateSensorsData()
        {
            using (var scope = this.serviceProvider.CreateScope())
            {
                var icbApiService = scope.ServiceProvider.GetService<IIcbApiService>();
                var icbSensorsService = scope.ServiceProvider.GetService<IIcbSensorsService>();
                var sensorsService = scope.ServiceProvider.GetService<ISensorsService>();
                var notificationService = scope.ServiceProvider.GetService<INotificationService>();

                // get all registered sensors by users and then update them value if needed
                var existingUserSensors = await sensorsService.GetAll();
                var newDataCache = new Dictionary<string, ApiSensorValueDTO>();
                var sensorsToUpdate = new List<Sensor>();
                var alarmsActivatedSensors = new List<Sensor>();

                foreach (var userSensor in existingUserSensors)
                {
                    // caching all 13? api sensors data for current BackgroundJob
                    if (!newDataCache.ContainsKey(userSensor.IcbSensorId))
                    {
                        var newApiData = await icbApiService
                                                  .GetIcbSensorValueById(userSensor.IcbSensorId);

                        newDataCache[userSensor.IcbSensorId] = newApiData;
                    }

                    var newData = newDataCache[userSensor.IcbSensorId];

                    var passedTime = DateTime.Now.Subtract(userSensor.LastUpdateOn);

                    if (passedTime.TotalSeconds >= userSensor.PollingInterval)
                    {
                        float newValue = ApiDataExtractorHelper
                                                     .GetLastValue(newData.LastValue);
                        userSensor.CurrentValue = newValue;
                        userSensor.LastUpdateOn = newData.TimeStamp;

                        // populate list of sensors which data should be updated
                        sensorsToUpdate.Add(userSensor);

                        if (userSensor.AlarmOn &&
                            (newValue <= userSensor.MinRangeValue ||
                                newValue >= userSensor.MaxRangeValue))
                        {
                            // populate list of sensors with activated alarms
                            alarmsActivatedSensors.Add(userSensor);
                        }
                        // SaveChanges at once
                    }
                }

                //check if should send alarm notifications
                await notificationService.BuildNotifications(alarmsActivatedSensors);

                //update all user sensors data at once
                await sensorsService.UpdateRange(sensorsToUpdate);
                //this.Context.SavechagenesAsync();  --- save everything at once


                BackgroundJob.Schedule(() => UpdateSensorsData(), TimeSpan.FromSeconds(5));
            }
        }

        //[DisableConcurrentExecution(timeoutInSeconds: 10 * 60)]
        public async Task SetupSensorUpdateReccuringJob(string id, int pollingInterval)
        {
            using (var scope = this.serviceProvider.CreateScope())
            {
                var icbApiService = scope.ServiceProvider.GetService<IIcbApiService>();
                var icbSensorsService = scope.ServiceProvider.GetService<IIcbSensorsService>();

                var data = await icbApiService.GetIcbSensorValueById(id);

                await icbSensorsService
                        .UpdateSensorValueAsync(id, data.TimeStamp, data.LastValue, data.MeasurementUnit);

                BackgroundJob.Schedule(() => SetupSensorUpdateReccuringJob(id, pollingInterval),
                                             TimeSpan.FromSeconds(pollingInterval));
            }
        }
    }
}
