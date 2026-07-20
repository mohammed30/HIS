using HIS.Inventory.Tests;
using Xunit;

namespace HIS.EntityFrameworkCore.Applications.Inventory;

[Collection("HIS_Db_Collection")]
public class EfCoreInternalRequestAppServiceTests : InternalRequestAppServiceTests<HISEntityFrameworkCoreTestModule>
{
}
