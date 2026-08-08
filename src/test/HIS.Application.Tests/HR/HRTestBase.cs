using Volo.Abp.Modularity;

namespace HIS.HR.Tests;

public abstract class HRTestBase<TStartupModule> : HISTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    
}
