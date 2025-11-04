using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartJobScheduler.Jobs.SampleJobs;
using SmartJobScheduler.Models;
using SmartJobScheduler.Services;
using SmartJobScheduler.Services.Interfaces;

namespace SmartJobScheduler.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSmartJobScheduler(this IServiceCollection services, IConfiguration configuration)
    {
        // روش استاندارد Microsoft.Extensions.Options
        services.Configure<JobScheduleOption>(configuration.GetSection("JobScheduler"));

        // Register services
        services.AddScoped<ICommandDispatcher, CommandDispatcherService>();
        services.AddScoped<IJobSchedulerService, JobSchedulerService>();

        // Register jobs
        services.AddHostedService<DailyReportJob>();
        services.AddHostedService<DataCleanupJob>();
        services.AddHostedService<HealthMonitorJob>();

        return services;
    }
}