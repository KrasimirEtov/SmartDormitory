using Hangfire;
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
using System.Net.Http;
using System.Threading.Tasks;

namespace SmartDormitory.App.Infrastructure.Hangfire
{
    public class HangfireJobsScheduler : IHangfireJobsScheduler
    {
        private readonly IServiceProvider serviceProvider;
        private readonly INotificationManager notificationManager;
        private readonly IIcbSensorsService icbSensorsService;
        private readonly IIcbApiService apiService;

        public HangfireJobsScheduler(IServiceProvider serviceProvider, INotificationManager notificationManager, IIcbSensorsService icbSensorsService, IIcbApiService apiService)
        {
            this.serviceProvider = serviceProvider;
            this.notificationManager = notificationManager;
            this.icbSensorsService = icbSensorsService;
            this.apiService = apiService;
        }

        // seed icb sensors and check for new ones
        public async Task ReviseIcbSensors()
        {
            try
            {
                var upToDateApiSensors = await this.apiService.GetAllIcbSensors();
                var dbExistingSensors = await this.icbSensorsService.GetAll();

                var sensorsToAdd = upToDateApiSensors
                                        .Where(s => !dbExistingSensors
                                                .Any(dbSensor => dbSensor.Id != s.ApiSensorId))
                                        .ToList();

                await this.icbSensorsService.AddSensors(sensorsToAdd);
            }
            catch (HttpRequestException e)
            {
                await this.notificationManager.SendAdminsAlert(e.Message);
            }
        }

        public async Task HardTenSecondsRecurringJob()
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

                var liveDataCache = new Dictionary<string, ApiSensorValueDTO>();
                var sensorsToUpdate = new List<Sensor>();
                var alarmsActivatedSensors = new List<Sensor>();

                var userSensorsForUpdate = await sensorsService.GetAllForUpdate();

                foreach (var userSensor in userSensorsForUpdate)
                {
                    try
                    {
                        // caching all 13 api sensors data for current BackgroundJob iteration
                        if (!liveDataCache.ContainsKey(userSensor.IcbSensorId))
                        {
                            var newApiData = await icbApi.GetIcbSensorDataById(userSensor.IcbSensorId);
                            liveDataCache[userSensor.IcbSensorId] = newApiData;
                        }

                        var liveSensorData = liveDataCache[userSensor.IcbSensorId];
                        float newValue = ApiDataHelper.GetLastValue(liveSensorData.LastValue);

                        //if live data value is same like last time, skip
                        if (newValue != userSensor.CurrentValue)
                        {
                            userSensor.CurrentValue = newValue;
                        }
                        userSensor.LastUpdateOn = liveSensorData.TimeStamp;

                        // populate list of sensors which data should be updated
                        sensorsToUpdate.Add(userSensor);

                        if (userSensor.AlarmOn &&
                                (newValue <= userSensor.MinRangeValue || newValue >= userSensor.MaxRangeValue))
                        {
                            // populate list of sensors with activated alarms
                            alarmsActivatedSensors.Add(userSensor);
                        }
                    }
                    catch (HttpRequestException e)
                    {
                        await this.notificationManager.SendUIErrorAlerts(e.Message);
                    }
                }

                //stage notifications
                var notifications = await notificationService.CreateAlarmNotifications(alarmsActivatedSensors);

                foreach (var notify in notifications)
                {
                    await this.notificationManager.SendNotification(notify.ReceiverId, notify.Title);
                }

                //--- Save everything(sensors + notifies) with one transaction
                dbContext.UpdateRange(sensorsToUpdate);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
