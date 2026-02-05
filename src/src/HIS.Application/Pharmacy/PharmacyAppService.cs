using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HIS.Clinical;
using HIS.Inventory;
using HIS.Patients;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace HIS.Pharmacy;

public class PharmacyAppService : HISAppService, IPharmacyAppService
{
    private readonly IRepository<MedicalOrder, Guid> _medicalOrderRepository;
    private readonly IRepository<Dispensing, Guid> _dispensingRepository;
    private readonly InventoryManager _inventoryManager;
    private readonly IRepository<Warehouse, Guid> _warehouseRepository;
    private readonly IRepository<Patient, Guid> _patientRepository;
    private readonly IRepository<InventoryItem, Guid> _inventoryItemRepository;

    public PharmacyAppService(
        IRepository<MedicalOrder, Guid> medicalOrderRepository,
        IRepository<Dispensing, Guid> dispensingRepository,
        InventoryManager inventoryManager,
        IRepository<Warehouse, Guid> warehouseRepository,
        IRepository<Patient, Guid> patientRepository,
        IRepository<InventoryItem, Guid> inventoryItemRepository)
    {
        _medicalOrderRepository = medicalOrderRepository;
        _dispensingRepository = dispensingRepository;
        _inventoryManager = inventoryManager;
        _warehouseRepository = warehouseRepository;
        _patientRepository = patientRepository;
        _inventoryItemRepository = inventoryItemRepository;
    }

    public async Task<List<PendingPrescriptionDto>> GetPendingPrescriptionsAsync()
    {
        var orders = await _medicalOrderRepository.GetListAsync(x => x.Type == OrderType.Medication && x.Status == OrderStatus.Pending);
        
        var patientIds = orders.Select(x => x.PatientId).Distinct().ToList();
        var patients = await _patientRepository.GetListAsync(x => patientIds.Contains(x.Id));

        var taskList = new List<PendingPrescriptionDto>();

        foreach (var order in orders)
        {
            var patient = patients.FirstOrDefault(p => p.Id == order.PatientId);
            var dto = ObjectMapper.Map<MedicalOrder, PendingPrescriptionDto>(order);
            
            if (patient != null)
            {
                dto.PatientName = !string.IsNullOrEmpty(patient.FullNameAr) ? patient.FullNameAr : patient.FullNameEn;
                dto.PatientMRN = patient.MRN;
            }
            
            taskList.Add(dto);
        }
        return taskList.OrderByDescending(x => x.CreationTime).ToList();
    }

    public async Task<PendingPrescriptionDto> GetPrescriptionAsync(Guid id)
    {
        var order = await _medicalOrderRepository.GetAsync(id);
        var dto = ObjectMapper.Map<MedicalOrder, PendingPrescriptionDto>(order);
        
        var patient = await _patientRepository.GetAsync(order.PatientId);
        if (patient != null)
        {
            dto.PatientName = !string.IsNullOrEmpty(patient.FullNameAr) ? patient.FullNameAr : patient.FullNameEn;
            dto.PatientMRN = patient.MRN;
        }
        return dto;
    }

    public async Task<List<Inventory.Dtos.InventoryItemDto>> GetPharmacyStockAsync()
    {
        var pharmacyWarehouse = await _warehouseRepository.FirstOrDefaultAsync(x => x.Name == "Pharmacy");
        if (pharmacyWarehouse == null) return new List<Inventory.Dtos.InventoryItemDto>();

        var items = await _inventoryItemRepository.GetListAsync(x => x.WarehouseId == pharmacyWarehouse.Id);
        return ObjectMapper.Map<List<InventoryItem>, List<Inventory.Dtos.InventoryItemDto>>(items);
    }

    public async Task DispenseMedicationAsync(DispenseDto input)
    {
        var order = await _medicalOrderRepository.GetAsync(input.MedicalOrderId);
        if (order.Status == OrderStatus.Completed)
        {
             throw new UserFriendlyException("Prescription already dispensed.");
        }

        var pharmacyWarehouse = await _warehouseRepository.FirstOrDefaultAsync(x => x.Name == "Pharmacy");
        if (pharmacyWarehouse == null)
        {
             throw new UserFriendlyException("Pharmacy Warehouse not found. Please contact admin.");
        }

        // 1. Dispense from Inventory (LIFO)
        var batchDetails = await _inventoryManager.DispenseStockAsync(
            pharmacyWarehouse.Id, 
            order.ServiceItemId, 
            order.Quantity, 
            order.Id.ToString()
        );

        // 2. Record Dispensing Event
        var dispensing = new Dispensing(GuidGenerator.Create(), order.Id, order.PatientId);
        foreach (var b in batchDetails)
        {
            dispensing.AddItem(order.ServiceItemId, b.BatchId, b.Quantity, b.BatchNumber, b.UnitCost);
        }
        await _dispensingRepository.InsertAsync(dispensing);

        // 3. Complete Order
        order.Status = OrderStatus.Completed;
        await _medicalOrderRepository.UpdateAsync(order);
    }
}
