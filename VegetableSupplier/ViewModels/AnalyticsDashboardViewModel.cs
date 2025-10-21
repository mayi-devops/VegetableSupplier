using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VegetableSupplier.Models;
using VegetableSupplier.Services;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using Microcharts.Maui;
using SkiaSharp;

namespace VegetableSupplier.ViewModels;

public partial class AnalyticsDashboardViewModel : ObservableObject
{
    private readonly AnalyticsService _analyticsService;
    
    [ObservableProperty]
    private decimal totalRevenue;
    
    [ObservableProperty]
    private decimal totalProfit;
    
    [ObservableProperty]
    private int totalOrders;
    
    [ObservableProperty]
    private Chart salesChart;
    
    [ObservableProperty]
    private Chart topVegetablesChart;
    
    [ObservableProperty]
    private Chart topVendorsChart;
    
    [ObservableProperty]
    private bool isLoading;
    
    [ObservableProperty]
    private string selectedPeriod;

    public ObservableCollection<string> TimePeriods { get; } = new()
    {
        "Today",
        "This Week",
        "This Month",
        "This Year",
        "Custom"
    };

    public AnalyticsDashboardViewModel(AnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
        SelectedPeriod = "This Month";
    }

    [RelayCommand]
    private async Task LoadAnalytics()
    {
        if (IsLoading) return;

        try
        {
            IsLoading = true;

            var (startDate, endDate) = GetDateRange(SelectedPeriod);
            var analytics = await _analyticsService.GetSalesAnalyticsAsync(startDate, endDate);

            TotalRevenue = analytics.TotalRevenue;
            TotalProfit = analytics.Profit;
            TotalOrders = analytics.TotalOrders;

            // Create sales trend chart
            var salesEntries = analytics.VegetableWiseSales
                .OrderByDescending(x => x.Value)
                .Take(5)
                .Select(x => new ChartEntry((float)x.Value)
                {
                    Label = x.Key,
                    ValueLabel = $"₹{x.Value:N0}",
                    Color = SKColor.Parse("#2c3e50")
                })
                .ToList();

            SalesChart = new BarChart
            {
                Entries = salesEntries,
                LabelTextSize = 14,
                ValueLabelOrientation = Orientation.Horizontal,
                LabelOrientation = Orientation.Horizontal
            };

            // Create top vegetables chart
            var vegetableEntries = analytics.VegetableWiseSales
                .OrderByDescending(x => x.Value)
                .Take(5)
                .Select(x => new ChartEntry((float)x.Value)
                {
                    Label = x.Key,
                    ValueLabel = $"₹{x.Value:N0}",
                    Color = SKColor.Parse("#27ae60")
                })
                .ToList();

            TopVegetablesChart = new DonutChart
            {
                Entries = vegetableEntries,
                LabelTextSize = 14
            };

            // Create top vendors chart
            var vendorEntries = analytics.VendorWisePurchases
                .OrderByDescending(x => x.Value)
                .Take(5)
                .Select(x => new ChartEntry((float)x.Value)
                {
                    Label = x.Key,
                    ValueLabel = $"₹{x.Value:N0}",
                    Color = SKColor.Parse("#e74c3c")
                })
                .ToList();

            TopVendorsChart = new RadialGaugeChart
            {
                Entries = vendorEntries,
                LabelTextSize = 14
            };
        }
        finally
        {
            IsLoading = false;
        }
    }

    private (DateTime startDate, DateTime endDate) GetDateRange(string period)
    {
        var now = DateTime.Now;
        return period switch
        {
            "Today" => (now.Date, now.Date.AddDays(1).AddSeconds(-1)),
            "This Week" => (now.AddDays(-(int)now.DayOfWeek).Date, now.Date.AddDays(1).AddSeconds(-1)),
            "This Month" => (new DateTime(now.Year, now.Month, 1), now.Date.AddDays(1).AddSeconds(-1)),
            "This Year" => (new DateTime(now.Year, 1, 1), now.Date.AddDays(1).AddSeconds(-1)),
            _ => (now.AddDays(-30), now.Date.AddDays(1).AddSeconds(-1))
        };
    }

    partial void OnSelectedPeriodChanged(string value)
    {
        LoadAnalytics().ConfigureAwait(false);
    }
}