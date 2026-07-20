using HIS.Accounting.Tests;
using Xunit;

namespace HIS.EntityFrameworkCore.Applications.Accounting;

[Collection("HIS_Db_Collection")]
public class EfCorePaymentVoucherAppServiceTests : PaymentVoucherAppServiceTests<HISEntityFrameworkCoreTestModule>
{
}
