using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using VegetableSupplier.Models;
using VegetableSupplier.Services;

namespace VegetableSupplier.ViewModels;

public partial class DashboardPageViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    
    [ObservableProperty]
    private ObservableCollection<InvoiceViewModel> recentInvoices;
    
    [ObservableProperty]
    private bool isBusy;

    public DashboardPageViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
        RecentInvoices = new ObservableCollection<InvoiceViewModel>();
    }

    public async Task LoadDataAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            RecentInvoices.Clear();

            // Get all invoices and join with vendors
            var invoices = await _databaseService.GetRecentInvoicesAsync(10);
            foreach (var invoice in invoices)
            {
                var vendor = await _databaseService.GetVendorAsync(invoice.VendorId);
                RecentInvoices.Add(new InvoiceViewModel
                {
                    Id = invoice.Id,
                    VendorName = vendor?.Name ?? "Unknown",
                    TotalAmount = invoice.TotalAmount,
                    Date = invoice.Date,
                    Status = invoice.Status
                });
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CreateInvoice()
    {
        await Shell.Current.GoToAsync("createinvoice");
    }

    [RelayCommand]
    private async Task AddVendor()
    {
        await Shell.Current.GoToAsync("vendor");
    }

    [RelayCommand]
    private async Task ManageVegetables()
    {
        await Shell.Current.GoToAsync("vegetables");
    }
}

public class InvoiceViewModel
{
    public int Id { get; set; }
    public string VendorName { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; }
}