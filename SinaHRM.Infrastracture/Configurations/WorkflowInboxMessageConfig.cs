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

        builder.HasIndex(x => x.EventId)
            .IsUnique();
    }
}
