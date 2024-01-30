using Quartz;

namespace Models.Scheduler
{
    public class JobScheduleData
    {
        public IScheduler Scheduler;
        public string JobName = string.Empty;
        public string JobGroup = string.Empty;
        public string CronExpression = string.Empty;
        public JobDataMap? JobData;
        public bool DiscardMisfires = false;
        public bool RescheduleIfExists = true;
        public bool IsActive = false;
    }
}
