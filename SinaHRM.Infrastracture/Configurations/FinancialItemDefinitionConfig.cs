using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SinaHRM.Domain.Entities;

namespace SinaHRM.Infrastracture.Data.Sql.Configurations;

public sealed class FinancialItemDefinitionConfig : IEntityTypeConfiguration<FinancialItemDefinition>
{
    public void Configure(EntityTypeBuilder<FinancialItemDefinition> builder)
    {
        builder.ToTable("FinancialItemDefinitions", "Contract");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Code).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.ItemType).HasMaxLength(10).HasConversion<string>().IsRequired();
        builder.Property(x => x.IsDelete).IsRequired().HasDefaultValue(true);
        builder.Property(x => x.AllowManualOverride).IsRequired();
        builder.Property(x => x.DisplayOrder).IsRequired();
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("SYSDATETIME()");

        builder.HasIndex(x => x.Code)
            .IsUnique();

        #region Navigation
        builder.HasMany(x => x.FinancialItemCoefficients).WithOne(x => x.FinancialItemDefinition).HasForeignKey(x => x.FinancialItemDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(x => x.FinancialItemCoefficients).HasField("_financialItemCoefficients")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.ContractItems).WithOne(x => x.Definition).HasForeignKey(x => x.FinancialItemDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(x => x.ContractItems).HasField("_contractItems")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        #endregion
    }
}
