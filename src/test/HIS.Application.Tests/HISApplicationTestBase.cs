using Volo.Abp.Modularity;

namespace HIS;

public abstract class HISApplicationTestBase<TStartupModule> : HISTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
