using Microsoft.Extensions.Logging;
using Quartz;

namespace Services.Quartz.Job
{
    public class YourSampleJob : IJob
    {
        private readonly ILogger<YourSampleJob> _logger;

        public YourSampleJob(ILogger<YourSampleJob> logger)
        {
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Executing sample job...");

            // Your job logic here

            return Task.CompletedTask;
        }
    }
}
