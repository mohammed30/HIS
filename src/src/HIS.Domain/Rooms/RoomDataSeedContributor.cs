using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;

namespace HIS.Rooms;

public class RoomDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<Room, Guid> _roomRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ICurrentTenant _currentTenant;

    public RoomDataSeedContributor(
        IRepository<Room, Guid> roomRepository,
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant)
    {
        _roomRepository = roomRepository;
        _guidGenerator = guidGenerator;
        _currentTenant = currentTenant;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        if (await _roomRepository.GetCountAsync() > 0)
        {
            return;
        }

        // 1. Standard Room (Male Ward) - 4 Beds
        await CreateRoomAsync(
            "101",
            "جناح الرجال (أ)",
            RoomType.Standard,
            500,
            4,
            "الدور الأول",
            "جناح عام للمرضى الرجال"
        );

        // 2. Standard Room (Female Ward) - 4 Beds
        await CreateRoomAsync(
            "102",
            "جناح النساء (أ)",
            RoomType.Standard,
            500,
            4,
            "الدور الأول",
            "جناح عام للمريضات النساء"
        );

        // 3. Private Room - 1 Bed
        await CreateRoomAsync(
            "201",
            "غرفة خاصة 201",
            RoomType.Private,
            1200,
            1,
            "الدور الثاني",
            "غرفة خاصة مع تلفاز وثلاجة"
        );

        // 4. Suite - 1 Bed
        await CreateRoomAsync(
            "301",
            "جناح ملكي",
            RoomType.Suite,
            2500,
            1,
            "الدور الثالث",
            "جناح فاخر مع منطقة ضيوف"
        );

        // 5. ICU - 1 Bed
        await CreateRoomAsync(
            "ICU-01",
            "سرير عناية مركزة 1",
            RoomType.ICU,
            3000,
            1,
            "العناية المركزة",
            "وحدة العناية المركزة"
        );
    }

    private async Task CreateRoomAsync(
        string roomNumber,
        string name,
        RoomType type,
        decimal dailyRate,
        int bedCount,
        string floor,
        string notes)
    {
        var roomId = _guidGenerator.Create();
        var room = new Room(
            roomId,
            _currentTenant.Id,
            roomNumber,
            type,
            dailyRate,
            bedCount
        )
        {
            Name = name,
            Floor = floor,
            Notes = notes,
            Status = RoomStatus.Available
        };

        for (int i = 1; i <= bedCount; i++)
        {
            room.Beds.Add(new Bed(
                _guidGenerator.Create(),
                _currentTenant.Id,
                roomId,
                $"{roomNumber}-{i}",
                BedType.Standard,
                BedStatus.Available
            ));
        }

        await _roomRepository.InsertAsync(room);
    }
}
