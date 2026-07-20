using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using HIS.Notifications;
using Volo.Abp.Modularity;

namespace HIS;

[DependsOn(
    typeof(HISApplicationModule),
    typeof(HISDomainTestModule)
)]
public class HISApplicationTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var hubContext = Substitute.For<IHubContext<NotificationHub>>();
        context.Services.AddSingleton(hubContext);

        var webHostEnvironment = Substitute.For<IWebHostEnvironment>();
        context.Services.AddSingleton(webHostEnvironment);
    }
}
