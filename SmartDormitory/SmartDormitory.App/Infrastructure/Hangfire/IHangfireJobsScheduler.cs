using System.Threading.Tasks;

namespace SmartDormitory.App.Infrastructure.Hangfire
{
    public interface IHangfireJobsScheduler
    {
        void ActivateRecurringJobs();
        //Task UpdateSensorsData();
        Task HardTenSecondsRecurringJob();

        Task ReviseIcbSensors();
    }
}