using System.Collections.ObjectModel;

namespace VegetableSupplier.Models;

public class SalesAnalytics
{
    public DateTime Date { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalCost { get; set; }
    public decimal Profit => TotalRevenue - TotalCost;
    public int TotalOrders { get; set; }
    public Dictionary<string, decimal> VegetableWiseSales { get; set; }
    public Dictionary<string, decimal> VendorWisePurchases { get; set; }
}

public class VendorAnalytics
{
    public int VendorId { get; set; }
    public string VendorName { get; set; }
    public decimal TotalPurchases { get; set; }
    public int TotalTransactions { get; set; }
    public decimal AverageTransactionValue { get; set; }
    public List<decimal> PurchaseTrend { get; set; }
    public Dictionary<string, decimal> VegetableWisePurchases { get; set; }
}

public class VegetableAnalytics
{
    public int VegetableId { get; set; }
    public string VegetableName { get; set; }
    public decimal TotalSales { get; set; }
    public decimal AveragePrice { get; set; }
    public List<decimal> PriceTrend { get; set; }
    public int TotalTransactions { get; set; }
    public Dictionary<string, decimal> MonthlyTrend { get; set; }
}