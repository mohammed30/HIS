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
            "Male Ward A",
            RoomType.Standard,
            500,
            4,
            "1st Floor",
            "General Ward for Male Patients"
        );

        // 2. Standard Room (Female Ward) - 4 Beds
        await CreateRoomAsync(
            "102",
            "Female Ward A",
            RoomType.Standard,
            500,
            4,
            "1st Floor",
            "General Ward for Female Patients"
        );

        // 3. Private Room - 1 Bed
        await CreateRoomAsync(
            "201",
            "Private Room 201",
            RoomType.Private,
            1200,
            1,
            "2nd Floor",
            "Private Room with TV and Fridge"
        );

        // 4. Suite - 1 Bed
        await CreateRoomAsync(
            "301",
            "Royal Suite",
            RoomType.Suite,
            2500,
            1,
            "3rd Floor",
            "Luxury Suite with Guest Area"
        );

        // 5. ICU - 1 Bed
        await CreateRoomAsync(
            "ICU-01",
            "ICU Bed 1",
            RoomType.ICU,
            3000,
            1,
            "ICU",
            "Intensive Care Unit"
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
