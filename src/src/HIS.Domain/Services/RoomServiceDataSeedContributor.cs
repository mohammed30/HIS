using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using HIS.Rooms;

namespace HIS.Services;

public class RoomServiceDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<ServiceItem, Guid> _serviceItemRepository;
    private readonly IGuidGenerator _guidGenerator;

    public RoomServiceDataSeedContributor(
        IRepository<ServiceItem, Guid> serviceItemRepository,
        IGuidGenerator guidGenerator)
    {
        _serviceItemRepository = serviceItemRepository;
        _guidGenerator = guidGenerator;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        if (await _serviceItemRepository.AnyAsync(x => x.Category == ServiceCategory.Inpatient))
        {
            return;
        }

        await CreateServiceItemAsync("ROOM-STD", "إقامة غرفة عادية - Standard Room", ServiceCategory.Inpatient, 500);
        await CreateServiceItemAsync("ROOM-PRV", "إقامة غرفة خاصة - Private Room", ServiceCategory.Inpatient, 1200);
        await CreateServiceItemAsync("ROOM-ICU", "إقامة عناية مركزة - ICU", ServiceCategory.Inpatient, 3000);
        await CreateServiceItemAsync("ROOM-SUI", "إقامة جناح - Suite", ServiceCategory.Inpatient, 2500);
        await CreateServiceItemAsync("ROOM-ISO", "إقامة عزل - Isolation Room", ServiceCategory.Inpatient, 1500);
    }

    private async Task CreateServiceItemAsync(string code, string name, ServiceCategory category, decimal price)
    {
        var item = new ServiceItem(_guidGenerator.Create(), code, name, category)
        {
            Price = price,
            IsActive = true
        };
        await _serviceItemRepository.InsertAsync(item);
    }
}
