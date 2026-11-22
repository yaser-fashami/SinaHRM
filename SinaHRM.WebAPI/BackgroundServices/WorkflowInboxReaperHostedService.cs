using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SinaHRM.Application.Abstractions.Persistence;

namespace SinaHRM.WebAPI.BackgroundServices;

public sealed class WorkflowInboxReaperHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<WorkflowInboxReaperHostedService> _logger;
    private readonly int _batchSize;
    private readonly int _maxAttempts;
    private readonly TimeSpan _interval;

    public WorkflowInboxReaperHostedService(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<WorkflowInboxReaperHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;

        var section = configuration.GetSection("WorkflowIntegration:InboxReaper");
        _batchSize = section.GetValue("BatchSize", 20);
        _maxAttempts = section.GetValue("MaxAttempts", 10);
        _interval = TimeSpan.FromSeconds(section.GetValue("IntervalSeconds", 30));

        if (_batchSize <= 0)
            throw new InvalidOperationException("WorkflowIntegration:InboxReaper:BatchSize must be greater than zero.");
        if (_maxAttempts <= 0)
            throw new InvalidOperationException("WorkflowIntegration:InboxReaper:MaxAttempts must be greater than zero.");
        if (_interval <= TimeSpan.Zero)
            throw new InvalidOperationException("WorkflowIntegration:InboxReaper:IntervalSeconds must be greater than zero.");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(_interval);

        do
        {
            await ReapOnceAsync(stoppingToken);
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task ReapOnceAsync(CancellationToken cancellationToken)
    {
        try
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var repository = scope.ServiceProvider.GetRequiredService<IWorkflowInboxRepository>();

            var movedCount = await repository.ReapExpiredLeasesAsync(
                _batchSize,
                _maxAttempts,
                DateTimeOffset.UtcNow,
                cancellationToken);

            if (movedCount > 0)
            {
                _logger.LogWarning(
                    "Workflow Inbox Reaper moved {MessageCount} expired Processing message(s) to DeadLetter. Operator review is required.",
                    movedCount);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Normal host shutdown.
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Workflow Inbox Reaper iteration failed. It will retry on the next interval.");
        }
    }
}
