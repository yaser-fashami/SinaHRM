using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SinaHRM.Domain.Entities;

namespace SinaHRM.Infrastracture.Data.Sql.Configurations;

internal class JobLocationConfig : IEntityTypeConfiguration<JobLocation>
{
    public void Configure(EntityTypeBuilder<JobLocation> builder)
    {
        builder.ToTable("JobLocations", "Basic");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.JobLocationName).IsRequired().HasMaxLength(100).IsUnicode(true);
        builder.Property(x => x.Port).IsRequired().HasConversion<string>();

        #region Nav
        builder.HasMany(c => c.Emploies).WithOne(c => c.JobLocation).HasForeignKey(e => e.JobLocationId);
        #endregion
    }
}
