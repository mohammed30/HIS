using HIS.Users.Tests;
using Xunit;

namespace HIS.EntityFrameworkCore.Applications.Users;

[Collection("HIS_Db_Collection")]
public class EfCoreMyIdentityUserAppServiceTests : MyIdentityUserAppServiceTests<HISEntityFrameworkCoreTestModule>
{
}
