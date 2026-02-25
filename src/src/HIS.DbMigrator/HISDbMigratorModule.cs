using HIS.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;

namespace HIS.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(HISEntityFrameworkCoreModule),
    typeof(HISApplicationContractsModule)
)]
public class HISDbMigratorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpUnitOfWorkDefaultOptions>(options =>
        {
            options.TransactionBehavior = UnitOfWorkTransactionBehavior.Disabled;
        });
    }
}
