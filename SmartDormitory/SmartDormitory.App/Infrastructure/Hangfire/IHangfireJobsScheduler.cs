using System.Threading.Tasks;

namespace SmartDormitory.App.Infrastructure.Hangfire
{
    public interface IHangfireJobsScheduler
    {
        Task HardTenSecondsRecurringJob();

        Task ReviseIcbSensors();
    }
}