using System.Threading.Tasks;

namespace SmartDormitory.App.Infrastructure.Hangfire
{
    public interface IHangfireJobsScheduler
    {
        void StartingJobsQueue();
        Task UpdateSensorsData();
        Task Magic();
    }
}