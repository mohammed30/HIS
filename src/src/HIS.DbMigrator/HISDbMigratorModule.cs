using HIS.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace HIS.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(HISEntityFrameworkCoreModule),
    typeof(HISApplicationContractsModule)
)]
public class HISDbMigratorModule : AbpModule
{
}
