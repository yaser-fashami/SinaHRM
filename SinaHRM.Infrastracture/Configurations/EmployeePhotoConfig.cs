using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SinaHRM.Domain.Entities;

namespace SinaHRM.Infrastracture.Data.Sql.Configurations;

internal class EmployeePhotoConfig : IEntityTypeConfiguration<EmployeePhoto>
{
    public void Configure(EntityTypeBuilder<EmployeePhoto> builder)
    {
        builder.ToTable("EmployeePhotos", "Basic");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ContentType).IsRequired().HasMaxLength(5);
        builder.Property(x => x.FileName).HasMaxLength(20);
    }
}