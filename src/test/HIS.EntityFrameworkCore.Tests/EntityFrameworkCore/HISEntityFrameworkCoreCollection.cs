using Xunit;

namespace HIS.EntityFrameworkCore;

[CollectionDefinition(HISTestConsts.CollectionDefinitionName)]
public class HISEntityFrameworkCoreCollection : ICollectionFixture<HISEntityFrameworkCoreFixture>
{

}
