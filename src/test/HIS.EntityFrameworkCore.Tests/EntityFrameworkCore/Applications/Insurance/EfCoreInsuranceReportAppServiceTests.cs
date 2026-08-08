using HIS.Insurance.Tests;
using Xunit;

namespace HIS.EntityFrameworkCore.Applications.Insurance;

[Collection("HIS_Db_Collection")]
public class EfCoreInsuranceReportAppServiceTests : InsuranceReportAppServiceTests<HISEntityFrameworkCoreTestModule>
{
}
