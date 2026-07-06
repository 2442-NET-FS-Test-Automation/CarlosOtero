
using Microsoft.EntityFrameworkCore;
using Library.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Options;
using Library.Data.Entities;
using Serilog;
using Microsoft.AspNetCore.Components.Infrastructure;
using Library.Api.Fulfillment;

// This is my API program.cs
// No main. We can think of it as 2 sections
// registering things with the builder.
// And then configuring things on the app
// And at the very bottom that app object that represents our entire API calls its run method

// Builder area
var builder = WebApplication.CreateBuilder(args);

// the first thing that we need is to give our builder a connection string to our database
var conn_string = "Server=localhost,1433;Database=LibraryMinimalDb;User ID=sa;Password=Pass-123;Trust Server Certificate=True";

Log.Logger = new LoggerConfiguration()
.WriteTo.Console() // Write to console, and write to a file - starting a new file each day
.WriteTo.File("logs/fulfillment-log-.txt", rollingInterval: RollingInterval.Day)
.CreateLogger();

builder.Host.UseSerilog(); // tell the builder to use Serilog for logging

// Tell the builder to use our LibraryDbContext with the connection string above
// By registering our DbContext class (or even classes, technically you use one per database)
// we hand off the managing of creating and destroying these DbContext objects to ASP.NET's
// dependency injection container. Like spring beans if you're familiar.

// ASP.NET has few different scope types.
// Transient - a new instance is created every time it's requested.
// Scoped - a new instance per HTTP request
// Singleton - A single instane for the entire runtime of the app
builder.Services.AddDbContext<LibraryDbContext>(Options => Options.UseSqlServer(conn_string),
ServiceLifetime.Scoped, ServiceLifetime.Singleton); // Scoped is the default, but we can be explicit - and allow for SingletonScope when needed

// we know we will need more than one LibraryDbContext in one or more of these methods. But we don't know how many
// before runtime. So we can use a DbContextFactory to create as many as we need at runtime.
builder.Services.AddDbContextFactory<LibraryDbContext>(options => options.UseSqlServer(conn_string));

builder.Services.AddScoped<IFulfillmentService, FulfillmentService>();
builder.Services.AddScoped<ISeeder, Seeder>();
// Swagger stuff added to builder
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// App area
var app = builder.Build();

// Swagger stuff added to app
app.UseSwagger();
app.UseSwaggerUI();


// Endpoint area
app.MapGet("/", () => "Hello World!");

// Get all items from the inventory
app.MapGet("/inventory", async (LibraryDbContext db) =>
{
    // We should probably await this
    return await db.Inventory.ToListAsync();
});

// Let's use LINQ - Language Integrated Query
// LINQ is a library that just let's us query collections
// The logic actually flows from SQL DQL - You can use method OR sql query syntax
// You can even save the queries themselves as C# objects if you want to
app.MapGet("/inventory/by-value", (LibraryDbContext db) =>
{
    return db.Inventory.Include(i => i.Product)
        .GroupBy(i => i.CurrentStock >= 5 ? "well-stocked" : "low") // group by just like in SQL
        .Select(g => new { tier = g.Key, count = g.Count(), units = g.Sum(i => i.CurrentStock) })
        .ToList();
});

// Any endpoints that start with "/peek/*" are diagnostic/demo
// We are going to use them to expose things like EF Core change tracking and other
// underlying behaviors for learning. A real app would have no reason to expose HTTP endpoints
// to outside users to make this stuff observable.

app.MapGet("/peek/tracking", (LibraryDbContext db) =>
{
    // Let's see the underlying EF Core change tracker
    var unchanged = db.Products.First(); // Grab the first object. Read but not modified => Unchanged
    var modified = db.Products.Skip(1).First(); // queried ... still unchanged as of  here.

    modified.Price += 1; // state => Modified

    // When we create a new object and call the dbset's .Add() method it's state is
    // "Added" - this has not actually hit the databse yet. but it's tracked to be added.
    db.Products.Add(new Product { Sku = "BK-TMP", Name = "Tmp", Price = 1m });

    // This bit of code  is like the non-production demo bit
    // We  are accessing the LibraryDbContext object's change traker to pull info.
    // At most you'd debug with this.
    var states = db.ChangeTracker.Entries()
    .Select(e => new { entity = e.Entity.GetType().Name, state = e.State.ToString() }).ToList();

    // Clearing the change tracker manually
    db.ChangeTracker.Clear();

    return states;
});

// Lets manually go out of our way to create a conflict - obviously don't do this in a real app
app.MapGet("peek/conflict", (IServiceScopeFactory scopes) =>
{
    // Manually asking for  scopes. Normallu each endpoint method call gets its own scope tracked
    // by ASP.NET under the hood during runtime. We can, for various reasons good and bad do this manually
    using var scopeA = scopes.CreateScope();
    using var scopeB = scopes.CreateScope();

    // Now, remember that a dbContext is generated per scope, so we have to do that too
    var firstDb = scopeA.ServiceProvider.GetRequiredService<LibraryDbContext>();
    var secondDb = scopeA.ServiceProvider.GetRequiredService<LibraryDbContext>();

    // Each dBContext reads from the same database BUT they track changes independently
    // remember we gave Inventory entities a RowVersion . not just a property named RowVersion
    // but an actual OnModelCreation FluentAPI config for a RowVersion
    // Both of these sart with the same RowVersion value
    var firstInventory = firstDb.Inventory.First(i => i.Id == 1);
    var secondInventory = secondDb.Inventory.First(i => i.Id == 1);

    // Let's modify on AND save its changes, while just modifying the other
    firstInventory.CurrentStock--; // Decrement => Modified
    firstDb.SaveChanges(); // Save changes is  what persists any created, deleted or modified objects
    // That row in the D now has a RowVersion of 2


    // Calling SaveChanges() above modifies the RowVersion value

    // This object, that should represent the exact same row in the DB now has a stale RowVersion
    // before EF tries to persis anny changes, it will check the RowVersion. It Won't match
    // and an exception will be thrown;
    secondInventory.CurrentStock--; // Rowversion still 1 - doesn't match DB
    try
    {
        secondDb.SaveChanges(); // This should fail as RowVersions don't match
    }
    catch (DbUpdateConcurrencyException ex)
    {
        // In this case we want EF to retry with the UPDATE
        // Asking for the actual ChangeTracker entry that  threw the exception
        // this is EF Core specific.
        var entry = ex.Entries.Single();

        // For the entry that threw the exception - grap it's current values from the DB
        // not the object, just the values
        var current = entry.GetDatabaseValues();

        // Every entry  in the change tracker tracks two set of values.
        // OriginalValues = the values of the object when it was Loaded from the db
        // CurrentValues = the new modified values we changed on the object in our app
        // Here we manually set the OriginalValues set to the values from the DB we JUST grabbed
        entry.OriginalValues.SetValues(current!);

        // Using the entry to grab the actual item - going somewhat backwards
        ((InventoryItem)entry.Entity).CurrentStock =
            current!.GetValue<int>(nameof(InventoryItem.CurrentStock)) - 1;

        secondDb.SaveChanges();
    }
    // I can send back specific codes via methods like .Ok() with messages inside
    // others include Problem(), NotFound(), etc
    return Results.Ok("Conflict caught, reloaded and retried.");

});

// endpoint to reset the stock of the items in my catalog - useful for testing and demo
// might need to hit this endpoint  while we work
app.MapPost("/inventory/rest", (LibraryDbContext db, ILogger<Program> Logger) =>
{
    // We just ask for an Ilogger like we do our dbcontext
    // then use it as normal
    Logger.LogInformation("Started seeing database");

    // What  I want to do is reset the  items that I  know I stuck into the db
    foreach (InventoryItem inv in db.Inventory) // for each item in mu db inventory table... do something
    {
        // I only want to do something if the  primary key is 1, 2, or 3
        switch (inv.Id)
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

    db.SaveChanges(); // Persisting to db
    Logger.LogInformation("Stock reset");
    return Results.Ok("stock reset");
}
);

// Fulfillment stuff for order goes down here
// I'm going to take in info from the front end (swagger for now)
// I have a few options
// I can take in from the uri/query string
// I can also take in parameters from the body

app.MapPost("/orders", async (OrderPaylod orderRequest, IDbContextFactory<LibraryDbContext> factory, CancellationToken ct, IFulfillmentService fSvc) =>
{
    // Remember we create an order in our db
    // Then try to create a Successful fulfillment record against the db
    await using var db = await factory.CreateDbContextAsync(ct); // Ask for db context to place order
    var newOrder = new Order
    {
        CustomerId = orderRequest.CustomerId,
        Priority = Priority.Normal,
        // Using the orderRequest from the HTTP request body to create my order
        Lines = { new OrderLines { ProductId = orderRequest.ProductId, Quantity = orderRequest.Quantity } }
    };

    db.Order.Add(newOrder); // Add new order
    await db.SaveChangesAsync(ct); // Save that order to db

    // Now that we've added the order - we try to fulfill it
    var result = await fSvc.FulfillOneAsync(newOrder.Id, ct); // newOrder is now in the db, we can add for it
    return Results.Ok(new { orderId = newOrder.Id, fulfillmentResult = result.ToString() });
});

// Burst endpoint
// Forgoing creating a record - we will take these from the query string
// IHostApplicationlifetime - this let's us see events related to the app lifetime
// we are going to use it to make sure we "flush" pending orders if the app is asked to stop
app.MapPost("/orders/burst", (int n, bool expedited, ISeeder seeder,
IServiceScopeFactory scopes, IHostApplicationLifetime lifetime) =>
{
    var ids = seeder.SeedOrders(n, expedited); //calling the seed orders method with the stuff from front end
    var appStopping = lifetime.ApplicationStopping; // gives us a cancellation token that is called when app goes to shutdown

    _ = Task.Run(async () =>
    {
        try
        {
            using var scope = scopes.CreateScope(); // ask for a fresh scope
            var service = scope.ServiceProvider.GetRequiredService<IFulfillmentService>(); // Grab a fulfillment service
            await service.FulfillBurstAsync(ids, appStopping); // use it to call fulfillBurstAsync()
        }
        catch (Exception ex)
        {
            // This task is fire and forget because we aren't waiting or storing its result
            // any exceptions would be "swallowed" i.e. they would die with the task in the background
            Log.Error(ex, "Burst fulfillment failed");
        }
    }, appStopping);
});

app.MapGet("/verify/no-oversell", (LibraryDbContext db) =>
{
    var rows = db.Inventory.Include(i => i.Product).ToList(); // Grab inventory rows, include the product objects as well
    var negative = rows.Where(i => i.CurrentStock < 0).ToList(); // Grab items with negative stock
    var fulfilled = db.FulfillmentEvent.Count(e => e.Type == "Fulfilled"); // count the fulfilled orders

    return new
    {
        anyNegative = negative.Any(),
        onHand = rows.Select(i => new { i.ProductId, i.CurrentStock }),
        unitsFulfilled = fulfilled
    };
});

// My file always ends with app.Run() - minimal API or Controller API

app.Run();
Log.CloseAndFlush();
public record OrderPaylod(int ProductId, int Quantity, int CustomerId);
