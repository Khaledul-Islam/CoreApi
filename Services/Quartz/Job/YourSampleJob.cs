using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Services.Quartz.Job
{
    public class YourSampleJob(ILogger<YourSampleJob> logger) : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            logger.LogError("Executing sample job...");
            Debug.WriteLine("Executing sample job...");
            // Your job logic here

            return Task.CompletedTask;
        }
    }
}
