using System;
using Xunit;

namespace HIS.Inventory.Tests;

public class EfCoreInventoryTransferTests : InventoryTransferTests<HIS.EntityFrameworkCore.HISEntityFrameworkCoreTestModule>
{
}
