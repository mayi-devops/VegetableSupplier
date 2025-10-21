using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using VegetableSupplier.Models;
using VegetableSupplier.Services;

namespace VegetableSupplier.ViewModels;

[QueryProperty(nameof(Vegetable), "vegetable")]
public partial class VegetableEditViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private string nameEn;

    [ObservableProperty]
    private string nameHi;

    [ObservableProperty]
    private string nameTe;

    [ObservableProperty]
    private ObservableCollection<UnitPriceViewModel> unitPrices;

    [ObservableProperty]
    private ObservableCollection<VegetableUnit> availableUnits;

    private int _vegetableId;

    public VegetableEditViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
        Title = "Add Vegetable";
        UnitPrices = new ObservableCollection<UnitPriceViewModel>();
        LoadUnits();
    }

    private async Task LoadUnits()
    {
        var units = await _databaseService.GetVegetableUnitsAsync();
        AvailableUnits = new ObservableCollection<VegetableUnit>(units);
    }

    public Vegetable Vegetable
    {
        set
        {
            if (value != null)
            {
                _vegetableId = value.Id;
                NameEn = value.NameEn;
                NameHi = value.NameHi;
                NameTe = value.NameTe;
                Title = "Edit Vegetable";

                UnitPrices.Clear();
                foreach (var price in value.UnitPrices)
                {
                    var unit = AvailableUnits.FirstOrDefault(u => u.CodeName == price.Key);
                    if (unit != null)
                    {
                        UnitPrices.Add(new UnitPriceViewModel
                        {
                            Unit = unit,
                            Price = price.Value
                        });
                    }
                }
            }
        }
    }

    [RelayCommand]
    private void AddUnitPrice()
    {
        UnitPrices.Add(new UnitPriceViewModel());
    }

    [RelayCommand]
    private void RemoveUnitPrice(UnitPriceViewModel unitPrice)
    {
        UnitPrices.Remove(unitPrice);
    }

    [RelayCommand]
    private async Task Save()
    {
        if (string.IsNullOrWhiteSpace(NameEn))
        {
            await Shell.Current.DisplayAlert("Error", "English name is required", "OK");
            return;
        }

        if (!UnitPrices.Any())
        {
            await Shell.Current.DisplayAlert("Error", "At least one unit price is required", "OK");
            return;
        }

        if (UnitPrices.Any(up => up.Unit == null || up.Price <= 0))
        {
            await Shell.Current.DisplayAlert("Error", "Invalid unit prices", "OK");
            return;
        }

        var vegetable = new Vegetable
        {
            Id = _vegetableId,
            NameEn = NameEn,
            NameHi = NameHi ?? NameEn,
            NameTe = NameTe ?? NameEn,
            UnitPrices = UnitPrices
                .Where(up => up.Unit != null)
                .ToDictionary(up => up.Unit.CodeName, up => up.Price),
            UpdatedAt = DateTime.UtcNow
        };

        await _databaseService.SaveVegetableAsync(vegetable);
        await Shell.Current.GoToAsync("..");
    }
}

public partial class UnitPriceViewModel : ObservableObject
{
    [ObservableProperty]
    private VegetableUnit unit;

    [ObservableProperty]
    private decimal price;
}