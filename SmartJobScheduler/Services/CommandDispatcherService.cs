using Microsoft.Extensions.Logging;
using SmartJobScheduler.Models;
using SmartJobScheduler.Services.Interfaces;

namespace SmartJobScheduler.Services;
public class CommandDispatcherService : ICommandDispatcher
{
    private readonly ILogger<CommandDispatcherService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public CommandDispatcherService(ILogger<CommandDispatcherService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task Send<TCommand>(TCommand command) where TCommand : class, ICommand
    {
        try
        {
            _logger.LogInformation("Dispatching command: {CommandType}", typeof(TCommand).Name);

            // Simulate command processing
            await ProcessCommand(command);

            _logger.LogInformation("Command processed successfully: {CommandType}", typeof(TCommand).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing command: {CommandType}", typeof(TCommand).Name);
        }
    }

    private async Task ProcessCommand<TCommand>(TCommand command) where TCommand : class, ICommand
    {
        // Simulate different processing times based on command type
        var processingTime = command switch
        {
            SimpleReportCommand => 1000,
            DataCleanupCommand => 2000,
            HealthCheckCommand => 500,
            _ => 1500
        };

        _logger.LogInformation("Processing {CommandType} for {ProcessingTime}ms",
            typeof(TCommand).Name, processingTime);

        await Task.Delay(processingTime);

        _logger.LogInformation("Completed processing {CommandType}", typeof(TCommand).Name);
    }
}