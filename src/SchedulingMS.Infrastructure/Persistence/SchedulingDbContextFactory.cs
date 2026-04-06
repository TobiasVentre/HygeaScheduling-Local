using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SchedulingMS.Infrastructure.Persistence;

public class SchedulingDbContextFactory : IDesignTimeDbContextFactory<SchedulingDbContext>
{
    private static readonly MySqlServerVersion MySqlVersion = new(new Version(8, 0, 0));

    public SchedulingDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<SchedulingDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Server=localhost;Port=3306;Database=hygea_scheduling;User Id=hygea;Password=change-me;SslMode=Preferred;";

        optionsBuilder.UseMySql(connectionString, MySqlVersion);

        return new SchedulingDbContext(optionsBuilder.Options);
    }
}


