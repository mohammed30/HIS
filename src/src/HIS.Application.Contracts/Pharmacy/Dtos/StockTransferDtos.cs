using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using HIS.Pharmacy;

namespace HIS.Pharmacy.Dtos;

public class StockTransferDto : FullAuditedEntityDto<Guid>
{
    public string TransferNumber { get; set; }
    public Guid FromWarehouseId { get; set; }
    public string FromWarehouseName { get; set; }
    public Guid ToWarehouseId { get; set; }
    public string ToWarehouseName { get; set; }
    public TransferStatus Status { get; set; }
    public DateTime? TransferDate { get; set; }
    public string? Notes { get; set; }
    public List<StockTransferItemDto> Items { get; set; }
}

public class StockTransferItemDto : EntityDto<Guid>
{
    public Guid StockTransferId { get; set; }
    public Guid DrugId { get; set; }
    public string DrugName { get; set; }
    public int Quantity { get; set; }
    public string? BatchNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
}

public class CreateStockTransferDto
{
    public Guid FromWarehouseId { get; set; }
    public Guid ToWarehouseId { get; set; }
    public string? Notes { get; set; }
    public List<CreateStockTransferItemDto> Items { get; set; }
}

public class CreateStockTransferItemDto
{
    public Guid DrugId { get; set; }
    public int Quantity { get; set; }
    public string? BatchNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
}
