using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SinaHRM.Domain.Entities;

namespace SinaHRM.Infrastracture.Data.Sql.Configurations;

public sealed class EmploymentContractTemplateVersionConfig : IEntityTypeConfiguration<EmploymentContractTemplateVersion>
{
    public void Configure(EntityTypeBuilder<EmploymentContractTemplateVersion> builder)
    {
        builder.ToTable("EmploymentContractTemplateVersions", "Contract");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.VersionNumber).IsRequired();
        builder.Property(x => x.TemplateText).HasColumnType("nvarchar(max)").IsRequired();
        builder.Property(x => x.ChangeDescription).HasMaxLength(1000);
        builder.Property(x => x.CreatedAt).IsRequired().HasDefaultValueSql("SYSDATETIME()");

        builder.HasIndex(x => new
        {
            x.EmploymentContractTemplateId,
            x.VersionNumber
        }).IsUnique();

        #region Navigation
        builder.HasOne(x => x.Template).WithMany(x => x.Versions).HasForeignKey(x => x.EmploymentContractTemplateId)
                    .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Contracts).WithOne(x => x.TemplateVersion).HasForeignKey(x => x.EmploymentContractTemplateVersionId)
                     .OnDelete(DeleteBehavior.Restrict);

        #endregion 
    }
}