using System;
using System.Collections.Generic;
using HIS.Billing;

namespace HIS.Pharmacy.Dtos;

public class PosSaleDto
{
    public Guid? PatientId { get; set; } // Optional for guest checkout
    public List<PosSaleItemDto> Items { get; set; }
    
    // Payment Details
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public PaymentMethod PaymentMethod { get; set; } // Cash, Card, etc. (Need Enum or ID)
}

public class PosSaleItemDto
{
    public Guid DrugId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
}

public class PosProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Barcode { get; set; }
    public decimal Price { get; set; }
    public int CurrentStock { get; set; }
}
