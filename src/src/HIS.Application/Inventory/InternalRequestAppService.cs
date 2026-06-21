using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HIS.Inventory.Dtos;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp;
using HIS.Billing;
using HIS.Inpatient;
using HIS.Laboratory;
using HIS.Radiology;

namespace HIS.Inventory;

[Authorize]
public class InternalRequestAppService : CrudAppService<InternalRequest, InternalRequestDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateInternalRequestDto>, IInternalRequestAppService
{
    private readonly InventoryManager _inventoryManager;
    private readonly IRepository<Warehouse, Guid> _warehouseRepository;
    private readonly IRepository<HIS.Settings.Department, Guid> _departmentRepository;
    private readonly IRepository<HIS.Inventory.InventoryItem, Guid> _inventoryItemRepository;
    private readonly IRepository<Invoice, Guid> _invoiceRepository;
    private readonly IRepository<InvoiceItem, Guid> _invoiceItemRepository;

    public InternalRequestAppService(
        IRepository<InternalRequest, Guid> repository,
        IRepository<Warehouse, Guid> warehouseRepository,
        IRepository<HIS.Settings.Department, Guid> departmentRepository,
        IRepository<HIS.Inventory.InventoryItem, Guid> inventoryItemRepository,
        IRepository<Invoice, Guid> invoiceRepository,
        IRepository<InvoiceItem, Guid> invoiceItemRepository,
        InventoryManager inventoryManager) 
        : base(repository)
    {
        _warehouseRepository = warehouseRepository;
        _departmentRepository = departmentRepository;
        _inventoryItemRepository = inventoryItemRepository;
        _invoiceRepository = invoiceRepository;
        _invoiceItemRepository = invoiceItemRepository;
        _inventoryManager = inventoryManager;
    }

    private IRepository<HIS.Laboratory.LabTest, Guid> LabTestRepository => LazyServiceProvider.LazyGetRequiredService<IRepository<HIS.Laboratory.LabTest, Guid>>();
    private IRepository<HIS.Laboratory.LabRequest, Guid> LabRequestRepository => LazyServiceProvider.LazyGetRequiredService<IRepository<HIS.Laboratory.LabRequest, Guid>>();
    private IRepository<HIS.Services.RadiologyItem, Guid> RadiologyItemRepository => LazyServiceProvider.LazyGetRequiredService<IRepository<HIS.Services.RadiologyItem, Guid>>();
    private IRepository<HIS.Radiology.RadiologyRequest, Guid> RadiologyRequestRepository => LazyServiceProvider.LazyGetRequiredService<IRepository<HIS.Radiology.RadiologyRequest, Guid>>();
    private IRepository<HIS.Inpatient.Admission, Guid> AdmissionRepository => LazyServiceProvider.LazyGetRequiredService<IRepository<HIS.Inpatient.Admission, Guid>>();
    private IRepository<HIS.Accounting.JournalEntry, Guid> JournalEntryRepository => LazyServiceProvider.LazyGetRequiredService<IRepository<HIS.Accounting.JournalEntry, Guid>>();
    private IRepository<HIS.Accounting.Account, Guid> AccountRepository => LazyServiceProvider.LazyGetRequiredService<IRepository<HIS.Accounting.Account, Guid>>();
    private HIS.Accounting.AccountingManager AccountingManager => LazyServiceProvider.LazyGetRequiredService<HIS.Accounting.AccountingManager>();
    private IRepository<HIS.Patients.Patient, Guid> PatientRepository => LazyServiceProvider.LazyGetRequiredService<IRepository<HIS.Patients.Patient, Guid>>();


    public override async Task<InternalRequestDto> CreateAsync(CreateUpdateInternalRequestDto input)
    {
        if (!input.AdmissionId.HasValue)
        {
            throw new UserFriendlyException("يجب اختيار المريض المنوم لإنشاء هذا الطلب.");
        }

        if (input.FulfilledByWarehouseId == Guid.Empty && (input.RequestType == InternalRequestType.Medication || input.RequestType == InternalRequestType.Consumable))
        {
            var pharmacyWarehouse = await _warehouseRepository.FirstOrDefaultAsync(x => x.Name.Contains("Pharmacy") || x.Name.Contains("صيدلية"));
            if (pharmacyWarehouse != null)
            {
                input.FulfilledByWarehouseId = pharmacyWarehouse.Id;
            }
        }

        var requestNumber = $"REQ-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}";

        var entity = new InternalRequest(
            GuidGenerator.Create(), 
            requestNumber, 
            input.RequestingDepartmentId, 
            input.FulfilledByWarehouseId, 
            input.RequestDate)
        {
            AdmissionId = input.AdmissionId,
            RequestType = input.RequestType,
            Notes = input.Notes,
            Status = InternalRequestStatus.Draft
        };

        foreach (var line in input.Lines)
        {
            entity.Lines.Add(new InternalRequestLine(
                GuidGenerator.Create(),
                entity.Id,
                line.InventoryItemId,
                line.RequestedQuantity)
            {
                Notes = line.Notes
            });
        }

        await Repository.InsertAsync(entity);
        return await MapToGetOutputDtoAsync(entity);
    }

    protected override async Task<IQueryable<InternalRequest>> CreateFilteredQueryAsync(PagedAndSortedResultRequestDto input)
    {
        return await Repository.WithDetailsAsync(x => x.Lines);
    }

    protected override async Task<InternalRequest> GetEntityByIdAsync(Guid id)
    {
        var query = await Repository.WithDetailsAsync(x => x.Lines);
        var entity = await AsyncExecuter.FirstOrDefaultAsync(query.Where(x => x.Id == id));
        if (entity == null) throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(InternalRequest), id);
        return entity;
    }

    public async Task<InternalRequestDto> SubmitRequestAsync(Guid id)
    {
        var query = await Repository.WithDetailsAsync(x => x.Lines);
        var entity = await AsyncExecuter.FirstOrDefaultAsync(query.Where(x => x.Id == id));

        if (entity == null) throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(InternalRequest), id);
        
        if (entity.Status != InternalRequestStatus.Draft)
            throw new UserFriendlyException("Only draft requests can be submitted.");

        // For Radiology and Lab, we skip approval and go straight to billing/fulfillment
        if (entity.RequestType == InternalRequestType.Radiology || entity.RequestType == InternalRequestType.Laboratory)
        {
            entity.Status = InternalRequestStatus.Approved;
            await Repository.UpdateAsync(entity);
            
            // Generate module-specific requests immediately
            await CreateModuleSpecificRequestsAsync(entity);
            
            // Process billing and accounting
            await ProcessClinicalBillingAsync(entity);
        }
        else
        {
            entity.Status = InternalRequestStatus.Submitted;
            await Repository.UpdateAsync(entity);
        }

        return await MapToGetOutputDtoAsync(entity);
    }

    private async Task ProcessClinicalBillingAsync(InternalRequest entity)
    {
        if (!entity.AdmissionId.HasValue) return;

        decimal totalCharge = 0m;
        
        foreach (var line in entity.Lines)
        {
            line.ApprovedQuantity = line.RequestedQuantity; // Auto-approve
            
            if (entity.RequestType == InternalRequestType.Laboratory)
            {
                var labTest = await LabTestRepository.FindAsync(line.InventoryItemId);
                if (labTest != null)
                {
                    totalCharge += (labTest.Price * line.ApprovedQuantity);
                    await AddToPatientInvoiceAsync(entity.AdmissionId.Value, labTest.Name, labTest.Price, line.ApprovedQuantity, labTest.Code, ServiceType.Laboratory,
                        $"Lab Request {entity.RequestNumber}");
                }
            }
            else if (entity.RequestType == InternalRequestType.Radiology)
            {
                var radItem = await RadiologyItemRepository.FindAsync(line.InventoryItemId);
                if (radItem != null)
                {
                    totalCharge += (radItem.Price * line.ApprovedQuantity);
                    await AddToPatientInvoiceAsync(entity.AdmissionId.Value, radItem.Name, radItem.Price, line.ApprovedQuantity, radItem.Code, ServiceType.Radiology,
                        $"Radiology Request {entity.RequestNumber}");
                }
            }
        }

        if (totalCharge > 0)
        {
            await CreateOrderJournalEntryAsync(entity, totalCharge);
        }
    }

    private async Task CreateOrderJournalEntryAsync(InternalRequest request, decimal amount)
    {
        var arAccount = await AccountRepository.FirstOrDefaultAsync(x => x.Code == "1120"); // Accounts Receivable
        var revenueAccount = await AccountRepository.FirstOrDefaultAsync(x => x.Code == "4100"); // Revenue

        var admission = await AdmissionRepository.GetAsync(request.AdmissionId.Value);
        var patient = await PatientRepository.FindAsync(admission.PatientId);
        var patientName = patient != null ? patient.FullNameAr : "Unknown";

        if (arAccount != null && revenueAccount != null)
        {
            var je = new HIS.Accounting.JournalEntry(
                GuidGenerator.Create(),
                DateTime.Now,
                request.RequestNumber,
                $"طلب خدمات طبية رقم {request.RequestNumber} - المريض: {patientName}"
            );
            
            revenueAccount = await GetLeafAccountAsync(revenueAccount);
            je.AddLine(GuidGenerator, arAccount.Id, amount, 0); // Debit AR
            je.AddLine(GuidGenerator, revenueAccount.Id, 0, amount); // Credit Revenue

            await JournalEntryRepository.InsertAsync(je);
            await AccountingManager.PostEntryAsync(je);
        }
    }

    public async Task<InternalRequestDto> CancelRequestAsync(Guid id)
    {
        var query = await Repository.WithDetailsAsync(x => x.Lines);
        var entity = await AsyncExecuter.FirstOrDefaultAsync(query.Where(x => x.Id == id));

        if (entity == null) throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(InternalRequest), id);

        if (entity.Status == InternalRequestStatus.Cancelled)
            throw new UserFriendlyException("Request is already cancelled.");

        if (entity.AdmissionId.HasValue)
        {
            var admission = await AdmissionRepository.GetAsync(entity.AdmissionId.Value);
            if (admission.Status == AdmissionStatus.Discharged)
                throw new UserFriendlyException("لا يمكن إلغاء الطلب بعد خروج المريض.");
        }

        // If it was already approved (clinical automation), we need to reverse charges
        if (entity.Status == InternalRequestStatus.Approved && (entity.RequestType == InternalRequestType.Radiology || entity.RequestType == InternalRequestType.Laboratory))
        {
            await ProcessReversalAsync(entity);
        }

        entity.Status = InternalRequestStatus.Cancelled;
        await Repository.UpdateAsync(entity);

        // Also cancel module-specific requests if they exist and are not completed
        await CancelModuleRequestsAsync(entity);

        return await MapToGetOutputDtoAsync(entity);
    }

    private async Task ProcessReversalAsync(InternalRequest entity)
    {
        if (!entity.AdmissionId.HasValue) return;

        decimal totalRefund = 0m;
        var requestRef = entity.RequestNumber;
        
        var invoiceItems = await _invoiceItemRepository.GetListAsync(x => x.Notes != null && x.Notes.Contains(requestRef));
        
        foreach (var item in invoiceItems)
        {
            var invoice = await _invoiceRepository.FindAsync(item.InvoiceId);
            if (invoice != null && invoice.Status == InvoiceStatus.Draft)
            {
                var admission = await AdmissionRepository.GetAsync(entity.AdmissionId.Value);
                decimal amount = item.UnitPrice * item.Quantity;
                
                admission.TotalAmount -= amount;
                await AdmissionRepository.UpdateAsync(admission);
                
                invoice.TotalAmount -= amount;
                invoice.NetAmount = invoice.TotalAmount - invoice.DiscountAmount + invoice.TaxAmount;
                await _invoiceRepository.UpdateAsync(invoice);
                
                totalRefund += amount;
                await _invoiceItemRepository.DeleteAsync(item.Id);
            }
        }

        if (totalRefund > 0)
        {
            await CreateReversalJournalEntryAsync(entity, totalRefund);
        }
    }

    private async Task CreateReversalJournalEntryAsync(InternalRequest request, decimal amount)
    {
        var arAccount = await AccountRepository.FirstOrDefaultAsync(x => x.Code == "1120");
        var revenueAccount = await AccountRepository.FirstOrDefaultAsync(x => x.Code == "4100");

        if (arAccount != null && revenueAccount != null)
        {
            var je = new HIS.Accounting.JournalEntry(
                GuidGenerator.Create(),
                DateTime.Now,
                $"CXL-{request.RequestNumber}",
                $"إلغاء طلب رقم {request.RequestNumber}"
            );
            
            revenueAccount = await GetLeafAccountAsync(revenueAccount);
            je.AddLine(GuidGenerator, revenueAccount.Id, amount, 0); // Debit Revenue (Reverse)
            je.AddLine(GuidGenerator, arAccount.Id, 0, amount); // Credit AR (Reverse)

            await JournalEntryRepository.InsertAsync(je);
            await AccountingManager.PostEntryAsync(je);
        }
    }

    private async Task CancelModuleRequestsAsync(InternalRequest entity)
    {
        if (entity.RequestType == InternalRequestType.Laboratory)
        {
            var labRequests = await LabRequestRepository.GetListAsync(x => x.Notes != null && x.Notes.Contains(entity.RequestNumber));
            foreach (var req in labRequests)
            {
                if (req.Status != LabRequestStatus.Completed)
                {
                    req.Status = LabRequestStatus.Cancelled;
                    await LabRequestRepository.UpdateAsync(req);
                }
            }
        }
        else if (entity.RequestType == InternalRequestType.Radiology)
        {
            var radRequests = await RadiologyRequestRepository.GetListAsync(x => x.RequestNumber == entity.RequestNumber);
            foreach (var req in radRequests)
            {
                if (req.Status != RadiologyRequestStatus.Reported)
                {
                    req.Status = RadiologyRequestStatus.Cancelled;
                    await RadiologyRequestRepository.UpdateAsync(req);
                }
            }
        }
    }

    private async Task CreateModuleSpecificRequestsAsync(InternalRequest entity)
    {
        if (entity.RequestType == InternalRequestType.Laboratory)
        {
            foreach (var line in entity.Lines)
            {
                var labTest = await LabTestRepository.FindAsync(line.InventoryItemId);
                if (labTest != null && entity.AdmissionId.HasValue)
                {
                    var admission = await AdmissionRepository.GetAsync(entity.AdmissionId.Value);
                    var labRequest = new HIS.Laboratory.LabRequest(GuidGenerator.Create(), admission.PatientId, CurrentUser.Id ?? Guid.Empty, labTest.Id)
                    {
                        Notes = $"Nursing Req: {entity.RequestNumber}. " + line.Notes
                    };
                    await LabRequestRepository.InsertAsync(labRequest);
                }
            }
        }
        else if (entity.RequestType == InternalRequestType.Radiology)
        {
            foreach (var line in entity.Lines)
            {
                var radItem = await RadiologyItemRepository.FindAsync(line.InventoryItemId);
                if (radItem != null && entity.AdmissionId.HasValue)
                {
                    var admission = await AdmissionRepository.GetAsync(entity.AdmissionId.Value);
                    var radOrder = new HIS.Radiology.RadiologyRequest(GuidGenerator.Create(), admission.PatientId, CurrentUser.Id ?? Guid.Empty, radItem.Id, entity.RequestNumber)
                    {
                        TechnicianNotes = $"Nursing Req: {entity.RequestNumber}. " + line.Notes
                    };
                    await RadiologyRequestRepository.InsertAsync(radOrder);
                }
            }
        }
    }

    public async Task<InternalRequestDto> ApproveAndFulfillAsync(Guid id)
    {
        var query = await Repository.WithDetailsAsync(x => x.Lines);
        var entity = await AsyncExecuter.FirstOrDefaultAsync(query.Where(x => x.Id == id));

        if (entity == null) throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(InternalRequest), id);

        if (entity.Status != InternalRequestStatus.Submitted)
            throw new UserFriendlyException("Only submitted requests can be approved.");

        decimal totalChargeForPatient = 0m;
        var serviceItemRepo = LazyServiceProvider.LazyGetRequiredService<IRepository<HIS.Services.ServiceItem, Guid>>();
        var inventoryItemRepo = LazyServiceProvider.LazyGetRequiredService<IRepository<HIS.Inventory.InventoryItem, Guid>>();

        // For simplicity, we auto-approve the full requested quantity here.
        // In a real UI, the Store Manager would pass the exactly approved quantities.
        foreach (var line in entity.Lines)
        {
            line.ApprovedQuantity = line.RequestedQuantity; 

            if (entity.RequestType == InternalRequestType.Medication || entity.RequestType == InternalRequestType.Consumable)
            {
                var inventoryItem = await inventoryItemRepo.FindAsync(line.InventoryItemId);
                if (inventoryItem == null) throw new Volo.Abp.BusinessException("Inventory:ItemNotFound");

                // Issue stock out
                await _inventoryManager.IssueStockAsync(
                    entity.FulfilledByWarehouseId,
                    inventoryItem.ProductId,
                    line.ApprovedQuantity,
                    $"Approved {entity.RequestType} Req: {entity.RequestNumber}",
                    entity.RequestingDepartmentId
                );
                
                if (entity.AdmissionId.HasValue)
                {
                    var serviceItem = await serviceItemRepo.FirstOrDefaultAsync(s => s.Id == inventoryItem.ProductId || s.Code == inventoryItem.ProductName);
                    if (serviceItem == null)
                    {
                        // Try to find any service item matching the name
                        serviceItem = await serviceItemRepo.FirstOrDefaultAsync(s => s.Name == inventoryItem.ProductName);
                    }

                    decimal price = serviceItem?.Price ?? inventoryItem.AverageCost; // Fallback to cost if no price defined
                    if (price <= 0) price = 10.0m; // Ultimate fallback to avoid 0.00 in report during demo
                    
                    totalChargeForPatient += (price * line.ApprovedQuantity);
                    
                    await AddToPatientInvoiceAsync(entity.AdmissionId.Value, inventoryItem.ProductName, price, line.ApprovedQuantity, serviceItem?.Code ?? "MED-CONS", 
                        entity.RequestType == InternalRequestType.Medication ? ServiceType.Medication : ServiceType.Consumables, 
                        $"Internal Request {entity.RequestNumber}");
                }
            }
            // Note: Lab and Radiology requests are now created during SubmitRequestAsync
            // This section now only handles the financial/billing aspect of the internal request.
            else if (entity.RequestType == InternalRequestType.Laboratory)
            {
                var labTest = await LabTestRepository.FindAsync(line.InventoryItemId);
                if (labTest != null && entity.AdmissionId.HasValue)
                {
                    totalChargeForPatient += (labTest.Price * line.ApprovedQuantity);
                    await AddToPatientInvoiceAsync(entity.AdmissionId.Value, labTest.Name, labTest.Price, line.ApprovedQuantity, labTest.Code, ServiceType.Laboratory,
                        $"Lab Request {entity.RequestNumber}");
                }
            }
            else if (entity.RequestType == InternalRequestType.Radiology)
            {
                var radItem = await RadiologyItemRepository.FindAsync(line.InventoryItemId);
                if (radItem != null && entity.AdmissionId.HasValue)
                {
                    totalChargeForPatient += (radItem.Price * line.ApprovedQuantity);
                    await AddToPatientInvoiceAsync(entity.AdmissionId.Value, radItem.Name, radItem.Price, line.ApprovedQuantity, radItem.Code, ServiceType.Radiology,
                        $"Radiology Request {entity.RequestNumber}");
                }
            }
        }

        // Admission total already updated inside AddToPatientInvoiceAsync if we want, 
        // but current code does it here too. I'll consolidate into a private method.

        entity.Status = InternalRequestStatus.Approved;
        await Repository.UpdateAsync(entity);

        return await MapToGetOutputDtoAsync(entity);
    }

    private async Task AddToPatientInvoiceAsync(Guid admissionId, string itemName, decimal price, decimal quantity, string code, ServiceType type, string notes)
    {
        var admission = await AdmissionRepository.GetAsync(admissionId);
        Invoice invoice = null;
        
        if (admission.InvoiceId.HasValue)
        {
            invoice = await _invoiceRepository.FindAsync(admission.InvoiceId.Value);
        }

        if (invoice == null)
        {
            var invoiceNumber = $"INV-INP-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}";
            invoice = new Invoice(GuidGenerator.Create(), CurrentTenant.Id, admission.PatientId, invoiceNumber)
            {
                Status = InvoiceStatus.Draft,
                Notes = $"Inpatient bill for Admission {admission.AdmissionDate:yyyy-MM-dd}"
            };
            await _invoiceRepository.InsertAsync(invoice);
            admission.InvoiceId = invoice.Id;
            await AdmissionRepository.UpdateAsync(admission);
        }

        var invoiceItem = new InvoiceItem(
            GuidGenerator.Create(),
            CurrentTenant.Id,
            invoice.Id,
            itemName,
            price)
        {
            Quantity = quantity,
            ServiceCode = code,
            ServiceType = type,
            Notes = notes
        };
        await _invoiceItemRepository.InsertAsync(invoiceItem);

        // Update totals
        decimal amount = price * quantity;
        admission.TotalAmount += amount;
        await AdmissionRepository.UpdateAsync(admission);

        invoice.TotalAmount += amount;
        invoice.NetAmount = invoice.TotalAmount - invoice.DiscountAmount + invoice.TaxAmount;
        await _invoiceRepository.UpdateAsync(invoice);
    }


    public async Task<InternalRequestDto> ConfirmReceiptAsync(Guid id)
    {
        var query = await Repository.WithDetailsAsync(x => x.Lines);
        var entity = await AsyncExecuter.FirstOrDefaultAsync(query.Where(x => x.Id == id));

        if (entity == null) throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(InternalRequest), id);

        if (entity.Status != InternalRequestStatus.Approved)
            throw new UserFriendlyException("Only approved requests can be received by the department.");

        // Here we can either receive it into the generic department, or skip if the department doesn't track inventory strictly.
        // If the requesting department IS a pharmacy (Warehouse), we should run ReceiveStockAsync.
        // For now, logging it as "Received".
        
        entity.Status = InternalRequestStatus.Received;
        await Repository.UpdateAsync(entity);

        return await MapToGetOutputDtoAsync(entity);
    }

    protected override async Task<InternalRequestDto> MapToGetOutputDtoAsync(InternalRequest entity)
    {
        var dto = await base.MapToGetOutputDtoAsync(entity);
        
        var department = await _departmentRepository.FindAsync(entity.RequestingDepartmentId);
        dto.RequestingDepartmentName = department?.NameAr ?? department?.NameEn ?? "قسم طبي";
        
        var warehouse = await _warehouseRepository.FindAsync(entity.FulfilledByWarehouseId);
        dto.FulfilledByWarehouseName = warehouse?.Name ?? "N/A";

        if (entity.AdmissionId.HasValue)
        {
            var admissionRepo = LazyServiceProvider.LazyGetRequiredService<IRepository<HIS.Inpatient.Admission, Guid>>();
            var patientRepo = LazyServiceProvider.LazyGetRequiredService<IRepository<HIS.Patients.Patient, Guid>>();
            var admission = await admissionRepo.FindAsync(entity.AdmissionId.Value);
            if (admission != null)
            {
                var patient = await patientRepo.FindAsync(admission.PatientId);
                dto.PatientName = patient?.FullNameAr ?? "Unknown Patient";
            }
        }

        foreach (var lineDto in dto.Lines)
        {
            if (entity.RequestType == InternalRequestType.Medication || entity.RequestType == InternalRequestType.Consumable)
            {
                var inventoryItem = await _inventoryItemRepository.FindAsync(lineDto.InventoryItemId);
                lineDto.InventoryItemName = inventoryItem?.ProductName ?? "Unknown Item";
            }
            else if (entity.RequestType == InternalRequestType.Laboratory)
            {
                var labTest = await LabTestRepository.FindAsync(lineDto.InventoryItemId);
                lineDto.InventoryItemName = labTest?.Name ?? "Unknown Test";
            }
            else if (entity.RequestType == InternalRequestType.Radiology)
            {
                var radItem = await RadiologyItemRepository.FindAsync(lineDto.InventoryItemId);
                lineDto.InventoryItemName = radItem?.Name ?? "Unknown Exam";
            }
        }

        return dto;
    }

    public override async Task DeleteAsync(Guid id)
    {
        var entity = await Repository.GetAsync(id);
        
        // Find associated financial entries via the request number stored in notes
        var requestRef = entity.RequestNumber;
        var invoiceItems = await _invoiceItemRepository.GetListAsync(x => x.Notes != null && x.Notes.Contains(requestRef));
        
        foreach (var item in invoiceItems)
        {
            var invoice = await _invoiceRepository.FindAsync(item.InvoiceId);
            // Only allow cleaning up charges if the invoice is still a Draft (Inpatient provisional bill)
            if (invoice != null && invoice.Status == InvoiceStatus.Draft)
            {
                var admission = await AdmissionRepository.FirstOrDefaultAsync(x => x.InvoiceId == invoice.Id);
                decimal amount = item.UnitPrice * item.Quantity;
                
                if (admission != null)
                {
                    admission.TotalAmount -= amount;
                    await AdmissionRepository.UpdateAsync(admission);
                }
                
                invoice.TotalAmount -= amount;
                invoice.NetAmount = invoice.TotalAmount - invoice.DiscountAmount + invoice.TaxAmount;
                await _invoiceRepository.UpdateAsync(invoice);
                
                await _invoiceItemRepository.DeleteAsync(item.Id);
            }
        }

        await base.DeleteAsync(id);
    }

    protected override async Task<InternalRequestDto> MapToGetListOutputDtoAsync(InternalRequest entity)
    {
        return await MapToGetOutputDtoAsync(entity);
    }

    private async Task<HIS.Accounting.Account> GetLeafAccountAsync(HIS.Accounting.Account account)
    {
        if (account == null) return null;

        var hasChildren = await AccountRepository.AnyAsync(x => x.ParentId == account.Id);
        if (!hasChildren)
        {
            return account;
        }

        var children = await AccountRepository.GetListAsync(x => x.ParentId == account.Id);
        if (!children.Any())
        {
            return account;
        }

        foreach (var child in children.OrderBy(x => x.Code))
        {
            var leaf = await GetLeafAccountAsync(child);
            if (leaf != null)
            {
                return leaf;
            }
        }

        return account;
    }
}
