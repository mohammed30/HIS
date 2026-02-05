namespace HIS.Inventory;

public enum TransactionType
{
    Receipt = 0,  // Incoming (Purchase)
    Issue = 1,    // Outgoing (Consumption/Sale)
    Transfer = 2, // Internal Movement
    Adjustment = 3, // Correction
    Dispensing = 4 // Pharmacy Dispensing
}
