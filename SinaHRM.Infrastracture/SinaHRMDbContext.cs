using Microsoft.EntityFrameworkCore;
using EntityFramework.Exceptions.SqlServer;
using SinaHRM.Infrastracture.Data.Sql.Configurations;
using SinaHRM.Domain.Entities;


namespace SinaHRM.Infrastracture
{
    public class SinaHRMDbContext : DbContext
    {
        public SinaHRMDbContext(DbContextOptions options) : base(options)
        {
                
        }

        #region Basic
        public DbSet<Employee> Emploies { get; set; }
        public DbSet<Profession> Professions { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<JobLocation> JobLocations { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<EmployeePhoto> EmployeePhotos { get; set; }
        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseExceptionProcessor();
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new EmployeeConfig());
            modelBuilder.ApplyConfiguration(new ProfessionConfig());
            modelBuilder.ApplyConfiguration(new MembershipConfig());
            modelBuilder.ApplyConfiguration(new JobLocationConfig());
            modelBuilder.ApplyConfiguration(new DepartmentConfig());
            modelBuilder.ApplyConfiguration(new EmployeePhotoConfig());

        }
    }
}
