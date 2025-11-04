using Microsoft.Extensions.Logging;
using NCrontab;
using SmartJobScheduler.Models;
using SmartJobScheduler.Services.Interfaces;

namespace SmartJobScheduler.Services;
public class JobSchedulerService : IJobSchedulerService
{
    private readonly ILogger<JobSchedulerService> _logger;

    public JobSchedulerService(ILogger<JobSchedulerService> logger)
    {
        _logger = logger;
    }

    public bool ShouldRunNow(JobOption job)
    {
        if (!job.IsEnabled) return false;

        return job.ScheduleType switch
        {
            ScheduleType.Simple => IsSimpleScheduleAllowed(job),
            ScheduleType.Advanced => IsAdvancedScheduleAllowed(job),
            ScheduleType.Cron => IsCronScheduleAllowed(job),
            _ => false
        };
    }

    public DateTime? GetNextRunTime(JobOption job)
    {
        if (!job.IsEnabled) return null;

        return job.ScheduleType switch
        {
            ScheduleType.Simple => GetNextSimpleRunTime(job),
            ScheduleType.Advanced => GetNextAdvancedRunTime(job),
            ScheduleType.Cron => GetNextCronRunTime(job),
            _ => null
        };
    }

    public TimeSpan GetTimeUntilNextRun(JobOption job)
    {
        var nextRun = GetNextRunTime(job);
        return nextRun.HasValue ? nextRun.Value - DateTime.Now : TimeSpan.MaxValue;
    }

    #region Simple Schedule Logic
    private bool IsSimpleScheduleAllowed(JobOption job)
    {
        if (!job.Period.HasValue) return false;

        // For simple schedules, we check if current time matches the scheduled time
        var now = DateTime.Now;
        if (job.Hour.HasValue && job.Hour != now.Hour) return false;
        if (job.Minute.HasValue && job.Minute != now.Minute) return false;

        return true;
    }

    private DateTime? GetNextSimpleRunTime(JobOption job)
    {
        if (!job.Period.HasValue) return null;

        var now = DateTime.Now;
        var nextRun = now;

        if (job.Hour.HasValue && job.Minute.HasValue)
        {
            nextRun = new DateTime(now.Year, now.Month, now.Day, job.Hour.Value, job.Minute.Value, 0);
            if (nextRun < now)
            {
                nextRun = nextRun.AddDays(1);
            }
        }
        else
        {
            nextRun = now.AddMinutes(job.Period.Value);
        }

        return nextRun;
    }
    #endregion

    #region Advanced Schedule Logic
    private bool IsAdvancedScheduleAllowed(JobOption job)
    {
        var now = DateTime.Now;

        // Check date range
        if (!IsInActiveDateRange(job, now)) return false;

        // Check allowed days
        if (!IsInAllowedDays(job, now)) return false;

        // Check time restrictions
        return IsInAllowedTimeWindow(job, now);
    }

    private DateTime? GetNextAdvancedRunTime(JobOption job)
    {
        var current = DateTime.Now;

        for (int i = 0; i < 365; i++) // Check next 365 days
        {
            var checkDate = current.AddDays(i);

            if (!IsInActiveDateRange(job, checkDate) || !IsInAllowedDays(job, checkDate))
                continue;

            var dayRestrictions = job.TimeRestrictions
                .Where(r => r.DayOfWeek == checkDate.DayOfWeek && !IsExcludedDate(r, checkDate))
                .OrderBy(r => r.StartTime)
                .ToList();

            foreach (var restriction in dayRestrictions)
            {
                var restrictionStart = checkDate.Date + restriction.StartTime;

                if (restrictionStart > current)
                    return restrictionStart;
            }
        }

        return null;
    }

    private bool IsInActiveDateRange(JobOption job, DateTime date)
    {
        if (job.ActiveDateRange == null) return true;

        var afterStart = !job.ActiveDateRange.StartDate.HasValue ||
                       date >= job.ActiveDateRange.StartDate.Value.Date;
        var beforeEnd = !job.ActiveDateRange.EndDate.HasValue ||
                      date <= job.ActiveDateRange.EndDate.Value.Date;

        return afterStart && beforeEnd;
    }

    private bool IsInAllowedDays(JobOption job, DateTime date)
    {
        if (!job.AllowedDays.Any()) return true;
        return job.AllowedDays.Contains(date.DayOfWeek);
    }

    private bool IsInAllowedTimeWindow(JobOption job, DateTime dateTime)
    {
        var dayRestrictions = job.TimeRestrictions
            .Where(r => r.DayOfWeek == dateTime.DayOfWeek && !IsExcludedDate(r, dateTime))
            .ToList();

        if (!dayRestrictions.Any()) return false;

        var currentTime = dateTime.TimeOfDay;
        return dayRestrictions.Any(r => currentTime >= r.StartTime && currentTime <= r.EndTime);
    }

    private bool IsExcludedDate(TimeRestriction restriction, DateTime date)
    {
        return restriction.ExcludedDates.Any(excluded => excluded.Date == date.Date);
    }
    #endregion

    #region Cron Schedule Logic
    private bool IsCronScheduleAllowed(JobOption job)
    {
        if (string.IsNullOrEmpty(job.CronExpression)) return false;

        try
        {
            var expression = CrontabSchedule.Parse(job.CronExpression);
            var nextOccurrence = expression.GetNextOccurrence(DateTime.Now.AddMinutes(-1));
            return nextOccurrence <= DateTime.Now && DateTime.Now < nextOccurrence.AddMinutes(1);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Invalid cron expression: {CronExpression}", job.CronExpression);
            return false;
        }
    }

    private DateTime? GetNextCronRunTime(JobOption job)
    {
        if (string.IsNullOrEmpty(job.CronExpression)) return null;

        try
        {
            var expression = CrontabSchedule.Parse(job.CronExpression);
            return expression.GetNextOccurrence(DateTime.Now);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Invalid cron expression: {CronExpression}", job.CronExpression);
            return null;
        }
    }
    #endregion
}