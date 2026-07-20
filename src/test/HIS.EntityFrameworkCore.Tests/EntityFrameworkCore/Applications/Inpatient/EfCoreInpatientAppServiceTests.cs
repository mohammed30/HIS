using HIS.Inpatient.Tests;
using Xunit;

namespace HIS.EntityFrameworkCore.Applications.Inpatient;

[Collection(HISTestConsts.CollectionDefinitionName)]
public class EfCoreInpatientAppServiceTests : InpatientAppServiceTests<HISEntityFrameworkCoreTestModule>
{

}
