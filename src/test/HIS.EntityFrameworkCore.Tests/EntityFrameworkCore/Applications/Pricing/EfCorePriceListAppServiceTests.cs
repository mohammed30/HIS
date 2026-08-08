using HIS.Pricing.Tests;
using Xunit;

namespace HIS.EntityFrameworkCore.Applications.Pricing;

[Collection("HIS_Db_Collection")]
public class EfCorePriceListAppServiceTests : PriceListAppServiceTests<HISEntityFrameworkCoreTestModule>
{
}
