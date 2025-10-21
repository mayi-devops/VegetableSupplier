using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using VegetableSupplier.Models;
using VegetableSupplier.Services;

namespace VegetableSupplier.ViewModels;

public partial class VendorsPageViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    
    [ObservableProperty]
    private ObservableCollection<VendorViewModel> vendors;
    
    [ObservableProperty]
    private bool isRefreshing;
    
    [ObservableProperty]
    private string searchQuery;

    public VendorsPageViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
        Vendors = new ObservableCollection<VendorViewModel>();
    }

    [RelayCommand]
    private async Task LoadVendors()
    {
        if (IsRefreshing)
            return;

        try
        {
            IsRefreshing = true;
            Vendors.Clear();

            var vendorList = await _databaseService.GetVendorsAsync();
            foreach (var vendor in vendorList)
            {
                var invoices = await _databaseService.GetVendorInvoicesAsync(vendor.Id);
                var totalPurchases = invoices.Sum(i => i.TotalAmount);

                Vendors.Add(new VendorViewModel
                {
                    Id = vendor.Id,
                    Name = vendor.Name,
                    PhoneNumber = vendor.PhoneNumber,
                    Email = vendor.Email,
                    Address = vendor.Address,
                    TotalPurchases = totalPurchases
                });
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
            await LoadVendors();
            return;
        }

        var query = SearchQuery.ToLower();
        var allVendors = await _databaseService.GetVendorsAsync();
        var filteredVendors = allVendors.Where(v =>
            v.Name.ToLower().Contains(query) ||
            v.PhoneNumber.Contains(query) ||
            v.Email.ToLower().Contains(query));

        Vendors.Clear();
        foreach (var vendor in filteredVendors)
        {
            var invoices = await _databaseService.GetVendorInvoicesAsync(vendor.Id);
            var totalPurchases = invoices.Sum(i => i.TotalAmount);

            Vendors.Add(new VendorViewModel
            {
                Id = vendor.Id,
                Name = vendor.Name,
                PhoneNumber = vendor.PhoneNumber,
                Email = vendor.Email,
                Address = vendor.Address,
                TotalPurchases = totalPurchases
            });
        }
    }

    [RelayCommand]
    private async Task AddVendor()
    {
        await Shell.Current.GoToAsync("vendordetail");
    }

    [RelayCommand]
    private async Task EditVendor(VendorViewModel vendor)
    {
        var parameters = new Dictionary<string, object>
        {
            { "vendor", vendor }
        };
        await Shell.Current.GoToAsync("vendordetail", parameters);
    }

    [RelayCommand]
    private async Task DeleteVendor(VendorViewModel vendor)
    {
        var delete = await Shell.Current.DisplayAlert(
            "Delete Vendor",
            $"Are you sure you want to delete {vendor.Name}?",
            "Yes", "No");

        if (!delete)
            return;

        var dbVendor = await _databaseService.GetVendorAsync(vendor.Id);
        if (dbVendor != null)
        {
            await _databaseService.DeleteVendorAsync(dbVendor);
            Vendors.Remove(vendor);
        }
    }

    [RelayCommand]
    private Task Refresh()
    {
        return LoadVendors();
    }
}

public class VendorViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public decimal TotalPurchases { get; set; }
}