using System;
using System.Threading.Tasks;
using HIS.Services;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;

namespace HIS.Pricing;

public class PricingDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<PriceList, Guid> _priceListRepository;
    private readonly IRepository<ServiceItem, Guid> _serviceItemRepository;
    private readonly IRepository<RadiologyItem, Guid> _radiologyItemRepository;
    private readonly IRepository<ServicePrice, Guid> _servicePriceRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ICurrentTenant _currentTenant;

    public PricingDataSeedContributor(
        IRepository<PriceList, Guid> priceListRepository,
        IRepository<ServiceItem, Guid> serviceItemRepository,
        IRepository<RadiologyItem, Guid> radiologyItemRepository,
        IRepository<ServicePrice, Guid> servicePriceRepository,
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant)
    {
        _priceListRepository = priceListRepository;
        _serviceItemRepository = serviceItemRepository;
        _radiologyItemRepository = radiologyItemRepository;
        _servicePriceRepository = servicePriceRepository;
        _guidGenerator = guidGenerator;
        _currentTenant = currentTenant;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        var priceListId = await SeedPriceListAsync();

        // Seed Lab Services
        await SeedLabServiceAsync("LAB-001", "Complete Blood Count (CBC)", 150, priceListId);
        await SeedLabServiceAsync("LAB-002", "Lipid Panel", 200, priceListId);
        await SeedLabServiceAsync("LAB-003", "Fasting Blood Glucose", 80, priceListId);
        await SeedLabServiceAsync("LAB-004", "Liver Function Test (LFT)", 250, priceListId);
        await SeedLabServiceAsync("LAB-005", "Kidney Function Test (KFT)", 250, priceListId);

        // Seed Radiology Services
        await SeedRadiologyServiceAsync("RAD-XRAY-01", "Chest X-Ray (PA View)", "X-Ray", "Chest", 300, priceListId);
        await SeedRadiologyServiceAsync("RAD-MRI-01", "MRI Brain (Non-Contrast)", "MRI", "Brain", 1200, priceListId);
        await SeedRadiologyServiceAsync("RAD-CT-01", "CT Abdomen", "CT", "Abdomen", 900, priceListId);
        await SeedRadiologyServiceAsync("RAD-US-01", "Ultrasound Abdomen", "Ultrasound", "Abdomen", 400, priceListId);

        // Seed Clinic Services (Consultations & Procedures)
        await SeedClinicServiceAsync("CONS-001", "General Consultation", 50, ServiceCategory.Consultation, priceListId);
        await SeedClinicServiceAsync("CONS-002", "Specialist Consultation", 100, ServiceCategory.Consultation, priceListId);
        await SeedClinicServiceAsync("PROC-001", "Wound Dressing", 30, ServiceCategory.Procedure, priceListId);

        // Seed Surgery Services
        await SeedClinicServiceAsync("SURG-001", "Appendectomy (الزائدة الدودية)", 5000, ServiceCategory.Surgery, priceListId);
        await SeedClinicServiceAsync("SURG-002", "Cholecystectomy (استئصال المرارة)", 7000, ServiceCategory.Surgery, priceListId);
        await SeedClinicServiceAsync("SURG-003", "Hernia Repair (إصلاح الفتق)", 4000, ServiceCategory.Surgery, priceListId);
        await SeedClinicServiceAsync("SURG-004", "Knee Replacement (استبدال الركبة)", 15000, ServiceCategory.Surgery, priceListId);
    }

    private async Task<Guid> SeedPriceListAsync()
    {
        var priceListName = "Standard Price List 2025";
        var existing = await _priceListRepository.FirstOrDefaultAsync(x => x.Name == priceListName);
        
        if (existing != null)
        {
            return existing.Id;
        }

        var priceList = new PriceList(
            _guidGenerator.Create(),
            priceListName,
            true, // Is Default
            new DateTime(2025, 1, 1)
        );

        await _priceListRepository.InsertAsync(priceList);
        return priceList.Id;
    }

    private async Task SeedLabServiceAsync(string code, string name, decimal price, Guid priceListId)
    {
        var existing = await _serviceItemRepository.FirstOrDefaultAsync(x => x.Code == code);
        Guid serviceId;

        if (existing == null)
        {
            var service = new ServiceItem(
                _guidGenerator.Create(),
                code,
                name,
                ServiceCategory.LabTest
            );
            await _serviceItemRepository.InsertAsync(service);
            serviceId = service.Id;
        }
        else
        {
            serviceId = existing.Id;
        }

        await SeedPriceAsync(serviceId, priceListId, price);
    }

    private async Task SeedRadiologyServiceAsync(string code, string name, string modality, string bodyPart, decimal price, Guid priceListId)
    {
        var existing = await _radiologyItemRepository.FirstOrDefaultAsync(x => x.Code == code);
        Guid serviceId;

        if (existing == null)
        {
            var radiologyItem = new RadiologyItem(
                _guidGenerator.Create(),
                code,
                name,
                null, // DepartmentId (Optional)
                modality,
                bodyPart
            );
            
            // RadiologyItem inherits from ServiceItem, but needs to be inserted via its own repo or base repo depending on inheritance strategy.
            // Since it's likely Table-Per-Type or similar where RadiologyItem IS A ServiceItem, sticking to _radiologyItemRepository is safest for specific fields.
            await _radiologyItemRepository.InsertAsync(radiologyItem);
            serviceId = radiologyItem.Id;
        }
        else
        {
            serviceId = existing.Id;
        }

        await SeedPriceAsync(serviceId, priceListId, price);
    }

    private async Task SeedClinicServiceAsync(string code, string name, decimal price, ServiceCategory category, Guid priceListId)
    {
        var existing = await _serviceItemRepository.FirstOrDefaultAsync(x => x.Code == code);
        Guid serviceId;

        if (existing == null)
        {
            var service = new ServiceItem(
                _guidGenerator.Create(),
                code,
                name,
                category
            );
            await _serviceItemRepository.InsertAsync(service);
            serviceId = service.Id;
        }
        else
        {
            serviceId = existing.Id;
        }

        await SeedPriceAsync(serviceId, priceListId, price);
    }

    private async Task SeedPriceAsync(Guid serviceId, Guid priceListId, decimal amount)
    {
        var existing = await _servicePriceRepository.FirstOrDefaultAsync(x => x.ServiceItemId == serviceId && x.PriceListId == priceListId);

        if (existing == null)
        {
            var price = new ServicePrice(
                _guidGenerator.Create(),
                priceListId,
                serviceId,
                amount
            );
            await _servicePriceRepository.InsertAsync(price);
        }
    }
}
