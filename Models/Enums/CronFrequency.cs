using System.ComponentModel;

namespace Models.Enums
{
    public enum CronFrequency
    {
        Unknown = 0,
        [Description("Every Minute")]
        EveryMinute=1,
        [Description("Every 30 Minute")]
        HalfHourly =2,
        Hourly =3,
        Daily = 4,
        Weekly =5,
        [Description("Bi-Weekly")]
        BiWeekly =6,
        [Description("Tri-Weekly")]
        Monthly =7,
        YearLy =8,
    }
}
