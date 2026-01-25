using Volo.Abp.Modularity;

namespace HIS;

[DependsOn(
    typeof(HISApplicationModule),
    typeof(HISDomainTestModule)
)]
public class HISApplicationTestModule : AbpModule
{

}
