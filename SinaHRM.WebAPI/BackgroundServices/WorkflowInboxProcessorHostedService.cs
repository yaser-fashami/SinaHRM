using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SinaHRM.Application.Abstractions.Persistence;
using SinaHRM.Application.Integrations.WorkflowEngine.Contracts;
using SinaHRM.Application.Integrations.WorkflowEngine.Services;
using SinaHRM.Domain.Entities;

namespace SinaHRM.WebAPI.BackgroundServices;

public sealed class WorkflowInboxProcessorHostedService : BackgroundService
{
    private static readonly TimeSpan[] RetryDelays =
    [
        TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(15),
        TimeSpan.FromSeconds(30),
        TimeSpan.FromMinutes(1),
        TimeSpan.FromMinutes(2),
        TimeSpan.FromMinutes(4),
        TimeSpan.FromMinutes(8),
        TimeSpan.FromMinutes(15),
        TimeSpan.FromMinutes(30)
    ];

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<WorkflowInboxProcessorHostedService> _logger;
    private readonly int _batchSize;
    private readonly int _maxAttempts;
    private readonly TimeSpan _leaseDuration;
    private readonly TimeSpan _interval;

    public WorkflowInboxProcessorHostedService(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<WorkflowInboxProcessorHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;

        var section = configuration.GetSection("WorkflowIntegration:InboxProcessor");
        _batchSize = section.GetValue("BatchSize", 20);
        _maxAttempts = section.GetValue("MaxAttempts", 10);
        _leaseDuration = TimeSpan.FromSeconds(section.GetValue("LeaseSeconds", 120));
        _interval = TimeSpan.FromSeconds(section.GetValue("IntervalSeconds", 5));

        if (_batchSize <= 0) throw new InvalidOperationException("InboxProcessor:BatchSize must be greater than zero.");
        if (_maxAttempts <= 0) throw new InvalidOperationException("InboxProcessor:MaxAttempts must be greater than zero.");
        if (_leaseDuration <= TimeSpan.Zero) throw new InvalidOperationException("InboxProcessor:LeaseSeconds must be greater than zero.");
        if (_interval <= TimeSpan.Zero) throw new InvalidOperationException("InboxProcessor:IntervalSeconds must be greater than zero.");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(_interval);
        do
        {
            await ProcessBatchAsync(stoppingToken);
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task ProcessBatchAsync(CancellationToken cancellationToken)
    {
        try
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var repository = scope.ServiceProvider.GetRequiredService<IWorkflowInboxRepository>();
            var processor = scope.ServiceProvider.GetRequiredService<WorkflowInboxProcessor>();
            var now = DateTimeOffset.UtcNow;
            var messages = await repository.ClaimBatchAsync(
                _batchSize, now, now.Add(_leaseDuration), _maxAttempts, cancellationToken);

            foreach (var message in messages)
            {
                if (cancellationToken.IsCancellationRequested) return;
                try
                {
                    var outcome = await processor.ProcessAsync(message, DateTimeOffset.UtcNow, cancellationToken);
                    if (outcome == WorkflowInboxCompletionOutcome.LeaseLost)
                        _logger.LogWarning("Inbox lease was lost before completion. EventId: {EventId}", message.EventId);
                    else if (outcome == WorkflowInboxCompletionOutcome.InvalidContractState)
                        _logger.LogError("Workflow completion event moved to DeadLetter. EventId: {EventId}", message.EventId);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
                catch (Exception exception)
                {
                    await ScheduleRetryAsync(repository, message, exception, cancellationToken);
                }
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Normal host shutdown.
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Workflow Inbox Processor iteration failed. It will retry on the next interval.");
        }
    }

    private async Task ScheduleRetryAsync(
        IWorkflowInboxRepository repository,
        WorkflowInboxMessage message,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (message.LeaseToken is not Guid leaseToken) return;

        var attemptIndex = Math.Clamp(message.AttemptCount - 1, 0, RetryDelays.Length - 1);
        var baseDelay = RetryDelays[attemptIndex];
        var jitterFactor = 1d + (Random.Shared.NextDouble() * 0.4d - 0.2d);
        var nextAttemptAtUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromMilliseconds(baseDelay.TotalMilliseconds * jitterFactor));

        try
        {
            var updated = await repository.ScheduleRetryAsync(
                message.EventId,
                leaseToken,
                nextAttemptAtUtc,
                exception.ToString(),
                _maxAttempts,
                cancellationToken);

            if (updated)
                _logger.LogWarning(exception, "Workflow Inbox processing failed. Retry scheduled for EventId {EventId}, attempt {AttemptCount}.", message.EventId, message.AttemptCount);
            else
                _logger.LogWarning("Workflow Inbox retry was not scheduled because the lease is no longer valid. EventId: {EventId}", message.EventId);
        }
        catch (Exception retryException) when (retryException is not OperationCanceledException)
        {
            _logger.LogError(retryException, "Failed to schedule Workflow Inbox retry. EventId: {EventId}", message.EventId);
        }
    }
}