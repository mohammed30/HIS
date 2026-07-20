using System;
using Xunit;

namespace HIS.Inventory.Tests;

public class EfCorePurchaseInvoiceAppServiceTests : PurchaseInvoiceAppServiceTests<HIS.EntityFrameworkCore.HISEntityFrameworkCoreTestModule>
{
}
