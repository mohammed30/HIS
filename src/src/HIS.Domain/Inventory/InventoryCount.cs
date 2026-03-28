using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.Inventory;

public class InventoryCount : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public Guid WarehouseId { get; set; }
    public DateTime CountDate { get; set; }
    public InventoryCountStatus Status { get; set; }
    public string? Notes { get; set; }

    public virtual ICollection<InventoryCountItem> Items { get; set; } = new List<InventoryCountItem>();

    protected InventoryCount() { }

    public InventoryCount(Guid id, Guid warehouseId, DateTime countDate, Guid? tenantId = null)
        : base(id)
    {
        WarehouseId = warehouseId;
        CountDate = countDate;
        TenantId = tenantId;
        Status = InventoryCountStatus.Draft;
    }
}

public class InventoryCountItem : FullAuditedEntity<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public Guid InventoryCountId { get; set; }
    public Guid InventoryItemId { get; set; }
    public decimal SystemQuantity { get; set; }
    public decimal CountedQuantity { get; set; }
    public decimal Difference => CountedQuantity - SystemQuantity;
    public string? Notes { get; set; }

    protected InventoryCountItem() { }

    public InventoryCountItem(Guid id, Guid inventoryCountId, Guid inventoryItemId, decimal systemQuantity, Guid? tenantId = null)
        : base(id)
    {
        InventoryCountId = inventoryCountId;
        InventoryItemId = inventoryItemId;
        SystemQuantity = systemQuantity;
        CountedQuantity = systemQuantity; // Default to system qty
        TenantId = tenantId;
    }
}
