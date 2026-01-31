using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HIS.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class HISDbContextFactory : IDesignTimeDbContextFactory<HISDbContext>
{
    public HISDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        
        HISEfCoreEntityExtensionMappings.Configure();

        var builder = new DbContextOptionsBuilder<HISDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));
        
        return new HISDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../HIS.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables();

        return builder.Build();
    }
}
