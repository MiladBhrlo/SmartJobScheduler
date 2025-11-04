using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartJobScheduler.Models;

namespace SmartJobScheduler.Jobs.SampleJobs;
public class HealthMonitorJob : BaseSmartJob<HealthCheckCommand, HealthMonitorJob>
{
    public HealthMonitorJob(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<HealthMonitorJob> logger,
        IOptions<JobScheduleOption> jobSchedule)
        : base(serviceScopeFactory, logger, jobSchedule)
    {
    }
}