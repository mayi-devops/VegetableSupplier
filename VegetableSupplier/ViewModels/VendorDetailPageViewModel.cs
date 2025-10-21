using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using VegetableSupplier.Models;
using VegetableSupplier.Services;

namespace VegetableSupplier.ViewModels;

[QueryProperty(nameof(Vendor), "vendor")]
public partial class VendorDetailPageViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    
    [ObservableProperty]
    private string title;
    
    [ObservableProperty]
    private string name;
    
    [ObservableProperty]
    private string phoneNumber;
    
    [ObservableProperty]
    private string email;
    
    [ObservableProperty]
    private string address;
    
    [ObservableProperty]
    private bool isExistingVendor;
    
    [ObservableProperty]
    private ObservableCollection<InvoiceViewModel> purchaseHistory;

    private int _vendorId;

    public VendorDetailPageViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
        PurchaseHistory = new ObservableCollection<InvoiceViewModel>();
        Title = AppResources.AddVendor;
    }

    public VendorViewModel Vendor
    {
        set
        {
            if (value != null)
            {
                _vendorId = value.Id;
                Name = value.Name;
                PhoneNumber = value.PhoneNumber;
                Email = value.Email;
                Address = value.Address;
                IsExistingVendor = true;
                Title = AppResources.EditVendor;
                LoadPurchaseHistory();
            }
        }
    }

    private async Task LoadPurchaseHistory()
    {
        if (_vendorId == 0)
            return;

        var invoices = await _databaseService.GetVendorInvoicesAsync(_vendorId);
        PurchaseHistory.Clear();
        foreach (var invoice in invoices)
        {
            PurchaseHistory.Add(new InvoiceViewModel
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                TotalAmount = invoice.TotalAmount,
                Date = invoice.Date,
                Status = invoice.Status
            });
        }
    }

    [RelayCommand]
    private async Task Save()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            await Shell.Current.DisplayAlert("Error", "Vendor name is required", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(PhoneNumber))
        {
            await Shell.Current.DisplayAlert("Error", "Phone number is required", "OK");
            return;
        }

        var vendor = new Vendor
        {
            Id = _vendorId,
            Name = Name,
            PhoneNumber = PhoneNumber,
            Email = Email ?? string.Empty,
            Address = Address ?? string.Empty
        };

        await _databaseService.SaveVendorAsync(vendor);
        await Shell.Current.GoToAsync("..");
    }
}