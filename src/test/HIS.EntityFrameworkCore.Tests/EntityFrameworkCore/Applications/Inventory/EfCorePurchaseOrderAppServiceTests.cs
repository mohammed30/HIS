using System;
using Xunit;

namespace HIS.Inventory.Tests;

public class EfCorePurchaseOrderAppServiceTests : PurchaseOrderAppServiceTests<HIS.EntityFrameworkCore.HISEntityFrameworkCoreTestModule>
{
}
