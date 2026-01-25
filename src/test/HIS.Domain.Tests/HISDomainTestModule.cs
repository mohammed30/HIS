using Volo.Abp.Modularity;

namespace HIS;

[DependsOn(
    typeof(HISDomainModule),
    typeof(HISTestBaseModule)
)]
public class HISDomainTestModule : AbpModule
{

}
