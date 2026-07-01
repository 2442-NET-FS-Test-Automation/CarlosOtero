
using Microsoft.EntityFrameworkCore;
using Library.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Options;

// This is my API program.cs
// No main. We can think of it as 2 sections
// registering things with the builder.
// And then configuring things on the app
// And at the very bottom that app object that represents our entire API calls its run method

// Builder area
var builder = WebApplication.CreateBuilder(args);

// the first thing that we need is to give our builder a connection string to our database
var conn_string = "Server=localhost,1433;Database=LibraryMinimalDb;User ID=sa;Password=Pass-123;Trust Server Certificate=True";

// Tell the builder to use our LibraryDbContext with the connection string above
// By registering our DbContext class (or even classes, technically you use one per database)
// we hand off the managing of creating and destroying these DbContext objects to ASP.NET's
// dependency injection container. Like spring beans if you're familiar.
builder.Services.AddDbContext<LibraryDbContext>(Options => Options.UseSqlServer(conn_string));

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
app.MapGet("/inventory", async (LibraryDbContext db) => {
    // We should probably await this
    return await db.Inventory.ToListAsync();
});

// Let's use LINQ - Language Integrated Query
// LINQ is a library that just let's us query collections
// The logic actually flows from SQL DQL - You can use method OR sql query syntax
// You can even save the queries themselves as C# objects if you want to
app.MapGet("/inventory/by-value", (LibraryDbContext db) => {
    return db.Inventory.Include(i => i.Product)
        .GroupBy(i => i.CurrentStock >= 5 ? "well-stocked": "low") // group by just like in SQL
        .Select(g => new{ tier = g.Key, count = g.Count(), units = g.Sum(i => i.CurrentStock)})
        .ToList();
});

// My file always ends with app.Run() - minimal API or Controller API

app.Run();
