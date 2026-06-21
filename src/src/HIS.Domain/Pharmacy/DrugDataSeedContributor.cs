using System;
using System.Threading.Tasks;
using HIS.Clinical;
using HIS.Inventory;
using HIS.Services;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

namespace HIS.Pharmacy;

public class DrugDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<Drug, Guid> _drugRepository;
    private readonly IRepository<ServiceItem, Guid> _serviceItemRepository;
    private readonly IRepository<MedicalOrder, Guid> _medicalOrderRepository;
    private readonly IRepository<Warehouse, Guid> _warehouseRepository;
    private readonly IRepository<InventoryItem, Guid> _inventoryItemRepository;
    private readonly IRepository<HIS.Patients.Patient, Guid> _patientRepository;
    private readonly IGuidGenerator _guidGenerator;

    public DrugDataSeedContributor(
        IRepository<Drug, Guid> drugRepository,
        IRepository<ServiceItem, Guid> serviceItemRepository,
        IRepository<MedicalOrder, Guid> medicalOrderRepository,
        IRepository<Warehouse, Guid> warehouseRepository,
        IRepository<InventoryItem, Guid> inventoryItemRepository,
        IRepository<HIS.Patients.Patient, Guid> patientRepository,
        IGuidGenerator guidGenerator)
    {
        _drugRepository = drugRepository;
        _serviceItemRepository = serviceItemRepository;
        _medicalOrderRepository = medicalOrderRepository;
        _warehouseRepository = warehouseRepository;
        _inventoryItemRepository = inventoryItemRepository;
        _patientRepository = patientRepository;
        _guidGenerator = guidGenerator;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        if (await _drugRepository.GetCountAsync() > 0) return;

        // 1. Create Drugs & Linked ServiceItems
        // 1. Create Drugs & Linked ServiceItems
        var panadol = await CreateDrugAsync("6280000001", "Panadol Extra", "Paracetamol + Caffeine", "500mg/65mg", "Tablet", "GSK", 10.0m);
        var aspirin = await CreateDrugAsync("6280000002", "Jusprin", "Aspirin", "81mg", "Tablet", "Julphar", 5.0m);
        var warfarin = await CreateDrugAsync("6280000003", "Warfarin", "Warfarin Sodium", "5mg", "Tablet", "Taro", 15.0m);
        var amoxicillin = await CreateDrugAsync("6280000004", "Amoxil", "Amoxicillin", "500mg", "Capsule", "GSK", 25.0m);
        
        // Additional 6 Drugs (Total 10)
        var augmentin = await CreateDrugAsync("6280000005", "Augmentin", "Amoxicillin + Clavulanic Acid", "1g", "Tablet", "GSK", 45.0m);
        var lipitor = await CreateDrugAsync("6280000006", "Lipitor", "Atorvastatin", "20mg", "Tablet", "Pfizer", 55.0m);
        var glucophage = await CreateDrugAsync("6280000007", "Glucophage", "Metformin", "500mg", "Tablet", "Merck", 12.0m);
        var brufen = await CreateDrugAsync("6280000008", "Brufen", "Ibuprofen", "400mg", "Tablet", "Abbott", 18.0m);
        var pantoloc = await CreateDrugAsync("6280000009", "Pantoloc", "Pantoprazole", "40mg", "Tablet", "Takeda", 35.0m);
        var crestor = await CreateDrugAsync("6280000010", "Crestor", "Rosuvastatin", "10mg", "Tablet", "AstraZeneca", 60.0m);

        // 2. Add Stock to Pharmacy Warehouse
        var pharmacy = await _warehouseRepository.FirstOrDefaultAsync(x => x.Name == "Pharmacy Warehouse" || x.Name == "مستودع الصيدلية");
        if (pharmacy != null)
        {
            await AddStockAsync(pharmacy.Id, panadol.ServiceItemId.Value, panadol.BrandName, InventoryItemType.Medication, 1000, 10);
            await AddStockAsync(pharmacy.Id, aspirin.ServiceItemId.Value, aspirin.BrandName, InventoryItemType.Medication, 500, 5);
            await AddStockAsync(pharmacy.Id, warfarin.ServiceItemId.Value, warfarin.BrandName, InventoryItemType.Medication, 200, 15);
            await AddStockAsync(pharmacy.Id, amoxicillin.ServiceItemId.Value, amoxicillin.BrandName, InventoryItemType.Medication, 300, 25);
            
            // Stock for new items
            await AddStockAsync(pharmacy.Id, augmentin.ServiceItemId.Value, augmentin.BrandName, InventoryItemType.Medication, 150, 40);
            await AddStockAsync(pharmacy.Id, lipitor.ServiceItemId.Value, lipitor.BrandName, InventoryItemType.Medication, 100, 50);
            await AddStockAsync(pharmacy.Id, glucophage.ServiceItemId.Value, glucophage.BrandName, InventoryItemType.Medication, 500, 10);
            await AddStockAsync(pharmacy.Id, brufen.ServiceItemId.Value, brufen.BrandName, InventoryItemType.Medication, 400, 15);
            await AddStockAsync(pharmacy.Id, pantoloc.ServiceItemId.Value, pantoloc.BrandName, InventoryItemType.Medication, 200, 30);
            await AddStockAsync(pharmacy.Id, crestor.ServiceItemId.Value, crestor.BrandName, InventoryItemType.Medication, 120, 55);
        }

        // 3. Create Sample Prescriptions (MedicalOrders)
        var patient = await _patientRepository.FirstOrDefaultAsync();
        if (patient != null)
        {
            // Pending Order: Aspirin
            await CreatePrescriptionAsync(patient.Id, aspirin, 30, "1 Tablet", "Daily", "Oral", "30 Days", "Take with food");
            
            // Active Order (For Interaction Check): Warfarin
            // Creates a completed order to simulate active medication list for interactions (assuming logic checks history)
            // Or create a pending request that represents an active concurrent usage
            await CreatePrescriptionAsync(patient.Id, warfarin, 30, "1 Tablet", "Daily", "Oral", "30 Days", "Monitor INR", OrderStatus.Pending); // Two pending scripts that interact!
        }
    }

    private async Task<Drug> CreateDrugAsync(string barcode, string brand, string scientific, string strength, string form, string manuf, decimal price)
    {
        // 1. ServiceItem
        var svc = new ServiceItem(_guidGenerator.Create(), barcode, $"{brand} {strength}", ServiceCategory.Pharmacy);
        svc.Price = price;
        await _serviceItemRepository.InsertAsync(svc);

        // 2. Drug
        var drug = new Drug(_guidGenerator.Create(), barcode, brand, scientific, strength, form, manuf);
        drug.ServiceItemId = svc.Id;
        await _drugRepository.InsertAsync(drug);
        return drug;
    }

    private async Task AddStockAsync(Guid whId, Guid prodId, string name, InventoryItemType type, decimal qty, decimal cost)
    {
         if (await _inventoryItemRepository.AnyAsync(x => x.ProductId == prodId && x.WarehouseId == whId)) return;
         
         await _inventoryItemRepository.InsertAsync(
             new InventoryItem(_guidGenerator.Create(), whId, prodId, name, type, qty, cost)
         );
    }

    private async Task CreatePrescriptionAsync(Guid patientId, Drug drug, decimal qty, string dose, string freq, string route, string dur, string instr, OrderStatus status = OrderStatus.Pending)
    {
        var order = new MedicalOrder(_guidGenerator.Create(), patientId, OrderType.Medication, drug.ServiceItemId.Value, $"{drug.BrandName} {drug.Strength}", 0);
        order.Quantity = qty;
        order.Details = "Dr. System";
        order.Dosage = dose;
        order.Frequency = freq;
        order.Route = route;
        order.Duration = dur;
        order.Instructions = instr;
        order.Status = status;
        
        await _medicalOrderRepository.InsertAsync(order);
    }
}
