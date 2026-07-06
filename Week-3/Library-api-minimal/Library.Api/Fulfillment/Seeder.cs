using System.Reflection.PortableExecutable;
using Library.Data;
using Library.Data.Entities;

using Microsoft.EntityFrameworkCore;

// In "production" our order would come from users. These API's run locally
// so we could either - create a post for a single order and run a shell script or something
// or we could create a seeding endpoint from here to generate some order for us.

public interface ISeeder
{
    IReadOnlyList<int> SeedOrders(int n, bool espedited);
    IReadOnlyList<int> ResetAndCreateOrders(int n);
}

public class Seeder : ISeeder
{

    // Going ahead and hardcoding some item SKUs (barcode numbers essentially in a list)

    private static readonly string[] Skus = { "BK-001", "BK-002", "BK-003" };
    private readonly IDbContextFactory<LibraryDbContext> _factory;
    public Seeder(IDbContextFactory<LibraryDbContext> factory)
    {
        _factory = factory;
    }
    public IReadOnlyList<int> SeedOrders(int n, bool expedited)
    {
        // Ask for  a db context
        using var db = _factory.CreateDbContext();

        // Create a dictionary based on our product table (the IDs in the DB) and the sku's
        var pid = db.Products.ToDictionary(p => p.Sku, p => p.Id); // SKU key, productId value

        var ids = new List<int>(n);

        // Based on n (number of order the user want to seed)
        // let's use a for Loop to create those orders programatically

        for (int i = 0; i < n; i++)
        {
            var order = new Order
            {
                CustomerId = Random.Shared.Next(1, 3), // Random number - bounded
                Priority = expedited ? Priority.Expedited : Priority.Normal,
                Lines = { new OrderLines { ProductId = pid[Skus[i % Skus.Length]], Quantity = 1 } }
            };

            db.Order.Add(order); // Add - stage changes in EF Core change tracker
            db.SaveChanges();
            ids.Add(order.Id);
        }
        return ids;
    }


    public IReadOnlyList<int> ResetAndCreateOrders(int n)
    {
        using var db = _factory.CreateDbContext();

        foreach (InventoryItem inv in db.Inventory)
        {
            switch (inv.ProductId)
            {
                case 1:
                    inv.CurrentStock = 5;
                    break;
                case 2:
                    inv.CurrentStock = 3;
                    break;
                case 3:
                    inv.CurrentStock = 8;
                    break;
                default:
                    break;
            }
        }
        db.SaveChanges(); // Saving changes after reset

        // Similar logic to the burst - just creating mixed orders this time
        var pid = db.Products.ToDictionary(p => p.Sku, p => p.Id);

        var ids = new List<int>(n);

        for (var i = 0; i < n; i++)
        {
            var order = new Order
            {
                CustomerId = Random.Shared.Next(1, 3),
                Priority = i % 3 == 0 ? Priority.Expedited : Priority.Normal,
                // YOU can seed this however you like - for demo we're doing modulo checks
                Lines = { new OrderLines { ProductId = pid[new[] { "BK-001", "BK-002", "BK-003" }[i % 3]], Quantity = 1 } }
            };

            db.Order.Add(order);
            db.SaveChanges();
            ids.Add(order.Id);
        }
        return ids;
    }
}