using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SinaHRM.Domain.Entities;
using SinaHRM.Infrastracture.Data.Sql.Converters;

namespace SinaHRM.Infrastracture.Data.Sql.Configurations;

public sealed class EmploymentContractFinancialItemConfig : IEntityTypeConfiguration<EmploymentContractFinancialItem>
{
    public void Configure(EntityTypeBuilder<EmploymentContractFinancialItem> builder)
    {
        var moneyConverter = new MoneyRialConverter();

        builder.ToTable("EmploymentContractFinancialItems", "Contract");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Code).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.ItemType).HasMaxLength(20).HasConversion<string>().IsRequired();
        builder.Property(x => x.CalculatedAmount).HasConversion(moneyConverter).HasPrecision(18, 0).IsRequired();
        builder.Property(x => x.FinalAmount).HasConversion(moneyConverter).HasPrecision(18, 0).IsRequired();
        builder.Property(x => x.IsManuallyOverridden).IsRequired();
        builder.Property(x => x.OverrideReason).HasMaxLength(1000);
        builder.Property(x => x.DisplayOrder).IsRequired();
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("SYSDATETIME()");

        builder.HasIndex(x => new
        {
            x.EmploymentContractId,
            x.DisplayOrder
        });

        builder.ToTable("EmploymentContractFinancialItems", "Contract", table =>
        {
            table.HasCheckConstraint(
                "CK_EmploymentContractFinancialItems_CalculatedAmount_NonNegative",
                "[CalculatedAmount] >= 0");

            table.HasCheckConstraint(
                "CK_EmploymentContractFinancialItems_FinalAmount_NonNegative",
                "[FinalAmount] >= 0");
        });

        #region Navigation
        builder.HasOne(x => x.Contract).WithMany(x => x.FinancialItems).HasForeignKey(x => x.EmploymentContractId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Definition).WithMany(x => x.ContractItems).HasForeignKey(x => x.FinancialItemDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);

        #endregion
    }
}
