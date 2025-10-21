namespace VegetableSupplier.Models;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string CustomerType { get; set; } // Regular, Wholesale, Retail
    public decimal CreditLimit { get; set; }
    public decimal CurrentCredit { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CustomerPricing
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int VegetableId { get; set; }
    public decimal SpecialPrice { get; set; }
    public string Unit { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public decimal MinimumQuantity { get; set; }
    public decimal DiscountPercentage { get; set; }
}

public class CustomerOrder
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string OrderNumber { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public string Status { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentStatus { get; set; }
    public string PaymentMode { get; set; }
    public string DeliveryAddress { get; set; }
    public string Notes { get; set; }
    public bool IsRecurring { get; set; }
    public string RecurrencePattern { get; set; }
    public DateTime? NextOrderDate { get; set; }
}

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int VegetableId { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalTotal { get; set; }
}