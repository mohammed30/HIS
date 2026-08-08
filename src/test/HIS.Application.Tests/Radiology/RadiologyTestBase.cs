using Volo.Abp.Modularity;

namespace HIS.Radiology.Tests;

public abstract class RadiologyTestBase<TStartupModule> : HISTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    
}
