using HIS.Radiology.Tests;
using Xunit;

namespace HIS.EntityFrameworkCore.Applications.Radiology;

[Collection("HIS_Db_Collection")]
public class EfCoreRadiologyAppServiceTests : RadiologyAppServiceTests<HISEntityFrameworkCoreTestModule>
{
}
