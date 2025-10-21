using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using VegetableSupplier.Models;
using VegetableSupplier.Services;

namespace VegetableSupplier.ViewModels;

[INotifyPropertyChanged]
public partial class VegetablesPageViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    
    [ObservableProperty]
    private ObservableCollection<Vegetable> vegetables;
    
    [ObservableProperty]
    private bool isRefreshing;
    
    [ObservableProperty]
    private string searchQuery;

    public VegetablesPageViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
        Vegetables = new ObservableCollection<Vegetable>();
    }

    [RelayCommand]
    private async Task LoadVegetables()
    {
        if (IsRefreshing)
            return;

        try
        {
            IsRefreshing = true;
            var vegetablesList = await _databaseService.GetVegetablesAsync();
            
            Vegetables.Clear();
            foreach (var vegetable in vegetablesList)
            {
                Vegetables.Add(vegetable);
            }
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task Search()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery))
        {
            await LoadVegetables();
            return;
        }

        var query = SearchQuery.ToLower();
        var allVegetables = await _databaseService.GetVegetablesAsync();
        var filteredVegetables = allVegetables.Where(v =>
            v.Name.ToLower().Contains(query) ||
            v.Unit.ToLower().Contains(query));

        Vegetables.Clear();
        foreach (var vegetable in filteredVegetables)
        {
            Vegetables.Add(vegetable);
        }
    }

    [RelayCommand]
    private async Task AddVegetable()
    {
        var vegetableName = await Shell.Current.DisplayPromptAsync(
            AppResources.AddVegetable,
            AppResources.VegetableName);

        if (string.IsNullOrWhiteSpace(vegetableName))
            return;

        var price = await Shell.Current.DisplayPromptAsync(
            AppResources.AddVegetable,
            AppResources.Price,
            keyboard: Keyboard.Numeric);

        if (!decimal.TryParse(price, out var priceValue))
            return;

        var unit = await Shell.Current.DisplayPromptAsync(
            AppResources.AddVegetable,
            "Unit (kg, bunch, piece)",
            initialValue: "kg");

        if (string.IsNullOrWhiteSpace(unit))
            return;

        var vegetable = new Vegetable
        {
            Name = vegetableName,
            Price = priceValue,
            Unit = unit,
            UpdatedAt = DateTime.UtcNow
        };

        await _databaseService.SaveVegetableAsync(vegetable);
        await LoadVegetables();
    }

    [RelayCommand]
    private async Task EditVegetable(Vegetable vegetable)
    {
        var price = await Shell.Current.DisplayPromptAsync(
            AppResources.EditVegetable,
            AppResources.Price,
            initialValue: vegetable.Price.ToString());

        if (!decimal.TryParse(price, out var priceValue))
            return;

        var unit = await Shell.Current.DisplayPromptAsync(
            AppResources.EditVegetable,
            "Unit",
            initialValue: vegetable.Unit);

        if (string.IsNullOrWhiteSpace(unit))
            return;

        vegetable.Price = priceValue;
        vegetable.Unit = unit;
        vegetable.UpdatedAt = DateTime.UtcNow;

        await _databaseService.SaveVegetableAsync(vegetable);
        await LoadVegetables();
    }

    [RelayCommand]
    private async Task DeleteVegetable(Vegetable vegetable)
    {
        var delete = await Shell.Current.DisplayAlert(
            "Delete Vegetable",
            $"Are you sure you want to delete {vegetable.Name}?",
            "Yes", "No");

        if (!delete)
            return;

        // Implement delete in DatabaseService
        await _databaseService.DeleteVegetableAsync(vegetable);
        Vegetables.Remove(vegetable);
    }

    [RelayCommand]
    private Task Refresh()
    {
        return LoadVegetables();
    }
}