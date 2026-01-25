using HIS.ActivityLogs;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace HIS;

/// <summary>
/// Mapper for ActivityLog entities.
/// </summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ActivityLogMapper : MapperBase<ActivityLog, ActivityLogDto>
{
    public override partial ActivityLogDto Map(ActivityLog source);
    
    public override partial void Map(ActivityLog source, ActivityLogDto destination);
}
