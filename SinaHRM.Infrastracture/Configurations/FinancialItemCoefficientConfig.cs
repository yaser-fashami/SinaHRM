using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SinaHRM.Domain.Entities;
using SinaHRM.Infrastracture.Data.Sql.Converters;

namespace SinaHRM.Infrastracture.Data.Sql.Configurations;

internal class FinancialItemCoefficientConfig : IEntityTypeConfiguration<FinancialItemCoefficient>
{
    public void Configure(EntityTypeBuilder<FinancialItemCoefficient> builder)
    {
        var moneyConverter = new MoneyRialConverter();

        builder.ToTable("FinancialItemCoefficients", "Contract", table =>
        {
            table.HasCheckConstraint(
                "CK_FinancialItemCoefficients_IncreaseFactor",
                "[IncreaseFactor] > 0");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.FiscalYear).IsRequired().IsUnicode(false).HasMaxLength(4).IsFixedLength();
        builder.Property(x => x.BaseAmount).IsRequired().HasConversion(moneyConverter).HasPrecision(18, 0);
        builder.Property(x => x.IncreaseFactor).IsRequired().HasPrecision(8, 6);
        builder.Property(x => x.ConstantIncrease).IsRequired().HasConversion(moneyConverter).HasPrecision(18, 0);
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("SYSDATETIME()");

        builder.HasIndex(x => new
        {
            x.FinancialItemDefinitionId,
            x.FiscalYear,
            x.Version
        }).IsUnique();

        #region Navigation
        builder.HasOne(x => x.FinancialItemDefinition).WithMany(x => x.FinancialItemCoefficients).HasForeignKey(x => x.FinancialItemDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);
        #endregion

    }
}
