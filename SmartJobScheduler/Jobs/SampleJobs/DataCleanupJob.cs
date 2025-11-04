using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartJobScheduler.Models;

namespace SmartJobScheduler.Jobs.SampleJobs;
public class DataCleanupJob : BaseSmartJob<DataCleanupCommand, DataCleanupJob>
{
    public DataCleanupJob(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<DataCleanupJob> logger,
        IOptions<JobScheduleOption> jobSchedule)
        : base(serviceScopeFactory, logger, jobSchedule)
    {
    }
}