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
        builder.Property(x => x.BirthCertificateNumber).IsRequired().HasMaxLength(10);
        builder.Property(x => x.PlaceBirth).IsRequired().HasMaxLength(50).IsUnicode(true);
        builder.Property(x => x.PlaceOfIssuanceOfBirthCertificate).IsRequired().HasMaxLength(50).IsUnicode(true);
        builder.Property(x => x.Gneder).IsRequired().HasMaxLength(10).HasConversion<string>();
        builder.Property(x => x.EducationalQualification).IsRequired().HasMaxLength(20).HasConversion<string>();
        builder.Property(x => x.MobileNumber).HasMaxLength(11);
        builder.Property(x => x.PhoneNumber).HasMaxLength(10);
        builder.Property(x => x.Address).HasMaxLength(1000).IsUnicode(true);
        builder.Property(x => x.Email).HasMaxLength(50);
        builder.Property(x => x.PostalCode).HasMaxLength(10);
        builder.Property(x => x.ManagementLayer).HasColumnType("tinyint");
        builder.Property(x => x.MaritalStatus).HasColumnType("tinyint").IsRequired();

        builder.HasIndex(x => x.PersonnelCode).IsUnique();
        builder.HasIndex(x => x.Email).IsUnique();
        builder.HasIndex(x => x.NationalCode).IsUnique();


        #region Nav
        builder.HasOne(c => c.Membership).WithMany(c => c.Emploies).HasForeignKey(c => c.MembershipId);
        builder.HasOne(c => c.Profession).WithMany(c => c.Emploies).HasForeignKey(c => c.ProfessionId);
        builder.HasOne(c => c.JobLocation).WithMany(c => c.Emploies).HasForeignKey(c => c.JobLocationId);
        builder.HasOne(c => c.Department).WithMany(c => c.Emploies).HasForeignKey(c => c.DepartmentId);
        builder.HasMany(c => c.Photos).WithOne(c => c.Employee).HasForeignKey(c => c.EmployeeId);
        #endregion
    }
}
