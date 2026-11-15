using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SinaHRM.Domain.Entities;

namespace SinaHRM.Infrastracture.Data.Sql.Configurations;

public sealed class EmploymentContractConfig : IEntityTypeConfiguration<EmploymentContract>
{
    public void Configure(EntityTypeBuilder<EmploymentContract> builder)
    {
        builder.ToTable("EmploymentContracts", "Contract");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ContractNumber).HasMaxLength(50).IsRequired();
        builder.Property(x => x.ContractType).HasMaxLength(20).HasConversion<string>().IsRequired();
        builder.Property(x => x.Status).HasMaxLength(20).HasConversion<string>().IsRequired();
        builder.Property(x => x.StartDate).HasColumnType("date").IsRequired();
        builder.Property(x => x.EndDate).HasColumnType("date");
        builder.Property(x => x.FinalText).HasColumnType("nvarchar(max)").IsRequired();
        builder.Property(x => x.WorkflowInstanceId).HasMaxLength(200);
        builder.Property(x => x.CancellationReason).HasMaxLength(1000);
        builder.Property(x => x.CreatedAt).IsRequired().HasDefaultValueSql("SYSDATETIME()");

        builder.HasIndex(x => x.WorkflowInstanceId);
        builder.HasIndex(x => x.ContractNumber).IsUnique();
        builder.HasIndex(x => new { x.EmployeeId, x.StartDate });
        builder.HasIndex(x => new { x.EmployeeId, x.Status });


        #region Navigation

        builder.HasOne(x => x.TemplateVersion).WithMany(x => x.Contracts).HasForeignKey(x => x.EmploymentContractTemplateVersionId)
                .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.FinancialItems).WithOne(x => x.Contract).HasForeignKey(x => x.EmploymentContractId)
                 .OnDelete(DeleteBehavior.Restrict);
        #endregion

    }
}
