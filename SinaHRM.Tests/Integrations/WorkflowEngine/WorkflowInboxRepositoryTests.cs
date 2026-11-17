using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SinaHRM.Application.Integrations.WorkflowEngine.Contracts;
using SinaHRM.Domain.Entities;
using SinaHRM.Infrastracture;
using SinaHRM.Infrastracture.Data.Sql.Repositories;
using Xunit;

namespace SinaHRM.Tests.Integrations.WorkflowEngine;

public sealed class WorkflowInboxRepositoryTests
{
    [Fact]
    public async Task RecordAsync_FirstMessageWithEventId_ShouldReturnAccepted()
    {
        await using var database = await WorkflowInboxTestDatabase.CreateAsync();
        await using var dbContext = database.CreateDbContext();
        var repository = new WorkflowInboxRepository(dbContext);
        var message = CreateMessage(Guid.NewGuid(), """{"event":"completed"}""");

        var result = await repository.RecordAsync(message, CancellationToken.None);

        Assert.Equal(WorkflowInboxRecordOutcome.Accepted, result);
        Assert.Equal(1, await dbContext.WorkflowInboxMessages.CountAsync());
    }

    [Fact]
    public async Task RecordAsync_SameEventIdAndSamePayload_ShouldReturnDuplicate()
    {
        await using var database = await WorkflowInboxTestDatabase.CreateAsync();
        var eventId = Guid.NewGuid();
        const string payload = """{"event":"completed"}""";

        await using (var firstContext = database.CreateDbContext())
        {
            var firstRepository = new WorkflowInboxRepository(firstContext);
            var firstResult = await firstRepository.RecordAsync(
                CreateMessage(eventId, payload),
                CancellationToken.None);

            Assert.Equal(WorkflowInboxRecordOutcome.Accepted, firstResult);
        }

        await using (var secondContext = database.CreateDbContext())
        {
            var secondRepository = new WorkflowInboxRepository(secondContext);
            var duplicateResult = await secondRepository.RecordAsync(
                CreateMessage(eventId, payload),
                CancellationToken.None);

            Assert.Equal(WorkflowInboxRecordOutcome.Duplicate, duplicateResult);
            Assert.Equal(1, await secondContext.WorkflowInboxMessages.CountAsync());
        }
    }

    [Fact]
    public async Task RecordAsync_SameEventIdAndDifferentPayload_ShouldReturnConflict()
    {
        await using var database = await WorkflowInboxTestDatabase.CreateAsync();
        var eventId = Guid.NewGuid();

        await using (var firstContext = database.CreateDbContext())
        {
            var repository = new WorkflowInboxRepository(firstContext);
            await repository.RecordAsync(
                CreateMessage(eventId, """{"event":"completed"}"""),
                CancellationToken.None);
        }

        await using (var secondContext = database.CreateDbContext())
        {
            var repository = new WorkflowInboxRepository(secondContext);
            var result = await repository.RecordAsync(
                CreateMessage(eventId, """{"event":"cancelled"}"""),
                CancellationToken.None);

            Assert.Equal(WorkflowInboxRecordOutcome.Conflict, result);
            Assert.Equal(1, await secondContext.WorkflowInboxMessages.CountAsync());
        }
    }

    [Fact]
    public async Task RecordAsync_ConcurrentSameEventIdAndPayload_ShouldAcceptOnceAndReturnDuplicateForOtherRequest()
    {
        await using var database = await WorkflowInboxTestDatabase.CreateAsync();
        var eventId = Guid.NewGuid();
        const string payload = """{"event":"completed"}""";
        var barrier = new ConcurrentInboxReadBarrier();

        await using var firstContext = database.CreateDbContext(barrier);
        await using var secondContext = database.CreateDbContext(barrier);

        var firstRepository = new WorkflowInboxRepository(firstContext);
        var secondRepository = new WorkflowInboxRepository(secondContext);

        var results = await Task.WhenAll(
            firstRepository.RecordAsync(CreateMessage(eventId, payload), CancellationToken.None),
            secondRepository.RecordAsync(CreateMessage(eventId, payload), CancellationToken.None));

        Assert.Equal(
            new[] { WorkflowInboxRecordOutcome.Accepted, WorkflowInboxRecordOutcome.Duplicate },
            results.OrderBy(x => x).ToArray());

        await using var verificationContext = database.CreateDbContext();
        Assert.Equal(1, await verificationContext.WorkflowInboxMessages.CountAsync());
    }

    private static WorkflowInboxMessage CreateMessage(Guid eventId, string payload) =>
        WorkflowInboxMessage.Create(eventId, payload, DateTimeOffset.UtcNow);
}

internal sealed class WorkflowInboxTestDatabase : IAsyncDisposable
{
    private readonly SqliteConnection _keeperConnection;

    private WorkflowInboxTestDatabase(string connectionString, SqliteConnection keeperConnection)
    {
        ConnectionString = connectionString;
        _keeperConnection = keeperConnection;
    }

    private string ConnectionString { get; }

    public static async Task<WorkflowInboxTestDatabase> CreateAsync()
    {
        var connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = $"WorkflowInboxTests_{Guid.NewGuid():N}",
            Mode = SqliteOpenMode.Memory,
            Cache = SqliteCacheMode.Shared
        }.ToString();

        var keeperConnection = new SqliteConnection(connectionString);
        await keeperConnection.OpenAsync();

        var database = new WorkflowInboxTestDatabase(connectionString, keeperConnection);
        await using var dbContext = database.CreateDbContext();
        await dbContext.Database.EnsureCreatedAsync();

        return database;
    }

    public SinaHRMDbContext CreateDbContext(params IInterceptor[] interceptors)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SinaHRMDbContext>()
            .UseSqlite(ConnectionString);

        if (interceptors.Length > 0)
            optionsBuilder.AddInterceptors(interceptors);

        return new SinaHRMDbContext(optionsBuilder.Options);
    }

    public ValueTask DisposeAsync() => _keeperConnection.DisposeAsync();
}

internal sealed class ConcurrentInboxReadBarrier : DbCommandInterceptor
{
    private readonly TaskCompletionSource _bothReadsReached =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    private int _arrivals;

    public override async ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        if (command.CommandText.Contains(
                "WorkflowInboxMessages",
                StringComparison.OrdinalIgnoreCase) &&
            Interlocked.Increment(ref _arrivals) <= 2)
        {
            if (Volatile.Read(ref _arrivals) == 2)
                _bothReadsReached.TrySetResult();

            await _bothReadsReached.Task.WaitAsync(cancellationToken);
        }

        return result;
    }
}
