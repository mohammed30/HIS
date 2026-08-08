using HIS.HR.Tests;
using Xunit;

namespace HIS.EntityFrameworkCore.Applications.HR;

[Collection("HIS_Db_Collection")]
public class EfCoreHRAppServiceTests : HRAppServiceTests<HISEntityFrameworkCoreTestModule>
{
}
