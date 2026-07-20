using System;
using Xunit;

namespace HIS.Inventory.Tests;

public class EfCoreInventoryCountAppServiceTests : InventoryCountAppServiceTests<HIS.EntityFrameworkCore.HISEntityFrameworkCoreTestModule>
{
}
