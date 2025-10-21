using SQLite;
using System.Globalization;

namespace VegetableSupplier.Models;

public class Vendor
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class Vegetable
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string NameEn { get; set; }  // English name
    public string NameHi { get; set; }  // Hindi name
    public string NameTe { get; set; }  // Telugu name
    public string Name 
    { 
        get 
        {
            return CultureInfo.CurrentCulture.TwoLetterISOLanguageName switch
            {
                "hi" => NameHi,
                "te" => NameTe,
                _ => NameEn
            };
        }
    }
    public Dictionary<string, decimal> UnitPrices { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class VegetableUnit
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string CodeName { get; set; }
    public string NameEn { get; set; }
    public string NameHi { get; set; }
    public string NameTe { get; set; }
    public string Name 
    { 
        get 
        {
            return CultureInfo.CurrentCulture.TwoLetterISOLanguageName switch
            {
                "hi" => NameHi,
                "te" => NameTe,
                _ => NameEn
            };
        }
    }
}

public class Invoice
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int VendorId { get; set; }
    public DateTime Date { get; set; }
    public decimal TotalAmount { get; set; }
    public string InvoiceNumber { get; set; }
    public string Status { get; set; } // Pending, Paid, Cancelled
    public string DriveFileId { get; set; } // Google Drive File ID
    public DateTime CreatedAt { get; set; }
}

public class InvoiceItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public int VegetableId { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Total { get; set; }
}

public class AuthUser
{
    [PrimaryKey]
    public string Email { get; set; }
    public string DisplayName { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime TokenExpiry { get; set; }
    public bool IsAuthorized { get; set; }
    public DateTime LastChecked { get; set; }
}