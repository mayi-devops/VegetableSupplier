using VegetableSupplier.Models;

namespace VegetableSupplier.Services;

public class CustomerService
{
    private readonly DatabaseService _database;

    public CustomerService(DatabaseService database)
    {
        _database = database;
    }

    public async Task<List<Customer>> GetCustomersAsync()
    {
        return await _database.Table<Customer>().ToListAsync();
    }

    public async Task<Customer> GetCustomerAsync(int id)
    {
        return await _database.Table<Customer>()
            .Where(c => c.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<int> SaveCustomerAsync(Customer customer)
    {
        if (customer.Id == 0)
        {
            customer.CreatedAt = DateTime.UtcNow;
            customer.UpdatedAt = DateTime.UtcNow;
            return await _database.InsertAsync(customer);
        }
        
        customer.UpdatedAt = DateTime.UtcNow;
        return await _database.UpdateAsync(customer);
    }

    public async Task<CustomerPricing> GetCustomerPricingAsync(int customerId, int vegetableId)
    {
        return await _database.Table<CustomerPricing>()
            .Where(p => p.CustomerId == customerId 
                    && p.VegetableId == vegetableId
                    && (p.ValidTo == null || p.ValidTo >= DateTime.UtcNow))
            .FirstOrDefaultAsync();
    }

    public async Task<List<CustomerOrder>> GetCustomerOrdersAsync(int customerId)
    {
        return await _database.Table<CustomerOrder>()
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<CustomerOrder> CreateOrderAsync(CustomerOrder order, List<OrderItem> items)
    {
        await _database.RunInTransactionAsync(async (conn) =>
        {
            // Generate order number
            order.OrderNumber = GenerateOrderNumber();
            order.OrderDate = DateTime.UtcNow;
            
            // Calculate totals
            decimal total = 0;
            foreach (var item in items)
            {
                var pricing = await GetCustomerPricingAsync(order.CustomerId, item.VegetableId);
                if (pricing != null && item.Quantity >= pricing.MinimumQuantity)
                {
                    item.UnitPrice = pricing.SpecialPrice;
                    item.DiscountPercentage = pricing.DiscountPercentage;
                }
                else
                {
                    var vegetable = await _database.GetVegetableAsync(item.VegetableId);
                    item.UnitPrice = vegetable.UnitPrices[item.Unit];
                    item.DiscountPercentage = 0;
                }

                item.Total = item.Quantity * item.UnitPrice;
                item.DiscountAmount = item.Total * (item.DiscountPercentage / 100);
                item.FinalTotal = item.Total - item.DiscountAmount;
                total += item.FinalTotal;
            }

            order.TotalAmount = total;
            await _database.InsertAsync(order);

            foreach (var item in items)
            {
                item.OrderId = order.Id;
                await _database.InsertAsync(item);
            }

            // Create stock transactions
            foreach (var item in items)
            {
                await CreateStockTransactionForOrder(item);
            }

            // Update customer credit if applicable
            if (order.PaymentStatus != "Paid")
            {
                var customer = await GetCustomerAsync(order.CustomerId);
                customer.CurrentCredit += total;
                await SaveCustomerAsync(customer);
            }

            // Schedule next order if recurring
            if (order.IsRecurring && !string.IsNullOrEmpty(order.RecurrencePattern))
            {
                order.NextOrderDate = CalculateNextOrderDate(order.OrderDate, order.RecurrencePattern);
                await _database.UpdateAsync(order);
            }
        });

        return order;
    }

    private async Task CreateStockTransactionForOrder(OrderItem item)
    {
        var transaction = new StockTransaction
        {
            InventoryItemId = item.VegetableId,
            TransactionType = "OUT",
            Quantity = item.Quantity,
            Unit = item.Unit,
            UnitPrice = item.UnitPrice,
            ReferenceNumber = $"ORD-{item.OrderId}",
            TransactionDate = DateTime.UtcNow,
            OrderId = item.OrderId
        };

        await _database.InsertAsync(transaction);
    }

    private string GenerateOrderNumber()
    {
        return $"ORD{DateTime.Now:yyyyMMddHHmmss}";
    }

    private DateTime CalculateNextOrderDate(DateTime currentDate, string pattern)
    {
        // Pattern format: "FREQ=DAILY|WEEKLY|MONTHLY;INTERVAL=n"
        var parts = pattern.Split(';');
        var freq = parts[0].Split('=')[1];
        var interval = int.Parse(parts[1].Split('=')[1]);

        return freq switch
        {
            "DAILY" => currentDate.AddDays(interval),
            "WEEKLY" => currentDate.AddDays(interval * 7),
            "MONTHLY" => currentDate.AddMonths(interval),
            _ => throw new ArgumentException("Invalid recurrence pattern")
        };
    }

    public async Task ProcessRecurringOrdersAsync()
    {
        var now = DateTime.UtcNow;
        var dueOrders = await _database.Table<CustomerOrder>()
            .Where(o => o.IsRecurring 
                    && o.NextOrderDate <= now
                    && o.Status != "Cancelled")
            .ToListAsync();

        foreach (var order in dueOrders)
        {
            // Get original order items
            var originalItems = await _database.Table<OrderItem>()
                .Where(i => i.OrderId == order.Id)
                .ToListAsync();

            // Create new order with same items
            var newOrder = new CustomerOrder
            {
                CustomerId = order.CustomerId,
                DeliveryAddress = order.DeliveryAddress,
                IsRecurring = true,
                RecurrencePattern = order.RecurrencePattern,
                PaymentMode = order.PaymentMode,
                Notes = order.Notes
            };

            var newItems = originalItems.Select(i => new OrderItem
            {
                VegetableId = i.VegetableId,
                Quantity = i.Quantity,
                Unit = i.Unit
            }).ToList();

            await CreateOrderAsync(newOrder, newItems);
        }
    }
}