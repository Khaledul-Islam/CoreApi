using Models.Enums;

namespace Utilities.Extensions
{
    public static class CronExpressionExtensions
    {
        public static DateTimeOffset? ParseCronToNextOccurrence(this string cronExpression)
        {
            try
            {
                var cron = new Quartz.CronExpression(cronExpression);
                return cron.GetNextValidTimeAfter(DateTime.Now);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }

        public static readonly Dictionary<CronFrequency, string> CronMappings = new()
        {
            { CronFrequency.Unknown, "Unsupported cron expression" },
            { CronFrequency.EveryMinute, "0 * * ? * *" },
            { CronFrequency.HalfHourly, "0 */30 * ? * *" },
            { CronFrequency.Hourly, "0 0 * ? * *" },
            { CronFrequency.Daily, "0 0 0 * * ?" },
            { CronFrequency.Weekly, "0 0 12 ? * SUN" },
            { CronFrequency.BiWeekly, "0 0 0 14,28 * ? *" },
            { CronFrequency.Monthly, "0 0 0 1 * ? *" },
            { CronFrequency.YearLy, "0 0 0 1 JAN ? *" },

           
            //{ ScheduleFrequency.Monthly, "0 0 0 1 1/1 ? *" },
        };

        public static string ToCronExpression(this CronFrequency frequency)
        {
            if (CronMappings.TryGetValue(frequency, out var cronExpression))
            {
                return cronExpression;
            }
            throw new ArgumentException("Unsupported schedule frequency");
        }

        public static CronFrequency ParseCronToFrequency(this string cronExpression)
        {
            var (key, value) = CronMappings.FirstOrDefault(kvp => kvp.Value == cronExpression);
            return key;
        }

    }
}
