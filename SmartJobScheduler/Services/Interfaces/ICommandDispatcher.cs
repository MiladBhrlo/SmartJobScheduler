using SmartJobScheduler.Models;

namespace SmartJobScheduler.Services.Interfaces;
public interface ICommandDispatcher
{
    Task Send<TCommand>(TCommand command) where TCommand : class, ICommand;
}

public interface IJobSchedulerService
{
    DateTime? GetNextRunTime(JobOption job);
    bool ShouldRunNow(JobOption job);
    TimeSpan GetTimeUntilNextRun(JobOption job);
}