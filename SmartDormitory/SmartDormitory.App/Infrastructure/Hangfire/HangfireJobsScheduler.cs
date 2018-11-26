using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using SmartDormitory.Services.Contracts;
using System;
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
        }

        public async Task UpdateIcbSensors()
        {
            using (var scope = this.serviceProvider.CreateScope())
            {
                var icbSensorsService = scope.ServiceProvider.GetService<IIcbSensorsService>();

                var addedSensorsData = await icbSensorsService.AddSensorsAsync();

                // setup recurring job for every new IcbSensor inserted in Db
                foreach (var (Id, PollingInterval) in addedSensorsData)
                {
                    await SetupSensorUpdateReccuringJob(Id, PollingInterval);
                }
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
