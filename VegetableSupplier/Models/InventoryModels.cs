namespace VegetableSupplier.Models;

public class InventoryItem
{
    public int Id { get; set; }
    public int VegetableId { get; set; }
    public string VegetableName { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; }
    public decimal MinimumStock { get; set; }
    public decimal ReorderPoint { get; set; }
    public DateTime LastRestocked { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string BatchNumber { get; set; }
    public string LocationCode { get; set; }
    public string Status { get; set; }
    public decimal WastageQuantity { get; set; }
    public string WastageReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class StockTransaction
{
    public int Id { get; set; }
    public int InventoryItemId { get; set; }
    public string TransactionType { get; set; } // IN, OUT, WASTE, TRANSFER
    public decimal Quantity { get; set; }
    public string Unit { get; set; }
    public decimal UnitPrice { get; set; }
    public string ReferenceNumber { get; set; }
    public string Notes { get; set; }
    public DateTime TransactionDate { get; set; }
    public string SourceLocation { get; set; }
    public string DestinationLocation { get; set; }
    public int? OrderId { get; set; }
    public int? VendorId { get; set; }
    public int? CustomerId { get; set; }
}