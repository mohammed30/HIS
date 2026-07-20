using HIS.Billing.Tests;
using Xunit;

namespace HIS.EntityFrameworkCore.Applications.Billing;

[Collection(HISTestConsts.CollectionDefinitionName)]
public class EfCoreBillingAppServiceTests : BillingAppServiceTests<HISEntityFrameworkCoreTestModule>
{

}
