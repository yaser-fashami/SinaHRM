using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SinaHRM.Domain.Entities;

namespace SinaHRM.Infrastracture.Data.Sql.Configurations;

internal class ProfessionConfig : IEntityTypeConfiguration<Profession>
{
    public void Configure(EntityTypeBuilder<Profession> builder)
    {
        builder.ToTable("Professions", "Basic");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.JobName).IsRequired().HasMaxLength(100).IsUnicode(true);

        #region Nav

        #endregion

    }
}
