using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SinaHRM.Infrastracture.Data.Sql;

public class SinaHRMDbContextDesignTimeFactory : IDesignTimeDbContextFactory<SinaHRMDbContext>
{
    public SinaHRMDbContext CreateDbContext(string[] args)
    {
        string cnn = @"Server=.;Initial Catalog=SinaHRMDB;User ID=sa; Password=Tomcat745; Encrypt=false;";
        var options = new DbContextOptionsBuilder().UseSqlServer(cnn).Options;
        return new SinaHRMDbContext(options);
    }
}
