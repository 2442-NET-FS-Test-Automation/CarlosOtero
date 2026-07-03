
using Microsoft.EntityFrameworkCore;
using Hospital.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Options;
using Hospital.Data.Entities;

// This is my API program.cs
// No main. We can think of it as 2 sections
// registering things with the builder.
// And then configuring things on the app
// And at the very bottom that app object that represents our entire API calls its run method

// Builder area
var builder = WebApplication.CreateBuilder(args);

// the first thing that we need is to give our builder a connection string to our database
var conn_string = "Server=localhost,1433;Database=HospitalMinDb;User ID=sa;Password=Pass-123;Trust Server Certificate=True";

// Tell the builder to use our HospitalDbContext with the connection string above
// By registering our DbContext class (or even classes, technically you use one per database)
// we hand off the managing of creating and destroying these DbContext objects to ASP.NET's
// dependency injection container. Like spring beans if you're familiar.
builder.Services.AddDbContext<HospitalDbContext>(Options => Options.UseSqlServer(conn_string));

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
app.MapGet("/inventory", async (HospitalDbContext db) =>
{
    // We should probably await this
    return await db.Inventory.ToListAsync();
});

// Let's use LINQ - Language Integrated Query
// LINQ is a Hospital that just let's us query collections
// The logic actually flows from SQL DQL - You can use method OR sql query syntax
// You can even save the queries themselves as C# objects if you want to
app.MapGet("/inventory/by-value", (HospitalDbContext db) =>
{
    return db.Inventory.Include(i => i.Medication)
        .GroupBy(i => i.StockAmount >= 5 ? "well-stocked" : "low") // group by just like in SQL
        .Select(g => new { tier = g.Key, count = g.Count(), units = g.Sum(i => i.StockAmount) })
        .ToList();
});

// Any endpoints that start with "/peek/*" are diagnostic/demo
// We are going to use them to expose things like EF Core change tracking and other
// underlying behaviors for learning. A real app would have no reason to expose HTTP endpoints
// to outside users to make this stuff observable.

app.MapGet("/peek/tracking", (HospitalDbContext db) =>
{
    // Let's see the underlying EF Core change tracker
    var unchanged = db.Medication.First(); // Grab the first object. Read but not modified => Unchanged
    var modified = db.Medication.Skip(1).First(); // queried ... still unchanged as of  here.

    modified.UnitPrice += 1; // state => Modified

    // When we create a new object and call the dbset's .Add() method it's state is
    // "Added" - this has not actually hit the databse yet. but it's tracked to be added.
    db.Medication.Add(new Medication { Id = 5, Name = "New Medication", GenericName = "New Generic", BrandName = "New Brand", DosageForm = "Tablet", Strength = "100mg", UnitPrice = 1.00m });

    // This bit of code  is like the non-production demo bit
    // We  are accessing the HospitalDbContext object's change traker to pull info.
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
    var firstDb = scopeA.ServiceProvider.GetRequiredService<HospitalDbContext>();
    var secondDb = scopeA.ServiceProvider.GetRequiredService<HospitalDbContext>();

    // Each dBContext reads from the same database BUT they track changes independently
    // remember we gave Inventory entities a RowVersion . not just a property named RowVersion
    // but an actual OnModelCreation FluentAPI config for a RowVersion
    // Both of these sart with the same RowVersion value
    var firstInventory = firstDb.Inventory.First(i => i.Id == 1);
    var secondInventory = secondDb.Inventory.First(i => i.Id == 1);

    // Let's modify on AND save its changes, while just modifying the other
    firstInventory.StockAmount--; // Decrement => Modified
    firstDb.SaveChanges(); // Save changes is  what persists any created, deleted or modified objects
    // That row in the D now has a RowVersion of 2


    // Calling SaveChanges() above modifies the RowVersion value

    // This object, that should represent the exact same row in the DB now has a stale RowVersion
    // before EF tries to persis anny changes, it will check the RowVersion. It Won't match
    // and an exception will be thrown;
    secondInventory.StockAmount--; // Rowversion still 1 - doesn't match DB
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
        ((InventoryItem)entry.Entity).StockAmount =
            current!.GetValue<int>(nameof(InventoryItem.StockAmount)) - 1;

        secondDb.SaveChanges();
    }
    // I can send back specific codes via methods like .Ok() with messages inside
    // others include Problem(), NotFound(), etc
    return Results.Ok("Conflict caught, reloaded and retried.");
    
});


// My file always ends with app.Run() - minimal API or Controller API

app.Run();
