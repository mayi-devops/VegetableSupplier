using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections.ObjectModel;
using VegetableSupplier.Models;
using VegetableSupplier.Services;

namespace VegetableSupplier.ViewModels;

public partial class CreateInvoicePageViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    private readonly GoogleService _googleService;
    
    [ObservableProperty]
    private ObservableCollection<VendorViewModel> vendors;
    
    [ObservableProperty]
    private VendorViewModel selectedVendor;
    
    [ObservableProperty]
    private ObservableCollection<Vegetable> vegetables;
    
    [ObservableProperty]
    private ObservableCollection<InvoiceItemViewModel> invoiceItems;
    
    [ObservableProperty]
    private decimal totalAmount;
    
    [ObservableProperty]
    private bool canShare;
    
    private string _currentInvoicePath;
    private string _driveFileId;

    public CreateInvoicePageViewModel(DatabaseService databaseService, GoogleService googleService)
    {
        _databaseService = databaseService;
        _googleService = googleService;
        
        Vendors = new ObservableCollection<VendorViewModel>();
        Vegetables = new ObservableCollection<Vegetable>();
        InvoiceItems = new ObservableCollection<InvoiceItemViewModel>();
        
        LoadData();
    }

    private async Task LoadData()
    {
        var vendorsList = await _databaseService.GetVendorsAsync();
        Vendors.Clear();
        foreach (var vendor in vendorsList)
        {
            Vendors.Add(new VendorViewModel
            {
                Id = vendor.Id,
                Name = vendor.Name,
                PhoneNumber = vendor.PhoneNumber,
                Email = vendor.Email,
                Address = vendor.Address
            });
        }

        var vegetablesList = await _databaseService.GetVegetablesAsync();
        Vegetables.Clear();
        foreach (var vegetable in vegetablesList)
        {
            Vegetables.Add(vegetable);
        }
    }

    [RelayCommand]
    private void AddItem()
    {
        InvoiceItems.Add(new InvoiceItemViewModel
        {
            Vegetables = Vegetables.ToList(),
            PropertyChanged = (s, e) =>
            {
                if (e.PropertyName == nameof(InvoiceItemViewModel.Total))
                {
                    CalculateTotal();
                }
            }
        });
    }

    private void CalculateTotal()
    {
        TotalAmount = InvoiceItems.Sum(i => i.Total);
    }

    [RelayCommand]
    private async Task GenerateInvoice()
    {
        if (SelectedVendor == null)
        {
            await Shell.Current.DisplayAlert("Error", "Please select a vendor", "OK");
            return;
        }

        if (!InvoiceItems.Any() || InvoiceItems.Any(i => i.Vegetable == null))
        {
            await Shell.Current.DisplayAlert("Error", "Please add items to the invoice", "OK");
            return;
        }

        try
        {
            var invoice = new Invoice
            {
                VendorId = SelectedVendor.Id,
                Date = DateTime.Now,
                TotalAmount = TotalAmount,
                InvoiceNumber = GenerateInvoiceNumber(),
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            var items = InvoiceItems.Select(i => new InvoiceItem
            {
                VegetableId = i.Vegetable.Id,
                Quantity = i.Quantity,
                Price = i.Vegetable.Price,
                Total = i.Total
            }).ToList();

            await _databaseService.CreateInvoiceAsync(invoice, items);

            // Generate PDF
            _currentInvoicePath = await GeneratePdfAsync(invoice, items);
            
            // Upload to Google Drive
            _driveFileId = await _googleService.UploadInvoicePdfAsync(
                _currentInvoicePath,
                $"Invoice_{invoice.InvoiceNumber}.pdf");

            // Update invoice with Drive file ID
            invoice.DriveFileId = _driveFileId;
            await _databaseService.UpdateInvoiceAsync(invoice);

            CanShare = true;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", "Failed to generate invoice", "OK");
        }
    }

    [RelayCommand]
    private async Task Share()
    {
        if (string.IsNullOrEmpty(_currentInvoicePath))
            return;

        await Share.RequestAsync(new ShareFileRequest
        {
            Title = "Share Invoice",
            File = new ShareFile(_currentInvoicePath)
        });
    }

    private string GenerateInvoiceNumber()
    {
        return $"INV{DateTime.Now:yyyyMMddHHmmss}";
    }

    private async Task<string> GeneratePdfAsync(Invoice invoice, List<InvoiceItem> items)
    {
        var filePath = Path.Combine(FileSystem.CacheDirectory, $"Invoice_{invoice.InvoiceNumber}.pdf");
        
        using var document = new Document(PageSize.A4, 50, 50, 25, 25);
        using var writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
        
        document.Open();

        // Add header
        var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
        var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
        var smallFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);

        document.Add(new Paragraph("INVOICE", titleFont) { Alignment = Element.ALIGN_CENTER });
        document.Add(new Paragraph($"Invoice #: {invoice.InvoiceNumber}", normalFont));
        document.Add(new Paragraph($"Date: {invoice.Date:d}", normalFont));
        document.Add(new Paragraph("\n"));

        // Add vendor details
        document.Add(new Paragraph("Bill To:", normalFont));
        document.Add(new Paragraph(SelectedVendor.Name, normalFont));
        document.Add(new Paragraph(SelectedVendor.Address, smallFont));
        document.Add(new Paragraph(SelectedVendor.PhoneNumber, smallFont));
        document.Add(new Paragraph("\n"));

        // Add items table
        var table = new PdfPTable(5)
        {
            WidthPercentage = 100,
            SpacingBefore = 10f,
            SpacingAfter = 10f
        };

        table.AddCell(new PdfPCell(new Phrase("Item", normalFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
        table.AddCell(new PdfPCell(new Phrase("Unit", normalFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
        table.AddCell(new PdfPCell(new Phrase("Quantity", normalFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
        table.AddCell(new PdfPCell(new Phrase("Price", normalFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
        table.AddCell(new PdfPCell(new Phrase("Total", normalFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });

        foreach (var item in InvoiceItems)
        {
            table.AddCell(new Phrase(item.Vegetable.Name, normalFont));
            table.AddCell(new Phrase(item.Vegetable.Unit, normalFont));
            table.AddCell(new Phrase(item.Quantity.ToString("N2"), normalFont));
            table.AddCell(new Phrase(item.Vegetable.Price.ToString("C"), normalFont));
            table.AddCell(new Phrase(item.Total.ToString("C"), normalFont));
        }

        document.Add(table);

        // Add total
        var totalParagraph = new Paragraph($"Total Amount: {TotalAmount:C}", titleFont)
        {
            Alignment = Element.ALIGN_RIGHT
        };
        document.Add(totalParagraph);

        document.Close();
        return filePath;
    }

    [RelayCommand]
    private async Task AddVendor()
    {
        await Shell.Current.GoToAsync("vendordetail");
    }
}

public class InvoiceItemViewModel : ObservableObject
{
    private Vegetable _vegetable;
    private decimal _quantity;
    private decimal _total;
    
    public List<Vegetable> Vegetables { get; set; }
    
    public Vegetable Vegetable
    {
        get => _vegetable;
        set
        {
            _vegetable = value;
            CalculateTotal();
        }
    }
    
    public decimal Quantity
    {
        get => _quantity;
        set
        {
            _quantity = value;
            CalculateTotal();
        }
    }
    
    public decimal Total
    {
        get => _total;
        set => SetProperty(ref _total, value);
    }

    private void CalculateTotal()
    {
        if (Vegetable != null && Quantity > 0)
        {
            Total = Quantity * Vegetable.Price;
        }
        else
        {
            Total = 0;
        }
    }
}