using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using VegetableSupplier.Models;
using VegetableSupplier.Services;

namespace VegetableSupplier.ViewModels;

public partial class InventoryDashboardViewModel : ObservableObject
{
    private readonly InventoryService _inventoryService;
    
    [ObservableProperty]
    private ObservableCollection<InventoryItem> inventory;
    
    [ObservableProperty]
    private ObservableCollection<InventoryItem> lowStockItems;
    
    [ObservableProperty]
    private ObservableCollection<InventoryItem> expiringItems;
    
    [ObservableProperty]
    private bool isLoading;
    
    [ObservableProperty]
    private string searchQuery;
    
    [ObservableProperty]
    private string selectedLocation;

    public ObservableCollection<string> Locations { get; } = new();

    public InventoryDashboardViewModel(InventoryService inventoryService)
    {
        _inventoryService = inventoryService;
        Inventory = new ObservableCollection<InventoryItem>();
        LowStockItems = new ObservableCollection<InventoryItem>();
        ExpiringItems = new ObservableCollection<InventoryItem>();
    }

    [RelayCommand]
    private async Task LoadInventory()
    {
        if (IsLoading) return;

        try
        {
            IsLoading = true;

            var items = await _inventoryService.GetInventoryAsync();
            
            // Update locations list
            var locations = items.Select(i => i.LocationCode).Distinct().ToList();
            Locations.Clear();
            foreach (var location in locations)
                Locations.Add(location);

            // Filter by location if selected
            if (!string.IsNullOrEmpty(SelectedLocation))
                items = items.Where(i => i.LocationCode == SelectedLocation).ToList();

            // Filter by search query if any
            if (!string.IsNullOrEmpty(SearchQuery))
            {
                var query = SearchQuery.ToLower();
                items = items.Where(i => 
                    i.VegetableName.ToLower().Contains(query) ||
                    i.BatchNumber?.ToLower().Contains(query) == true
                ).ToList();
            }

            Inventory.Clear();
            foreach (var item in items)
                Inventory.Add(item);

            // Load low stock items
            var lowStock = await _inventoryService.GetLowStockItemsAsync();
            LowStockItems.Clear();
            foreach (var item in lowStock)
                LowStockItems.Add(item);

            // Load expiring items (within next 7 days)
            var expiring = await _inventoryService.GetExpiringItemsAsync(7);
            ExpiringItems.Clear();
            foreach (var item in expiring)
                ExpiringItems.Add(item);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task AddStock(InventoryItem item)
    {
        var quantity = await Shell.Current.DisplayPromptAsync(
            "Add Stock",
            $"Enter quantity to add for {item.VegetableName}",
            keyboard: Keyboard.Numeric);

        if (!decimal.TryParse(quantity, out var amount))
            return;

        var transaction = new StockTransaction
        {
            InventoryItemId = item.Id,
            TransactionType = "IN",
            Quantity = amount,
            Unit = item.Unit,
            TransactionDate = DateTime.UtcNow,
            SourceLocation = item.LocationCode
        };

        await _inventoryService.AddStockTransactionAsync(transaction);
        await LoadInventory();
    }

    [RelayCommand]
    private async Task RemoveStock(InventoryItem item)
    {
        var quantity = await Shell.Current.DisplayPromptAsync(
            "Remove Stock",
            $"Enter quantity to remove for {item.VegetableName}",
            keyboard: Keyboard.Numeric);

        if (!decimal.TryParse(quantity, out var amount))
            return;

        if (amount > item.Quantity)
        {
            await Shell.Current.DisplayAlert(
                "Error",
                "Cannot remove more than available stock",
                "OK");
            return;
        }

        var reason = await Shell.Current.DisplayPromptAsync(
            "Remove Stock",
            "Enter reason for removal");

        var transaction = new StockTransaction
        {
            InventoryItemId = item.Id,
            TransactionType = "OUT",
            Quantity = amount,
            Unit = item.Unit,
            TransactionDate = DateTime.UtcNow,
            Notes = reason,
            SourceLocation = item.LocationCode
        };

        await _inventoryService.AddStockTransactionAsync(transaction);
        await LoadInventory();
    }

    [RelayCommand]
    private async Task TransferStock(InventoryItem item)
    {
        var quantity = await Shell.Current.DisplayPromptAsync(
            "Transfer Stock",
            $"Enter quantity to transfer for {item.VegetableName}",
            keyboard: Keyboard.Numeric);

        if (!decimal.TryParse(quantity, out var amount))
            return;

        if (amount > item.Quantity)
        {
            await Shell.Current.DisplayAlert(
                "Error",
                "Cannot transfer more than available stock",
                "OK");
            return;
        }

        var destinationLocation = await Shell.Current.DisplayActionSheet(
            "Select Destination",
            "Cancel",
            null,
            Locations.Where(l => l != item.LocationCode).ToArray());

        if (string.IsNullOrEmpty(destinationLocation) || destinationLocation == "Cancel")
            return;

        var transaction = new StockTransaction
        {
            InventoryItemId = item.Id,
            TransactionType = "TRANSFER",
            Quantity = amount,
            Unit = item.Unit,
            TransactionDate = DateTime.UtcNow,
            SourceLocation = item.LocationCode,
            DestinationLocation = destinationLocation
        };

        await _inventoryService.AddStockTransactionAsync(transaction);
        await LoadInventory();
    }

    [RelayCommand]
    private async Task ViewTransactions(InventoryItem item)
    {
        await Shell.Current.GoToAsync("inventorytransactions", new Dictionary<string, object>
        {
            { "inventoryItem", item }
        });
    }

    partial void OnSearchQueryChanged(string value)
    {
        LoadInventory().ConfigureAwait(false);
    }

    partial void OnSelectedLocationChanged(string value)
    {
        LoadInventory().ConfigureAwait(false);
    }
}