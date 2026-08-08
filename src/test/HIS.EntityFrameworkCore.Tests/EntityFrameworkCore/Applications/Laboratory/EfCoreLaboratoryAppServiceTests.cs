using HIS.Laboratory.Tests;
using Xunit;

namespace HIS.EntityFrameworkCore.Applications.Laboratory;

[Collection("HIS_Db_Collection")]
public class EfCoreLabAppServiceTests : LabAppServiceTests<HISEntityFrameworkCoreTestModule>
{
}

[Collection("HIS_Db_Collection")]
public class EfCoreLabReceptionAppServiceTests : LabReceptionAppServiceTests<HISEntityFrameworkCoreTestModule>
{
}
