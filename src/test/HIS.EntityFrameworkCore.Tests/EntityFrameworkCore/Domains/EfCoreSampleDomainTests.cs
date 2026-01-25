using HIS.Samples;
using Xunit;

namespace HIS.EntityFrameworkCore.Domains;

[Collection(HISTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<HISEntityFrameworkCoreTestModule>
{

}
