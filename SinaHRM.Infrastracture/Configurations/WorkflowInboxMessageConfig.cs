using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SinaHRM.Domain.Entities;

namespace SinaHRM.Infrastracture.Data.Sql.Configurations;

public sealed class WorkflowInboxMessageConfig : IEntityTypeConfiguration<WorkflowInboxMessage>
{
    public void Configure(EntityTypeBuilder<WorkflowInboxMessage> builder)
    {
        builder.ToTable("WorkflowInboxMessages", "Integration");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.EventId)
            .IsRequired();

        builder.Property(x => x.PayloadJson)
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.ReceivedAtUtc)
            .IsRequired();

        builder.Property(x => x.AttemptCount)
            .IsRequired();

        builder.Property(x => x.NextAttemptAtUtc)
            .HasDefaultValueSql("SYSUTCDATETIME()")
            .IsRequired();

        builder.Property(x => x.LeaseToken);

        builder.Property(x => x.LeaseExpiresAtUtc);

        builder.Property(x => x.LastAttemptAtUtc);

        builder.Property(x => x.LastError)
            .HasMaxLength(4000);

        builder.Property(x => x.ProcessedAtUtc);

        builder.HasIndex(x => x.EventId)
            .IsUnique();

        builder.HasIndex(x => new { x.Status, x.NextAttemptAtUtc })
            .HasDatabaseName("IX_WorkflowInboxMessages_Status_NextAttemptAtUtc");

        builder.HasIndex(x => new { x.Status, x.LeaseExpiresAtUtc })
            .HasDatabaseName("IX_WorkflowInboxMessages_Status_LeaseExpiresAtUtc");
    }
}
