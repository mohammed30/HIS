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

    protected override async Task<IQueryable<Room>> CreateFilteredQueryAsync(GetRoomsInput input)
    {
        var queryable = await base.CreateFilteredQueryAsync(input);

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
