using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace HIS.Pharmacy;

public interface IPharmacyAppService : IApplicationService
{
    Task<List<PendingPrescriptionDto>> GetPendingPrescriptionsAsync();
    Task<PendingPrescriptionDto> GetPrescriptionAsync(System.Guid id);
    Task<List<Inventory.Dtos.InventoryItemDto>> GetPharmacyStockAsync(System.Guid warehouseId);
    Task DispenseMedicationAsync(DispenseDto input);
    Task<List<string>> CheckInteractionsAsync(System.Guid patientId, string newDrugName);
    Task<Dtos.DispensingLabelDto> GetLabelAsync(System.Guid dispensingId);
}
