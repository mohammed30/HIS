using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace HIS.DbMigrator;

class Program
{
    static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Volo.Abp", LogEventLevel.Warning)
#if DEBUG
                .MinimumLevel.Override("HIS", LogEventLevel.Debug)
#else
                .MinimumLevel.Override("HIS", LogEventLevel.Information)
#endif
                .Enrich.FromLogContext()
            .WriteTo.Async(c => c.File("Logs/logs.txt"))
            .WriteTo.Async(c => c.Console())
            .CreateLogger();

        await CreateHostBuilder(args).RunConsoleAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            // تحديد الـ Environment من CLI argument إن وُجد (يتجاوز launchSettings)
            .UseEnvironment(GetEnvironment(args))
            .ConfigureAppConfiguration((context, config) =>
            {
                var env = context.HostingEnvironment.EnvironmentName;

                // ترتيب الأولوية (الأعلى يُلغي الأدنى):
                // 1. appsettings.json           ← قيم افتراضية
                // 2. appsettings.{Env}.json     ← إعدادات البيئة (Development / Production)
                // 3. appsettings.secrets.json   ← أسرار محلية (تُلغي كل ما سبق)
                // 4. Environment Variables      ← أعلى أولوية
                config.SetBasePath(AppContext.BaseDirectory)
                      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                      .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
                      .AddJsonFile("appsettings.secrets.json", optional: true, reloadOnChange: true)
                      .AddEnvironmentVariables();

                Log.Information("Environment: {Env}", env);
                Log.Information("Config files loaded: appsettings.json → appsettings.{Env}.json → appsettings.secrets.json", env);
            })
            .ConfigureLogging((context, logging) => logging.ClearProviders())
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<DbMigratorHostedService>();
            });

    /// <summary>
    /// استخرج اسم البيئة من الـ args (مثال: --environment Production)
    /// أو من متغير البيئة ASPNETCORE_ENVIRONMENT
    /// </summary>
    private static string GetEnvironment(string[] args)
    {
        // البحث عن --environment أو -e في الـ args
        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i].Equals("--environment", StringComparison.OrdinalIgnoreCase) ||
                args[i].Equals("-e", StringComparison.OrdinalIgnoreCase))
            {
                return args[i + 1];
            }
        }

        // الرجوع لمتغير البيئة
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
    }
}
