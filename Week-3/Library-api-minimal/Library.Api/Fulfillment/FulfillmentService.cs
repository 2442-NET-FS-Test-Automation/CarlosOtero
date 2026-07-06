// This class will hold the business logic/db retry logic for fulfilling transactions

using Library.Data;
using Library.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Library.Api.Fulfillment;

// ASP.NET's builder (DI container) NEEDS us to provide 2 things when we  register a service
// An interface and a concrete implementation. These can both go in the samme file.

public interface IFulfillmentService
{
    public Task<FulfillmentResult> FulfillOneAsync(int orderId, CancellationToken ct);
    public Task<BurstResult> FulfillBurstAsync(IEnumerable<int> orderIds, CancellationToken ct);
}

// I'm going to stick everything about order fulfillment in this file
// requests are either fulfilled or Backordered - no other results possible

public enum FulfillmentResult { Fulfilled, Backordered }

// Also going to make a record for the result of a Burst (many orders at the same time)
// records are lightweight custom types that allow for comparison with ==

public record BurstResult(int Fulfilled, int Backordered);

public class FulfillmentService : IFulfillmentService
{
    // ASP.NET manages the creating (and destruction) of all our dependencies across our app
    // If we need a DbContext or DbContextFactory or Logger or any other dependency
    // we DO NOT instatiate one here, we ask for one via the Constructor
    private readonly IDbContextFactory<LibraryDbContext> _factory; //holds my factory

    // The factory in the constructor arguments list comes from the ASP.NET DI Container
    public FulfillmentService(IDbContextFactory<LibraryDbContext> factory)
    {
        _factory = factory;
    }

    // This method is going to handle fulfillment - it's going to be a bit  long. Which is why we didn't
    // just write all of this in Program.cs

    public async Task<FulfillmentResult> FulfillOneAsync(int orderId, CancellationToken ct)
    {
        // First - we need a db context
        await using var db = await _factory.CreateDbContextAsync(ct);

        // Let's grab our order from the database
        // Flow for this - a customer places an order. It hits the order table - we are now fulfilling that order
        var order = await db.Order.Include(o => o.Lines).FirstAsync(o => o.Id == orderId, ct); // LINQ with Async

        // Let's create that dictionary with the productId key and the OrderIde value
        // yay for LINQ/Collections namespace
        var requested = order.Lines.ToDictionary(L => L.ProductId, L => L.OrderId);

        // creating a flag for "can i continue fulfilling this order"
        bool canFulfill = true;

        foreach (OrderLines line in order.Lines)
        {
            // First - grab the current inventory from the db for 
            InventoryItem inv = await db.Inventory.FirstAsync(i => i.ProductId == line.ProductId, ct);

            // Next - check if we can meet the order
            if (inv.CurrentStock < line.Quantity)
            {
                canFulfill = false;
                break;
            }
            inv.CurrentStock -= line.Quantity; // This writes to the InventoryItem table is guarded ny RowVersion
        }

        // assuming we broke out of the foreach and cannot fulfill the order
        if (!canFulfill) // chacking if canFulFill is false
        {
            // We can't fulfill this order, it's now Backordered
            order.Status = Status.Backordered;


            // Create a new fullfillment event record for this transaction, setting it to backordered.
            db.FulfillmentEvent.Add(new FulfillmentEvent { OrderId = orderId, Type = "Backorder" });

            await db.SaveChangesAsync();
            // Log the transaction, using Serilog structured Logging syntax
            Log.Warning("Backordered {OrderId}: insufficient stock", orderId);

            return FulfillmentResult.Backordered;
        }
        // If we make it here, we CAN fulfill that order
        order.Status = Status.Fulfilled;
        order.CompletedUtc = DateTime.UtcNow;
        db.FulfillmentEvent.Add(new FulfillmentEvent { OrderId = orderId, Type = "Fulfilled" });

        // Adding our retry save method
        if (!await SaveWithRetryAsync(db, requested, ct)) // If we enter this if - we lost enough times
        {
            // That stock dropped this order was backordered
            db.ChangeTracker.Clear(); // clear change tracker
            Order staleOrder = await db.Order.FirstAsync(o => o.Id == orderId, ct); // grab stale order from db
            staleOrder.Status = Status.Backordered; // set its status to backordered
            Log.Warning("Backordered order {OrderId} after concurrency retry", orderId);
            return FulfillmentResult.Backordered;
        }

        await db.SaveChangesAsync(ct);
        Log.Information("fulfilled order: {OrderId}, {LineCount} lines", orderId, order.Lines.Count);
        return FulfillmentResult.Fulfilled;
    }

    // Let's break the logic for saving with retry (via RowVersion) into its own method
    // just to help keep things straight
    private static async Task<bool> SaveWithRetryAsync(
        LibraryDbContext db, IReadOnlyDictionary<int, int> requestedByProductId, CancellationToken ct)
    {
        // This is that RowVersion Change Tracker entry retry from yesterday
        // Let's set max retries to 3 - by wrapping everything in a loop
        while(true)
        {
            // Our loop as written never exits - it does increment attempt for us
            // If we retry and fail x amount of times - we will throw an exception manually

            try
            {
                // The DbContext inside this method came from FulfillOneAsync - if it has changes staged to it
                // We can save them here. It's the same object.
                await db.SaveChangesAsync(ct);
                return true;
            }
            // We can tell our try catch how many times to handle this exception for us
            // After 3 attempts - we won't enter the catch. It bubbles up to wherever this method was called
            catch (DbUpdateConcurrencyException ex)
            {
                // Retry logic - remember that Change Tracker stuff?
                // entry is an EF Core Change Tracker entry
                foreach (var entry in ex.Entries)
                {
                    var current = await entry.GetDatabaseValuesAsync(); //Grab the current database values
                    // return false
                    if (current is null) return false;

                    // Set the OriginalValues bucket on the  entry to what they currently are
                    entry.OriginalValues.SetValues(current);

                    if (entry.Entity is InventoryItem inv)
                    {
                        // Grab the current total for that item's stock
                        int freshValue = current.GetValue<int>(nameof(InventoryItem.CurrentStock));
                        // Dictionary lookup against the dict we passed in
                        int desiredAmount = requestedByProductId[inv.ProductId];

                        // Re-check on the fresh stock - don't blidly trust it
                        if (freshValue < desiredAmount) return false; // This is now our exit condition
                        inv.CurrentStock = freshValue - desiredAmount;
                    }
                }
            }

        }

    }

    public async Task<BurstResult> FulfillBurstAsync(IEnumerable<int> orderIds, CancellationToken ct)
    {
        // We are just going to piggyback off of our FulfillOneAsync - no need to reqwrite logic we can just call it again
        var tasks = orderIds.Select(id => FulfillOneAsync(id, ct)); // Each call will get it's own dbContext

        // Await here until all tasks in the collection are complete
        var results = await Task.WhenAll(tasks);

        return new BurstResult(
            Fulfilled: results.Count(r => r == FulfillmentResult.Fulfilled),
            Backordered: results.Count(r => r == FulfillmentResult.Backordered)
        );
    }
}