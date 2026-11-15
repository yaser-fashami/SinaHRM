using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SinaHRM.Domain.Entities;

namespace SinaHRM.Infrastracture.Data.Sql.Configurations;

internal class MembershipConfig : IEntityTypeConfiguration<Membership>
{
    public void Configure(EntityTypeBuilder<Membership> builder)
    {
        builder.ToTable("Memberships", "Basic");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd(); ;
        builder.Property(x => x.CompanyName).IsRequired().HasMaxLength(100).IsUnicode(true);
        builder.Property(x => x.ApearanceCompanyName).HasMaxLength(100).IsUnicode(true);
        builder.Property(x => x.TitleCompany).HasMaxLength(200).IsUnicode(true);
        builder.Property(x => x.AcountCode).HasMaxLength(20);
        builder.Property(x => x.CEO).HasMaxLength(100).IsUnicode(true);
        builder.Property(x => x.CEOMobile).HasMaxLength(12);
        builder.Property(x => x.Address).HasMaxLength(1000).IsUnicode(true);
        builder.Property(x => x.PostalCode).HasMaxLength(10);

        #region Nav

        #endregion
    }
}
