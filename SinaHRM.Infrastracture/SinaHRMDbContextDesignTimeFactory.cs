using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SinaHRM.Infrastracture.Data.Sql;

public class SinaHRMDbContextDesignTimeFactory : IDesignTimeDbContextFactory<SinaHRMDbContext>
{
    public SinaHRMDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder =
            new DbContextOptionsBuilder<SinaHRMDbContext>();

        optionsBuilder.UseSqlServer(
            "Server=.;" +
            "Initial Catalog=SinaHRMDb;" +
            "User Id=sa;Password=Tomcat745;" +
            "Encrypt=false;" +
            "TrustServerCertificate=True;",
            sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(
                    typeof(SinaHRMDbContext)
                        .Assembly
                        .GetName()
                        .Name);
            });

        return new SinaHRMDbContext(
            optionsBuilder.Options);
    }
}
