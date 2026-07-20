using HIS.Settings.Tests;
using Xunit;

namespace HIS.EntityFrameworkCore.Applications.Settings;

[Collection("HIS_Db_Collection")]
public class EfCoreDoctorAppServiceTests : DoctorAppServiceTests<HISEntityFrameworkCoreTestModule>
{
}

[Collection("HIS_Db_Collection")]
public class EfCoreClinicAppServiceTests : ClinicAppServiceTests<HISEntityFrameworkCoreTestModule>
{
}

[Collection("HIS_Db_Collection")]
public class EfCoreDepartmentAppServiceTests : DepartmentAppServiceTests<HISEntityFrameworkCoreTestModule>
{
}

[Collection("HIS_Db_Collection")]
public class EfCoreSpecialtyAppServiceTests : SpecialtyAppServiceTests<HISEntityFrameworkCoreTestModule>
{
}

[Collection("HIS_Db_Collection")]
public class EfCoreLaboratoryAppServiceTests : LaboratoryAppServiceTests<HISEntityFrameworkCoreTestModule>
{
}

