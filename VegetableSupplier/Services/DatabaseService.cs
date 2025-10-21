using SQLite;
using System.Text.Json;
using VegetableSupplier.Models;

namespace VegetableSupplier.Services;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _database;

    public DatabaseService()
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "VegetableSupplier.db");
        _database = new SQLiteAsyncConnection(dbPath);
        InitializeDatabaseAsync().Wait();
    }

    private async Task InitializeDatabaseAsync()
    {
        // Create all tables
        await _database.CreateTableAsync<Vendor>();
        await _database.CreateTableAsync<VegetableEntity>();
        await _database.CreateTableAsync<VegetableUnit>();
        await _database.CreateTableAsync<Invoice>();
        await _database.CreateTableAsync<InvoiceItem>();
        await _database.CreateTableAsync<AuthUser>();

        // Initialize default data
        await InitializeVegetableUnits();
        await InitializeDefaultVegetables();
    }

    public async Task<List<VegetableUnit>> GetVegetableUnitsAsync()
    {
        return await _database.Table<VegetableUnit>().ToListAsync();
    }

    public async Task<List<Vegetable>> GetVegetablesAsync()
    {
        var entities = await _database.Table<VegetableEntity>().ToListAsync();
        return entities.Select(e => e.ToVegetable()).ToList();
    }

        // Initialize default data
        InitializeVegetableUnits().Wait();
        InitializeDefaultVegetables().Wait();
    }

    private async Task InitializeVegetableUnits()
    {
        if (await _database.Table<VegetableUnit>().CountAsync() > 0)
        {
            return;
        }

        var units = new List<VegetableUnit>
        {
            new() { CodeName = "kg", NameEn = "Kilogram", NameHi = "किलोग्राम", NameTe = "కిలోగ్రాము" },
            new() { CodeName = "bunch_small", NameEn = "Small Bunch", NameHi = "छोटा गट्ठा", NameTe = "చిన్న కట్ట" },
            new() { CodeName = "bunch_big", NameEn = "Big Bunch", NameHi = "बड़ा गट्ठा", NameTe = "పెద్ద కట్ట" },
            new() { CodeName = "box", NameEn = "Box", NameHi = "बॉक्स", NameTe = "పెట్టె" },
            new() { CodeName = "basket", NameEn = "Basket", NameHi = "टोकरी", NameTe = "బుట్ట" },
            new() { CodeName = "piece", NameEn = "Piece", NameHi = "नग", NameTe = "ఒక్కటి" },
            new() { CodeName = "dozen", NameEn = "Dozen", NameHi = "दर्जन", NameTe = "డజను" }
        };

        foreach (var unit in units)
        {
            await _database.InsertAsync(unit);
        }
    }

    public async Task SaveVegetableAsync(Vegetable vegetable)
    {
        vegetable.UnitPrices ??= new Dictionary<string, decimal>();
        var serializedPrices = JsonSerializer.Serialize(vegetable.UnitPrices);
        
        var dbVegetable = new VegetableEntity
        {
            Id = vegetable.Id,
            NameEn = vegetable.NameEn,
            NameHi = vegetable.NameHi,
            NameTe = vegetable.NameTe,
            UnitPricesJson = serializedPrices,
            UpdatedAt = vegetable.UpdatedAt
        };

        if (vegetable.Id == 0)
        {
            await _database.InsertAsync(dbVegetable);
        }
        else
        {
            await _database.UpdateAsync(dbVegetable);
        }
    }

    private async Task InitializeDefaultVegetables()
    {
        if (await _database.Table<VegetableEntity>().CountAsync() > 0)
        {
            return;
        }

        var vegetables = new List<Vegetable>
        {
            new() 
            { 
                NameEn = "Tomato",
                NameHi = "टमाटर",
                NameTe = "టమాటో",
                UnitPrices = new Dictionary<string, decimal> 
                { 
                    { "kg", 40.0m },
                    { "box", 400.0m }
                },
                UpdatedAt = DateTime.UtcNow
            },
            new() 
            { 
                NameEn = "Onion",
                NameHi = "प्याज",
                NameTe = "ఉల్లిపాయ",
                UnitPrices = new Dictionary<string, decimal> 
                { 
                    { "kg", 30.0m },
                    { "box", 300.0m }
                },
                UpdatedAt = DateTime.UtcNow
            },
            new() 
            { 
                NameEn = "Potato",
                NameHi = "आलू",
                NameTe = "బంగాళాదుంప",
                UnitPrices = new Dictionary<string, decimal> 
                { 
                    { "kg", 25.0m },
                    { "box", 250.0m }
                },
                UpdatedAt = DateTime.UtcNow
            }
        };

        foreach (var vegetable in vegetables)
        {
            await SaveVegetableAsync(vegetable);
        }
    }

    public async Task<Vegetable> GetVegetableByIdAsync(int id)
    {
        var entity = await _database.GetAsync<VegetableEntity>(id);
        return entity?.ToVegetable();
    }

    public async Task DeleteVegetableAsync(int id)
    {
        await _database.DeleteAsync<VegetableEntity>(id);
    }
            new Vegetable 
            { 
                NameEn = "Spinach",
                NameHi = "पालक",
                NameTe = "పాలకూర",
                UnitPrices = new Dictionary<string, decimal> 
                { 
                    { "bunch_small", 15.0m },
                    { "bunch_big", 25.0m }
                },
                UpdatedAt = DateTime.UtcNow
            },
            new Vegetable 
            { 
                NameEn = "Cauliflower",
                NameHi = "फूलगोभी",
                NameTe = "కాలిఫ్లవర్",
                UnitPrices = new Dictionary<string, decimal> 
                { 
                    { "piece", 30.0m },
                    { "dozen", 300.0m }
                },
                UpdatedAt = DateTime.UtcNow
            },
            new Vegetable 
            { 
                NameEn = "Carrot",
                NameHi = "गाजर",
                NameTe = "కారెట్",
                UnitPrices = new Dictionary<string, decimal> 
                { 
                    { "kg", 35.0m },
                    { "bunch_small", 20.0m }
                },
                UpdatedAt = DateTime.UtcNow
            },
            new Vegetable 
            { 
                NameEn = "Cucumber",
                NameHi = "खीरा",
                NameTe = "దోసకాయ",
                UnitPrices = new Dictionary<string, decimal> 
                { 
                    { "kg", 30.0m },
                    { "piece", 10.0m }
                },
                UpdatedAt = DateTime.UtcNow
            }
            // Add more vegetables as needed
        };

        await _database.InsertAllAsync(vegetables);
    }

    // Vendor operations
    public Task<List<Vendor>> GetVendorsAsync() =>
        _database.Table<Vendor>().ToListAsync();

    public Task<Vendor> GetVendorAsync(int id) =>
        _database.Table<Vendor>().Where(v => v.Id == id).FirstOrDefaultAsync();

    public Task<int> SaveVendorAsync(Vendor vendor)
    {
        if (vendor.Id == 0)
        {
            vendor.CreatedAt = DateTime.UtcNow;
            vendor.UpdatedAt = DateTime.UtcNow;
            return _database.InsertAsync(vendor);
        }
        
        vendor.UpdatedAt = DateTime.UtcNow;
        return _database.UpdateAsync(vendor);
    }

    public Task<int> DeleteVendorAsync(Vendor vendor) =>
        _database.DeleteAsync(vendor);

    // Vegetable operations
    public Task<List<Vegetable>> GetVegetablesAsync() =>
        _database.Table<Vegetable>().ToListAsync();

    public Task<int> SaveVegetableAsync(Vegetable vegetable)
    {
        vegetable.UpdatedAt = DateTime.UtcNow;
        if (vegetable.Id == 0)
            return _database.InsertAsync(vegetable);
        return _database.UpdateAsync(vegetable);
    }

    // Invoice operations
    public async Task<Invoice> CreateInvoiceAsync(Invoice invoice, List<InvoiceItem> items)
    {
        await _database.RunInTransactionAsync(async (conn) =>
        {
            invoice.CreatedAt = DateTime.UtcNow;
            await conn.InsertAsync(invoice);
            
            foreach (var item in items)
            {
                item.InvoiceId = invoice.Id;
                await conn.InsertAsync(item);
            }
        });
        return invoice;
    }

    public Task<List<Invoice>> GetVendorInvoicesAsync(int vendorId) =>
        _database.Table<Invoice>().Where(i => i.VendorId == vendorId).ToListAsync();

    public async Task<List<InvoiceItem>> GetInvoiceItemsAsync(int invoiceId) =>
        await _database.Table<InvoiceItem>().Where(i => i.InvoiceId == invoiceId).ToListAsync();

    // Auth operations
    public Task<AuthUser> GetUserAsync(string email) =>
        _database.Table<AuthUser>().Where(u => u.Email == email).FirstOrDefaultAsync();

    public Task<int> SaveUserAsync(AuthUser user) =>
        _database.InsertOrReplaceAsync(user);
}