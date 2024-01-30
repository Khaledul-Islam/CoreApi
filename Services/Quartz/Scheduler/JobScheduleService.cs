using Microsoft.Extensions.Logging;
using Models.Scheduler;
using Quartz.Impl.Matchers;
using Quartz;

namespace Services.Quartz.Scheduler
{
    public class JobScheduleService
    {
        public async Task ScheduleCronJob<T>(JobScheduleData jobAndScheduleInfo, ILogger logger)
            where T : IJob
        {
            var jobBuilder = JobBuilder.Create<T>()
                .WithIdentity(jobAndScheduleInfo.JobName, jobAndScheduleInfo.JobGroup)
                .DisallowConcurrentExecution();
            if (jobAndScheduleInfo.JobData != null)
            {
                jobBuilder = jobBuilder.UsingJobData(jobAndScheduleInfo.JobData);
            }

            var job = jobBuilder.Build();

            var triggerBuilder = TriggerBuilder.Create()
                .WithIdentity($"{jobAndScheduleInfo.JobName}-T", jobAndScheduleInfo.JobGroup);


            var cronBuilder = CronScheduleBuilder.CronSchedule(jobAndScheduleInfo.CronExpression)
                .InTimeZone(TimeZoneInfo.Local);

            if (jobAndScheduleInfo.DiscardMisfires)
            {
                cronBuilder = cronBuilder.WithMisfireHandlingInstructionDoNothing();
            }
            else
            {
                cronBuilder = cronBuilder.WithMisfireHandlingInstructionFireAndProceed();
            }

            triggerBuilder = triggerBuilder.WithSchedule(cronBuilder).StartNow();


            var trigger = triggerBuilder.Build();

            try
            {
                logger.LogDebug("Scheduling {0}", jobAndScheduleInfo.JobName);
                await jobAndScheduleInfo.Scheduler.ScheduleJob(job, trigger);
                logger.LogDebug("Scheduled {0}", jobAndScheduleInfo.JobName);
            }
            catch
            {
                logger.LogDebug("Job {0} already exists!", jobAndScheduleInfo.JobName);

                if (jobAndScheduleInfo.RescheduleIfExists)
                {
                    logger.LogDebug("Rescheduling {0}", jobAndScheduleInfo.JobName);
                    await jobAndScheduleInfo.Scheduler.RescheduleJob(trigger.Key, trigger);
                    logger.LogDebug("Rescheduled {0}", jobAndScheduleInfo.JobName);
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task RescheduleJob(JobScheduleData jobAndScheduleInfo, ILogger logger)
        {
            var existingTriggerKey = new TriggerKey($"{jobAndScheduleInfo.JobName}-T", jobAndScheduleInfo.JobGroup);
            var existingTrigger = await jobAndScheduleInfo.Scheduler.GetTrigger(existingTriggerKey);

            if (existingTrigger != null)
            {
                logger.LogDebug("Rescheduling {0}", jobAndScheduleInfo.JobName);
                var newTrigger = TriggerBuilder.Create()
                    .WithIdentity(jobAndScheduleInfo.JobName, jobAndScheduleInfo.JobGroup)
                    .WithCronSchedule(jobAndScheduleInfo.CronExpression)
                    .Build();

                await jobAndScheduleInfo.Scheduler.RescheduleJob(existingTriggerKey, newTrigger);
                logger.LogDebug("Rescheduled {0}", jobAndScheduleInfo.JobName);
            }
        }

        public async Task ExecuteJobNow<T>(JobScheduleData jobAndScheduleInfo, ILogger logger)
            where T : IJob
        {
            logger.LogDebug("Execute Job Now {0}", jobAndScheduleInfo.JobName);
            var jobKey = new JobKey(jobAndScheduleInfo.JobName, jobAndScheduleInfo.JobGroup);
            var triggerKey = new TriggerKey($"{jobAndScheduleInfo.JobName}-T", jobAndScheduleInfo.JobGroup);

            if (await jobAndScheduleInfo.Scheduler.CheckExists(jobKey))
            {
                logger.LogDebug("Already Job Exists {0}", jobAndScheduleInfo.JobName);
                var triggerState = await jobAndScheduleInfo.Scheduler.GetTriggerState(triggerKey);
                switch (triggerState)
                {
                    case TriggerState.Paused:
                        await ResumeTrigger(jobAndScheduleInfo, logger);
                        break;
                }

                await jobAndScheduleInfo.Scheduler.TriggerJob(jobKey);
                logger.LogDebug("Executed Job Now {0}", jobAndScheduleInfo.JobName);
            }
            else
            {
                logger.LogDebug("Job Not Exists {0}", jobAndScheduleInfo.JobName);
                if (jobAndScheduleInfo.IsActive)
                    await ScheduleCronJob<T>(jobAndScheduleInfo, logger);
                else
                {
                    var job = JobBuilder.Create<T>()
                        .WithIdentity(jobAndScheduleInfo.JobName, jobAndScheduleInfo.JobGroup)
                        .Build();

                    var trigger = TriggerBuilder.Create()
                        .StartNow()
                        .Build();
                    await jobAndScheduleInfo.Scheduler.ScheduleJob(job, trigger);
                    logger.LogDebug("Scheduled Job {0}", jobAndScheduleInfo.JobName);
                }
            }
        }

        public async Task Remove(IScheduler scheduler, string jobName, ILogger logger, string? triggerName = null)
        {
            logger.LogDebug("UnScheduling job-trigger (as a part of remove job): {0}", triggerName);
            var allTriggerKeys = await scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup());

            triggerName ??= $"T{jobName}";
            foreach (var triggerKey in allTriggerKeys)
            {
                if (triggerKey.Name == triggerName)
                {
                    logger.LogDebug("");
                    await scheduler.UnscheduleJob(triggerKey);
                }
            }

            logger.LogDebug("Removing job: {0}", jobName);
            var allJobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());

            foreach (var jobKey in allJobKeys)
            {
                if (jobKey.Name == jobName)
                {
                    await scheduler.DeleteJob(jobKey);
                    logger.LogInformation("Removed Job: {0}", jobName);
                    return;
                }
            }

            logger.LogError("Remove job failed. Could not find job with name: {0}", jobName);
        }

        public async Task UnScheduleJob(JobScheduleData jobAndScheduleInfo, ILogger logger)
        {
            var triggerKey = new TriggerKey($"{jobAndScheduleInfo.JobName}-T", jobAndScheduleInfo.JobGroup);
            await jobAndScheduleInfo.Scheduler.UnscheduleJob(triggerKey);
        }

        public async Task PauseTrigger(JobScheduleData jobAndScheduleInfo, ILogger logger)
        {
            var triggerKey = new TriggerKey($"{jobAndScheduleInfo.JobName}-T", jobAndScheduleInfo.JobGroup);
            await jobAndScheduleInfo.Scheduler.PauseTrigger(triggerKey);
        }

        public async Task ResumeTrigger(JobScheduleData jobAndScheduleInfo, ILogger logger)
        {
            logger.LogDebug("Resume Trigger {0}", jobAndScheduleInfo.JobName);
            var triggerKey = new TriggerKey($"{jobAndScheduleInfo.JobName}-T", jobAndScheduleInfo.JobGroup);
            await jobAndScheduleInfo.Scheduler.ResumeTrigger(triggerKey);
            logger.LogDebug("Resumed Trigger {0}", jobAndScheduleInfo.JobName);
        }

        //public  async Task<JobInformation> GetJobDetail(string identity, string group)
        //{
        //    var triggerKey = new TriggerKey(identity, group);
        //    var triggerDetail = await _scheduler.GetTrigger(triggerKey);
        //    return null;
        //}
        //public  async Task<List<JobInformation>> GetAllJobs()
        //{
        //    var jobGroups = await _scheduler.GetJobGroupNames();
        //    var jobInfoList = new List<JobInformation>();
        //    foreach (var group in jobGroups)
        //    {
        //        var groupMatcher = GroupMatcher<JobKey>.GroupContains(group);
        //        var jobKeys = await _scheduler.GetJobKeys(groupMatcher);
        //        foreach (var jobKey in jobKeys)
        //        {
        //            var detail = await _scheduler.GetJobDetail(jobKey);
        //            var triggers = await _scheduler.GetTriggersOfJob(jobKey);
        //            foreach (var trigger in triggers)
        //            {
        //                var jobInfo = new JobInformation
        //                {
        //                    GroupName = group,
        //                    JobKeyName = jobKey.Name,
        //                    JobDescription = detail.Description,
        //                    TriggerKeyName = trigger.Key.Name,
        //                    TriggerKeyGroup = trigger.Key.Group,
        //                    TriggerType = trigger.GetType().Name,
        //                    TriggerState = _scheduler.GetTriggerState(trigger.Key)
        //                };
        //                var nextFireTime = trigger.GetNextFireTimeUtc();//
        //                if (nextFireTime.HasValue)
        //                    jobInfo.NextFireTime = nextFireTime.Value.LocalDateTime.ToString(CultureInfo.InvariantCulture);
        //                var previousFireTime = trigger.GetPreviousFireTimeUtc();
        //                if (previousFireTime.HasValue)
        //                    jobInfo.PreviousFireTime = previousFireTime.Value.LocalDateTime.ToString(CultureInfo.InvariantCulture);
        //                jobInfoList.Add(jobInfo);
        //            }
        //        }
        //    }

        //    return jobInfoList;
        //}
    }
}