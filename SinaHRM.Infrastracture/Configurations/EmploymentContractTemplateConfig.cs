using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SinaHRM.Domain.Entities;

namespace SinaHRM.Infrastracture.Data.Sql.Configurations;

internal class EmploymentContractTemplateConfig : IEntityTypeConfiguration<EmploymentContractTemplate>
{
    public void Configure(EntityTypeBuilder<EmploymentContractTemplate> builder)
    {
        builder.ToTable("EmploymentContractTemplates", "Contract");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Code).HasMaxLength(50);
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.IsDelete).IsRequired().HasDefaultValue(true);
        builder.Property(x => x.CreatedAt).IsRequired().HasDefaultValueSql("SYSDATETIME()");

        builder.HasIndex(x => x.Code)
            .IsUnique();

        #region Navigation
        builder.HasMany(x => x.Versions).WithOne(x => x.Template).HasForeignKey(x => x.EmploymentContractTemplateId)
            .OnDelete(DeleteBehavior.Restrict);

        #endregion

        builder.HasData(
            new EmploymentContractTemplate
            {
                Id = 1,
                Code = "CONTRACT_CENTER",
                Title = "قرارداد استخدامی ستاد مرکز",
            },
            new EmploymentContractTemplate
            {
                Id = 2,
                Code = "CONTRACT_BANDAR",
                Title = "قرارداد استخدامی بندر"
            }
        );

    }
}