namespace SmartDormitory.App.Infrastructure.Hangfire
{
    public interface IHangfireJobsScheduler
    {
        void StartingJobsQueue();
    }
}