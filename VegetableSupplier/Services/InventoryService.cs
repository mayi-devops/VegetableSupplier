using VegetableSupplier.Models;

namespace VegetableSupplier.Services;

public class InventoryService
{
    private readonly DatabaseService _database;

    public InventoryService(DatabaseService database)
    {
        _database = database;
    }

    public async Task<List<InventoryItem>> GetInventoryAsync()
    {
        return await _database.Table<InventoryItem>().ToListAsync();
    }

    public async Task<InventoryItem> GetInventoryItemAsync(int id)
    {
        return await _database.Table<InventoryItem>()
            .Where(i => i.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<List<InventoryItem>> GetLowStockItemsAsync()
    {
        return await _database.Table<InventoryItem>()
            .Where(i => i.Quantity <= i.ReorderPoint)
            .ToListAsync();
    }

    public async Task<List<InventoryItem>> GetExpiringItemsAsync(int daysThreshold)
    {
        var thresholdDate = DateTime.UtcNow.AddDays(daysThreshold);
        return await _database.Table<InventoryItem>()
            .Where(i => i.ExpiryDate != null && i.ExpiryDate <= thresholdDate)
            .ToListAsync();
    }

    public async Task AddStockTransactionAsync(StockTransaction transaction)
    {
        await _database.RunInTransactionAsync(async (conn) =>
        {
            var item = await GetInventoryItemAsync(transaction.InventoryItemId);
            if (item == null)
                throw new Exception("Inventory item not found");

            // Update inventory quantity
            switch (transaction.TransactionType)
            {
                case "IN":
                    item.Quantity += transaction.Quantity;
                    item.LastRestocked = transaction.TransactionDate;
                    break;
                case "OUT":
                    if (item.Quantity < transaction.Quantity)
                        throw new Exception("Insufficient stock");
                    item.Quantity -= transaction.Quantity;
                    break;
                case "WASTE":
                    if (item.Quantity < transaction.Quantity)
                        throw new Exception("Invalid waste quantity");
                    item.Quantity -= transaction.Quantity;
                    item.WastageQuantity += transaction.Quantity;
                    break;
                case "TRANSFER":
                    if (item.Quantity < transaction.Quantity)
                        throw new Exception("Insufficient stock for transfer");
                    item.Quantity -= transaction.Quantity;
                    // Create or update destination inventory
                    var destItem = await GetInventoryItemByLocationAsync(
                        transaction.InventoryItemId,
                        transaction.DestinationLocation);
                    if (destItem == null)
                    {
                        destItem = new InventoryItem
                        {
                            VegetableId = item.VegetableId,
                            VegetableName = item.VegetableName,
                            Quantity = transaction.Quantity,
                            Unit = item.Unit,
                            MinimumStock = item.MinimumStock,
                            ReorderPoint = item.ReorderPoint,
                            LocationCode = transaction.DestinationLocation,
                            Status = item.Status,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        await _database.InsertAsync(destItem);
                    }
                    else
                    {
                        destItem.Quantity += transaction.Quantity;
                        destItem.UpdatedAt = DateTime.UtcNow;
                        await _database.UpdateAsync(destItem);
                    }
                    break;
            }

            item.UpdatedAt = DateTime.UtcNow;
            await _database.UpdateAsync(item);
            await _database.InsertAsync(transaction);
        });
    }

    public async Task<InventoryItem> GetInventoryItemByLocationAsync(int vegetableId, string locationCode)
    {
        return await _database.Table<InventoryItem>()
            .Where(i => i.VegetableId == vegetableId && i.LocationCode == locationCode)
            .FirstOrDefaultAsync();
    }

    public async Task UpdateInventoryItemAsync(InventoryItem item)
    {
        item.UpdatedAt = DateTime.UtcNow;
        await _database.UpdateAsync(item);
    }

    public async Task<List<StockTransaction>> GetTransactionHistoryAsync(
        int inventoryItemId, 
        DateTime startDate, 
        DateTime endDate)
    {
        return await _database.Table<StockTransaction>()
            .Where(t => t.InventoryItemId == inventoryItemId 
                    && t.TransactionDate >= startDate 
                    && t.TransactionDate <= endDate)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
    }
}