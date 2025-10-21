using System.Collections.ObjectModel;
using VegetableSupplier.Models;

namespace VegetableSupplier.Services;

public class AnalyticsService
{
    private readonly DatabaseService _database;

    public AnalyticsService(DatabaseService database)
    {
        _database = database;
    }

    public async Task<SalesAnalytics> GetSalesAnalyticsAsync(DateTime startDate, DateTime endDate)
    {
        var analytics = new SalesAnalytics
        {
            Date = DateTime.UtcNow,
            VegetableWiseSales = new Dictionary<string, decimal>(),
            VendorWisePurchases = new Dictionary<string, decimal>()
        };

        // Get all orders in the date range
        var orders = await _database.GetOrdersByDateRangeAsync(startDate, endDate);
        analytics.TotalOrders = orders.Count;
        analytics.TotalRevenue = orders.Sum(o => o.TotalAmount);

        // Calculate vegetable-wise sales
        foreach (var order in orders)
        {
            var items = await _database.GetOrderItemsAsync(order.Id);
            foreach (var item in items)
            {
                var vegetable = await _database.GetVegetableAsync(item.VegetableId);
                if (vegetable != null)
                {
                    var vegName = vegetable.NameEn;
                    if (!analytics.VegetableWiseSales.ContainsKey(vegName))
                        analytics.VegetableWiseSales[vegName] = 0;
                    analytics.VegetableWiseSales[vegName] += item.FinalTotal;
                }
            }
        }

        // Calculate vendor-wise purchases
        var invoices = await _database.GetInvoicesByDateRangeAsync(startDate, endDate);
        analytics.TotalCost = invoices.Sum(i => i.TotalAmount);

        foreach (var invoice in invoices)
        {
            var vendor = await _database.GetVendorAsync(invoice.VendorId);
            if (vendor != null)
            {
                if (!analytics.VendorWisePurchases.ContainsKey(vendor.Name))
                    analytics.VendorWisePurchases[vendor.Name] = 0;
                analytics.VendorWisePurchases[vendor.Name] += invoice.TotalAmount;
            }
        }

        return analytics;
    }

    public async Task<List<VendorAnalytics>> GetVendorAnalyticsAsync(DateTime startDate, DateTime endDate)
    {
        var results = new List<VendorAnalytics>();
        var vendors = await _database.GetVendorsAsync();

        foreach (var vendor in vendors)
        {
            var invoices = await _database.GetVendorInvoicesInRangeAsync(vendor.Id, startDate, endDate);
            var analytics = new VendorAnalytics
            {
                VendorId = vendor.Id,
                VendorName = vendor.Name,
                TotalPurchases = invoices.Sum(i => i.TotalAmount),
                TotalTransactions = invoices.Count,
                PurchaseTrend = new List<decimal>(),
                VegetableWisePurchases = new Dictionary<string, decimal>()
            };

            analytics.AverageTransactionValue = analytics.TotalTransactions > 0 
                ? analytics.TotalPurchases / analytics.TotalTransactions 
                : 0;

            // Calculate monthly trend
            var months = (endDate.Year - startDate.Year) * 12 + endDate.Month - startDate.Month + 1;
            for (int i = 0; i < months; i++)
            {
                var monthStart = startDate.AddMonths(i);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);
                var monthlyInvoices = invoices.Where(i => 
                    i.CreatedAt >= monthStart && i.CreatedAt <= monthEnd);
                analytics.PurchaseTrend.Add(monthlyInvoices.Sum(i => i.TotalAmount));
            }

            results.Add(analytics);
        }

        return results;
    }

    public async Task<List<VegetableAnalytics>> GetVegetableAnalyticsAsync(DateTime startDate, DateTime endDate)
    {
        var results = new List<VegetableAnalytics>();
        var vegetables = await _database.GetVegetablesAsync();

        foreach (var vegetable in vegetables)
        {
            var analytics = new VegetableAnalytics
            {
                VegetableId = vegetable.Id,
                VegetableName = vegetable.NameEn,
                PriceTrend = new List<decimal>(),
                MonthlyTrend = new Dictionary<string, decimal>()
            };

            // Get all orders containing this vegetable
            var orders = await _database.GetOrdersWithVegetableAsync(vegetable.Id, startDate, endDate);
            var orderItems = orders.SelectMany(o => o.Items)
                .Where(i => i.VegetableId == vegetable.Id);

            analytics.TotalSales = orderItems.Sum(i => i.FinalTotal);
            analytics.TotalTransactions = orderItems.Count();
            analytics.AveragePrice = analytics.TotalTransactions > 0 
                ? orderItems.Average(i => i.UnitPrice) 
                : 0;

            // Calculate price trend
            var priceHistory = await _database.GetVegetablePriceHistoryAsync(vegetable.Id, startDate, endDate);
            analytics.PriceTrend = priceHistory.Select(p => p.Price).ToList();

            results.Add(analytics);
        }

        return results;
    }
}