using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using HIS.Permissions;

namespace HIS.Rooms;

/// <summary>
/// خدمة الغرف
/// </summary>
[Authorize(HISPermissions.Settings.Default)]
public class RoomAppService : CrudAppService<
    Room,
    RoomDto,
    Guid,
    GetRoomsInput,
    CreateUpdateRoomDto>, IRoomAppService
{
    public RoomAppService(IRepository<Room, Guid> repository) : base(repository)
    {
    }

    public override async Task<RoomDto> GetAsync(Guid id)
    {
        var query = await Repository.WithDetailsAsync(x => x.Beds);
        var room = await AsyncExecuter.FirstOrDefaultAsync(query, x => x.Id == id);
        if (room == null) throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(Room), id);
        
        return ObjectMapper.Map<Room, RoomDto>(room);
    }

    public override async Task<RoomDto> CreateAsync(CreateUpdateRoomDto input)
    {
        var roomId = GuidGenerator.Create();
        var room = new Room(
            roomId,
            CurrentTenant.Id,
            input.RoomNumber,
            input.Type,
            input.DailyRate,
            input.BedCount
        )
        {
            Name = input.Name,
            Floor = input.Floor,
            Status = input.Status,
            Notes = input.Notes,
            Amenities = input.Amenities
        };

        // Generate Beds
        for (int i = 1; i <= input.BedCount; i++)
        {
            var bedCode = $"{input.RoomNumber}-{i}";
            room.Beds.Add(new Bed(
                GuidGenerator.Create(),
                CurrentTenant.Id,
                roomId,
                bedCode,
                BedType.Standard, // Default type, can be updated later
                BedStatus.Available
            ));
        }

        await Repository.InsertAsync(room);

        return ObjectMapper.Map<Room, RoomDto>(room);
    }

    public override async Task<RoomDto> UpdateAsync(Guid id, CreateUpdateRoomDto input)
    {
        var room = await Repository.GetAsync(id);
        await Repository.EnsurePropertyLoadedAsync(room, x => x.Beds);

        // Update basic properties
        room.RoomNumber = input.RoomNumber; // Be careful changing this if used in bed codes
        room.Name = input.Name;
        room.Type = input.Type;
        room.DailyRate = input.DailyRate;
        room.Floor = input.Floor;
        room.Status = input.Status;
        room.Notes = input.Notes;
        room.Amenities = input.Amenities;

        // Determine if we need to add/remove beds based on count change
        // This is a naive implementation; mostly we should manage beds individually
        if (input.BedCount > room.Beds.Count)
        {
            int bedsToAdd = input.BedCount - room.Beds.Count;
            int lastNumber = room.Beds.Count;
            for (int i = 1; i <= bedsToAdd; i++)
            {
                room.Beds.Add(new Bed(
                    GuidGenerator.Create(),
                    CurrentTenant.Id,
                    room.Id,
                    $"{input.RoomNumber}-{lastNumber + i}",
                    BedType.Standard
                ));
            }
        }
        else if (input.BedCount < room.Beds.Count)
        {
             // Only remove if available
             // This is risky, skipping for now to avoid data loss on occupied beds
             // Ideally we should warn or not allow reducing count if beds are occupied
        }
        
        room.BedCount = room.Beds.Count; // Sync property
        room.AvailableBeds = room.Beds.Count(x => x.Status == BedStatus.Available);

        await Repository.UpdateAsync(room);
        
        return ObjectMapper.Map<Room, RoomDto>(room);
    }

    protected override async Task<IQueryable<Room>> CreateFilteredQueryAsync(GetRoomsInput input)
    {
        var queryable = await Repository.WithDetailsAsync(x => x.Beds);

        return queryable
            .WhereIf(!string.IsNullOrWhiteSpace(input.SearchText),
                x => x.RoomNumber.Contains(input.SearchText!) ||
                     (x.Name != null && x.Name.Contains(input.SearchText!)))
            .WhereIf(input.Type.HasValue, x => x.Type == input.Type!.Value)
            .WhereIf(input.Status.HasValue, x => x.Status == input.Status!.Value);
    }

    protected override IQueryable<Room> ApplyDefaultSorting(IQueryable<Room> query)
    {
        return query.OrderBy(x => x.RoomNumber);
    }

    /// <summary>
    /// الحصول على الغرف المتاحة فقط
    /// </summary>
    public async Task<List<RoomLookupDto>> GetAvailableRoomsAsync(RoomType? type = null)
    {
        var queryable = await Repository.GetQueryableAsync();
        var rooms = queryable
            .Where(x => x.Status == RoomStatus.Available && x.AvailableBeds > 0)
            .WhereIf(type.HasValue, x => x.Type == type!.Value)
            .OrderBy(x => x.RoomNumber)
            .ToList();

        return ObjectMapper.Map<List<Room>, List<RoomLookupDto>>(rooms);
    }
}
