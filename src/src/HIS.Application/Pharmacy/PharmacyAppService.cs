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
    private readonly DrugInteractionService _interactionService;

    public PharmacyAppService(
        IRepository<MedicalOrder, Guid> medicalOrderRepository,
        IRepository<Dispensing, Guid> dispensingRepository,
        InventoryManager inventoryManager,
        IRepository<Warehouse, Guid> warehouseRepository,
        IRepository<Patient, Guid> patientRepository,
        IRepository<InventoryItem, Guid> inventoryItemRepository,
        DrugInteractionService interactionService)
    {
        _medicalOrderRepository = medicalOrderRepository;
        _dispensingRepository = dispensingRepository;
        _inventoryManager = inventoryManager;
        _warehouseRepository = warehouseRepository;
        _patientRepository = patientRepository;
        _inventoryItemRepository = inventoryItemRepository;
        _interactionService = interactionService;
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

    public async Task<List<Inventory.Dtos.InventoryItemDto>> GetPharmacyStockAsync(Guid warehouseId)
    {
        var pharmacyWarehouse = await _warehouseRepository.FirstOrDefaultAsync(x => x.Id == warehouseId);
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

        var pharmacyWarehouse = await _warehouseRepository.FirstOrDefaultAsync(x => x.Id == input.WarehouseId);
        if (pharmacyWarehouse == null)
        {
             throw new UserFriendlyException("Selected Warehouse not found.");
        }

        var drugRepository = LazyServiceProvider.LazyGetRequiredService<IRepository<Drug, Guid>>();
        var drug = await drugRepository.FirstOrDefaultAsync(x => x.ServiceItemId == order.ServiceItemId);
        if (drug == null)
        {
             throw new UserFriendlyException("Drug not found for this service item.");
        }

        // 1. Dispense from Inventory (LIFO)
        var batchDetails = await _inventoryManager.DispenseStockAsync(
            pharmacyWarehouse.Id, 
            drug.ServiceItemId.Value, 
            order.Quantity, 
            order.Id.ToString()
        );

        // 2. Record Dispensing Event
        var dispensing = new Dispensing(Guid.NewGuid(), order.Id, order.PatientId)
        {
            CounselingNotes = input.CounselingNotes
        };
        foreach (var b in batchDetails)
        {
            dispensing.AddItem(order.ServiceItemId, b.BatchId, b.Quantity, b.BatchNumber, b.UnitCost);
        }
        await _dispensingRepository.InsertAsync(dispensing);

        // 3. Complete Order
        order.Status = OrderStatus.Completed;
        await _medicalOrderRepository.UpdateAsync(order);
    }

    public async Task<Dtos.DispensingLabelDto> GetLabelAsync(Guid dispensingId)
    {
         var dispensing = await _dispensingRepository.GetAsync(dispensingId);
         var patient = await _patientRepository.GetAsync(dispensing.PatientId);
         var order = await _medicalOrderRepository.GetAsync(dispensing.MedicalOrderId);

         return new Dtos.DispensingLabelDto
         {
             PatientName = !string.IsNullOrEmpty(patient.FullNameAr) ? patient.FullNameAr : patient.FullNameEn,
             MRN = patient.MRN,
             DrugName = order.ServiceName,
             DosageInstructions = order.Instructions ?? "As directed",
             DispensedDate = dispensing.CreationTime.ToString("yyyy-MM-dd HH:mm"),
             ExpiryDate = "Check Packaging", // In real app, get from batch
             PharmacistName = "Pharmacist"
         };
    }

    public async Task<List<string>> CheckInteractionsAsync(Guid patientId, string newDrugName)
    {
        // 1. Get active prescriptions for patient
        var activeOrders = await _medicalOrderRepository.GetListAsync(x => x.PatientId == patientId && x.Status != OrderStatus.Completed && x.Status != OrderStatus.Cancelled);
        var activeDrugNames = activeOrders.Select(x => x.ServiceName).ToList();
        
        // 2. Check interactions
        return await _interactionService.CheckInteractionsAsync(newDrugName, activeDrugNames);
    }
}
