using System;
using Xunit;

namespace HIS.Inventory.Tests;

public class EfCoreInventoryAppServiceTests : InventoryAppServiceTests<HIS.EntityFrameworkCore.HISEntityFrameworkCoreTestModule>
{
}
