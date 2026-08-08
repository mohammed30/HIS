using HIS.Laboratory;
using Volo.Abp.Modularity;

namespace HIS.Laboratory.Tests;

public abstract class LabTestBase<TStartupModule> : HISTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    
}
