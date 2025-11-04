using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartJobScheduler.Models;
using SmartJobScheduler.Services.Interfaces;

namespace SmartJobScheduler.Jobs;
public abstract class BaseSmartJob<TCommand, TJob> : BackgroundService
       where TCommand : class, ICommand, new()
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    protected readonly ILogger<BaseSmartJob<TCommand, TJob>> _logger;
    private readonly IOptions<JobScheduleOption> _jobScheduleOptions;
    protected readonly string _jobName;

    public BaseSmartJob(IServiceScopeFactory serviceScopeFactory,
                        ILogger<BaseSmartJob<TCommand, TJob>> logger,
                        IOptions<JobScheduleOption> jobScheduleOptions)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _jobScheduleOptions = jobScheduleOptions;
        _jobName = typeof(TJob).Name;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var jobOption = GetJobOption();
        if (jobOption == null)
        {
            _logger.LogWarning("Job configuration not found for: {JobName}", _jobName);
            return;
        }

        _logger.LogInformation("Starting job: {JobName} with schedule type: {ScheduleType}",
            _jobName, jobOption.ScheduleType);

        // Initial delay for scheduled jobs
        if (!jobOption.StartImmediately)
        {
            var initialDelay = GetInitialDelay(jobOption);
            if (initialDelay > TimeSpan.Zero)
            {
                _logger.LogInformation("Job {JobName} waiting {Delay} before first run",
                    _jobName, initialDelay);
                await Task.Delay(initialDelay, stoppingToken);
            }
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            await using var scope = _serviceScopeFactory.CreateAsyncScope();
            var scheduler = scope.ServiceProvider.GetRequiredService<IJobSchedulerService>();
            var dispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

            if (scheduler.ShouldRunNow(jobOption))
            {
                await ExecuteJobCycle(dispatcher, jobOption);
            }
            else
            {
                var nextRun = scheduler.GetNextRunTime(jobOption);
                if (nextRun.HasValue)
                {
                    var delay = nextRun.Value - DateTime.Now;
                    _logger.LogInformation("Job {JobName} next run at {NextRun}, waiting {Delay}",
                        _jobName, nextRun.Value, delay);
                    await Task.Delay(delay, stoppingToken);
                }
                else
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
        }

        _logger.LogInformation("Job {JobName} stopped", _jobName);
    }

    private async Task ExecuteJobCycle(ICommandDispatcher dispatcher, JobOption jobOption)
    {
        using (_logger.BeginScope("JobExecution {JobId}", Guid.NewGuid()))
        {
            try
            {
                var startTime = DateTime.Now;
                _logger.LogInformation("Job {JobName} started at {StartTime}", _jobName, startTime);

                await dispatcher.Send(new TCommand());

                var endTime = DateTime.Now;
                _logger.LogInformation("Job {JobName} completed at {EndTime} (Duration: {Duration}s)",
                    _jobName, endTime, (endTime - startTime).TotalSeconds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing job: {JobName}", _jobName);
            }
        }

        // Wait for next period
        if (jobOption.Period.HasValue)
        {
            await Task.Delay(TimeSpan.FromMinutes(jobOption.Period.Value));
        }
    }

    private JobOption? GetJobOption()
    {
        var jobOption = _jobScheduleOptions.Value.Jobs.FirstOrDefault(j =>
            j.Name.Equals(_jobName, StringComparison.OrdinalIgnoreCase));

        if (jobOption == null)
        {
            _logger.LogWarning("Job configuration not found for: {JobName}. Available jobs: {AvailableJobs}",
                _jobName,
                string.Join(", ", _jobScheduleOptions.Value.Jobs.Select(j => j.Name)));
        }

        return jobOption;
    }

    private TimeSpan GetInitialDelay(JobOption jobOption)
    {
        var nextRun = GetNextRunTime(jobOption);
        return nextRun.HasValue ? nextRun.Value - DateTime.Now : TimeSpan.Zero;
    }

    private DateTime? GetNextRunTime(JobOption jobOption)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var scheduler = scope.ServiceProvider.GetRequiredService<IJobSchedulerService>();
        return scheduler.GetNextRunTime(jobOption);
    }
}