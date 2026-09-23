using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SinaHRM.Domain.Entities;

namespace SinaHRM.Infrastracture.Data.Sql.Configurations;

internal class EmployeeConfig : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Emploies", "Basic");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PersonnelCode).IsRequired().HasMaxLength(10);
        builder.Property(x => x.FirstName).IsRequired().HasMaxLength(50).IsUnicode(true);
        builder.Property(x => x.LastName).IsRequired().HasMaxLength(200).IsUnicode(true);
        builder.Property(x => x.FatherName).IsRequired().HasMaxLength(50).IsUnicode(true);
        builder.Property(x => x.NationalCode).IsRequired().HasMaxLength(10);
        builder.Property(x => x.PlaceBirth).IsRequired().HasMaxLength(50).IsUnicode(true);
        builder.Property(x => x.PlaceOfIssuanceOfBirthCertificate).IsRequired().HasMaxLength(50).IsUnicode(true);
        builder.Property(x => x.Gneder).IsRequired().HasConversion<string>();
        builder.Property(x => x.EducationalQualification).IsRequired().HasConversion<string>();
        builder.Property(x => x.MobileNumber).HasMaxLength(10);
        builder.Property(x => x.PhoneNumber).HasMaxLength(10);
        builder.Property(x => x.Address).HasMaxLength(1000).IsUnicode(true);
        builder.Property(x => x.PostalCode).HasMaxLength(10);
        builder.Property(x => x.MaritalStatus).IsRequired();


        #region Nav
        builder.HasOne(c => c.Membership).WithMany(c => c.Emploies).HasForeignKey(c => c.MembershipId);
        builder.HasOne(c => c.Profession).WithMany(c => c.Emploies).HasForeignKey(c => c.ProfessionId);
        builder.HasOne(c => c.JobLocation).WithMany(c => c.Emploies).HasForeignKey(c => c.JobLocationId);
        #endregion
    }
}
