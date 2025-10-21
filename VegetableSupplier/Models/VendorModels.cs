namespace VegetableSupplier.Models;

public class VendorRating
{
    public int Id { get; set; }
    public int VendorId { get; set; }
    public int Rating { get; set; } // 1-5
    public string Comments { get; set; }
    public DateTime RatingDate { get; set; }
    public string Category { get; set; } // Quality, Price, Reliability, etc.
}

public class VendorPriceHistory
{
    public int Id { get; set; }
    public int VendorId { get; set; }
    public int VegetableId { get; set; }
    public decimal Price { get; set; }
    public string Unit { get; set; }
    public DateTime PriceDate { get; set; }
}

public class VendorCredit
{
    public int Id { get; set; }
    public int VendorId { get; set; }
    public decimal CreditLimit { get; set; }
    public decimal CurrentCredit { get; set; }
    public DateTime LastPaymentDate { get; set; }
    public string PaymentTerms { get; set; }
    public int PaymentDays { get; set; }
    public bool IsActive { get; set; }
}