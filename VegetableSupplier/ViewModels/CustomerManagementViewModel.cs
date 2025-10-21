using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using VegetableSupplier.Models;
using VegetableSupplier.Services;

namespace VegetableSupplier.ViewModels;

public partial class CustomerManagementViewModel : ObservableObject
{
    private readonly CustomerService _customerService;
    
    [ObservableProperty]
    private ObservableCollection<Customer> customers;
    
    [ObservableProperty]
    private bool isLoading;
    
    [ObservableProperty]
    private string searchQuery;
    
    [ObservableProperty]
    private string selectedType;

    public ObservableCollection<string> CustomerTypes { get; } = new()
    {
        "All",
        "Regular",
        "Wholesale",
        "Retail"
    };

    public CustomerManagementViewModel(CustomerService customerService)
    {
        _customerService = customerService;
        Customers = new ObservableCollection<Customer>();
        SelectedType = "All";
    }

    [RelayCommand]
    private async Task LoadCustomers()
    {
        if (IsLoading) return;

        try
        {
            IsLoading = true;

            var allCustomers = await _customerService.GetCustomersAsync();

            // Filter by type if selected
            if (SelectedType != "All")
                allCustomers = allCustomers.Where(c => c.CustomerType == SelectedType).ToList();

            // Filter by search query if any
            if (!string.IsNullOrEmpty(SearchQuery))
            {
                var query = SearchQuery.ToLower();
                allCustomers = allCustomers.Where(c => 
                    c.Name.ToLower().Contains(query) ||
                    c.PhoneNumber.Contains(query) ||
                    c.Email.ToLower().Contains(query)
                ).ToList();
            }

            Customers.Clear();
            foreach (var customer in allCustomers)
                Customers.Add(customer);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task AddCustomer()
    {
        await Shell.Current.GoToAsync("customeredit");
    }

    [RelayCommand]
    private async Task EditCustomer(Customer customer)
    {
        var parameters = new Dictionary<string, object>
        {
            { "customer", customer }
        };
        await Shell.Current.GoToAsync("customeredit", parameters);
    }

    [RelayCommand]
    private async Task ViewOrders(Customer customer)
    {
        var parameters = new Dictionary<string, object>
        {
            { "customer", customer }
        };
        await Shell.Current.GoToAsync("customerorders", parameters);
    }

    [RelayCommand]
    private async Task SetSpecialPricing(Customer customer)
    {
        var parameters = new Dictionary<string, object>
        {
            { "customer", customer }
        };
        await Shell.Current.GoToAsync("customerpricing", parameters);
    }

    [RelayCommand]
    private async Task CreateOrder(Customer customer)
    {
        var parameters = new Dictionary<string, object>
        {
            { "customer", customer }
        };
        await Shell.Current.GoToAsync("createorder", parameters);
    }

    partial void OnSearchQueryChanged(string value)
    {
        LoadCustomers().ConfigureAwait(false);
    }

    partial void OnSelectedTypeChanged(string value)
    {
        LoadCustomers().ConfigureAwait(false);
    }
}