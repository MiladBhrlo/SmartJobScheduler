using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartJobScheduler.Models;

namespace SmartJobScheduler.Jobs.SampleJobs;
public class DailyReportJob : BaseSmartJob<SimpleReportCommand, DailyReportJob>
{
    public DailyReportJob(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<DailyReportJob> logger,
        IOptions<JobScheduleOption> jobSchedule)
        : base(serviceScopeFactory, logger, jobSchedule)
    {
    }
}
