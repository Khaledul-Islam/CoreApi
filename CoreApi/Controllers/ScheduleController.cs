using Contracts.Quartz;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Scheduler;
using Quartz;
using Services.Quartz.Job;

namespace CoreApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ScheduleController(IJobScheduleService jobScheduleService,
            ILogger<ScheduleController> logger, IScheduler scheduler) : BaseController
    {
        [HttpGet("RunSampleJob")]
        public async Task<IActionResult> RunSampleJob()
        {
            var jobAndScheduleInfo = new JobScheduleData
            {
                JobName = "SampleJob",
                JobGroup = "SampleGroup",
                CronExpression = "0/5 * * * * ?", // Every 5 seconds
                Scheduler = scheduler
            };

            try
            {
                await jobScheduleService.ScheduleCronJob<YourSampleJob, ScheduleController>(jobAndScheduleInfo,
                    logger);
                return Ok("Sample job scheduled successfully!");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error scheduling sample job: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }


    }
}