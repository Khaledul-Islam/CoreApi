using Microsoft.Extensions.Logging;
using Models.Scheduler;
using Quartz;

namespace Contracts.Quartz
{
    public interface IJobScheduleService
    {
        Task ScheduleCronJob<T, TL>(JobScheduleData jobAndScheduleInfo, ILogger<TL> logger)
            where T : IJob where TL : class;

        Task RescheduleJob<TL>(JobScheduleData jobAndScheduleInfo, ILogger<TL> logger)
            where TL : class;

        Task ExecuteJobNow<T, TL>(JobScheduleData jobAndScheduleInfo, ILogger<TL> logger)
            where T : IJob where TL : class;

        Task Remove<TL>(IScheduler scheduler, string jobName, ILogger<TL> logger, string? triggerName = null)
            where TL : class;

        Task UnScheduleJob<TL>(JobScheduleData jobAndScheduleInfo, ILogger<TL> logger)
            where TL : class;

        Task PauseTrigger<TL>(JobScheduleData jobAndScheduleInfo, ILogger<TL> logger)
            where TL : class;

        Task ResumeTrigger<TL>(JobScheduleData jobAndScheduleInfo, ILogger<TL> logger)
            where TL : class;

    }
}
