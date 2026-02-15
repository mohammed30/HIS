using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace HIS.Rooms;

#region Room DTOs
public class RoomDto : FullAuditedEntityDto<Guid>
{
    public string RoomNumber { get; set; } = string.Empty;
    public string? Name { get; set; }
    public RoomType Type { get; set; }
    public int BedCount { get; set; }
    public int AvailableBeds { get; set; }
    public decimal DailyRate { get; set; }
    public string? Floor { get; set; }
    public RoomStatus Status { get; set; }
    public string? Notes { get; set; }
    public string? Amenities { get; set; }
    public List<BedDto> Beds { get; set; } = new();
}

public class BedDto : FullAuditedEntityDto<Guid>
{
    public Guid RoomId { get; set; }
    public string BedNumber { get; set; } = string.Empty;
    public BedType Type { get; set; }
    public BedStatus Status { get; set; }
}

public class CreateUpdateRoomDto
{
    public string RoomNumber { get; set; } = string.Empty;
    public string? Name { get; set; }
    public RoomType Type { get; set; }
    public int BedCount { get; set; } = 1;
    public decimal DailyRate { get; set; }
    public string? Floor { get; set; }
    public RoomStatus Status { get; set; } = RoomStatus.Available;
    public string? Notes { get; set; }
    public string? Amenities { get; set; }
}

public class GetRoomsInput : PagedAndSortedResultRequestDto
{
    public string? SearchText { get; set; }
    public RoomType? Type { get; set; }
    public RoomStatus? Status { get; set; }
}
#endregion

#region Room Lookup DTO
public class RoomLookupDto
{
    public Guid Id { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string? Name { get; set; }
    public RoomType Type { get; set; }
    public int AvailableBeds { get; set; }
    public decimal DailyRate { get; set; }
}
#endregion

#region Interface
public interface IRoomAppService
{
}
#endregion
