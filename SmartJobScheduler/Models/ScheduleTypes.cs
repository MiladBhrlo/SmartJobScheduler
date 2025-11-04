namespace SmartJobScheduler.Models;
public enum ScheduleType
{
    Simple,
    Advanced,
    Cron
}

public enum JobState
{
    Started,
    Done,
    Stopped,
    Skipped
}

public class JobScheduleOption
{
    public List<JobOption> Jobs { get; set; } = new();
}

public class JobOption
{
    public string Name { get; set; } = string.Empty;
    public ScheduleType ScheduleType { get; set; }
    public bool IsEnabled { get; set; } = true;

    // Simple Schedule Properties
    public int? Hour { get; set; }
    public int? Minute { get; set; }
    public int? Period { get; set; } // in minutes
    public bool StartImmediately { get; set; }

    // Advanced Schedule Properties
    public List<TimeRestriction> TimeRestrictions { get; set; } = new();
    public List<DayOfWeek> AllowedDays { get; set; } = new();
    public DateRange? ActiveDateRange { get; set; }

    // Cron Schedule Properties
    public string? CronExpression { get; set; }
}

public class TimeRestriction
{
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public List<DateTime> ExcludedDates { get; set; } = new();
}

public class DateRange
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

// Sample Commands
public record SimpleReportCommand : ICommand;
public record DataCleanupCommand : ICommand;
public record HealthCheckCommand : ICommand;

public interface ICommand { }