using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using HIS.Data;
using Serilog;
using Volo.Abp;
using Volo.Abp.Data;

namespace HIS.DbMigrator;

public class DbMigratorHostedService : IHostedService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly IConfiguration _configuration;

    public DbMigratorHostedService(IHostApplicationLifetime hostApplicationLifetime, IConfiguration configuration)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
        _configuration = configuration;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using (var application = await AbpApplicationFactory.CreateAsync<HISDbMigratorModule>(options =>
        {
           options.Services.ReplaceConfiguration(_configuration);
           options.UseAutofac();
           options.Services.AddLogging(c => c.AddSerilog());
           options.AddDataMigrationEnvironment();
        }))
        {
            await application.InitializeAsync();

            try
            {
                var connString = _configuration.GetConnectionString("Default");
                using var conn = new Microsoft.Data.SqlClient.SqlConnection(connString);
                await conn.OpenAsync();
                using var cmd = new Microsoft.Data.SqlClient.SqlCommand("IF OBJECT_ID('AppLabTestNormalRanges', 'U') IS NOT NULL DROP TABLE AppLabTestNormalRanges;", conn);
                await cmd.ExecuteNonQueryAsync();
                System.Console.WriteLine("Dropped AppLabTestNormalRanges successfully.");
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine("Error dropping table: " + ex.Message);
            }

            await application
                .ServiceProvider
                .GetRequiredService<HISDbMigrationService>()
                .MigrateAsync();

            await application.ShutdownAsync();

            _hostApplicationLifetime.StopApplication();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
