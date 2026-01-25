using HIS.Samples;
using Xunit;

namespace HIS.EntityFrameworkCore.Applications;

[Collection(HISTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<HISEntityFrameworkCoreTestModule>
{

}
