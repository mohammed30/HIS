using HIS.Pharmacy.Tests;
using Xunit;

namespace HIS.EntityFrameworkCore.Applications.Pharmacy;

/// <summary>
/// تنفيذ اختبارات صرف الأدوية على قاعدة بيانات SQLite (In-Memory)
/// Runs all PharmacyDispenseTests against SQLite in-memory via EF Core
/// </summary>
[Collection(HISTestConsts.CollectionDefinitionName)]
public class EfCorePharmacyDispenseTests : PharmacyDispenseTests<HISEntityFrameworkCoreTestModule>
{
}
