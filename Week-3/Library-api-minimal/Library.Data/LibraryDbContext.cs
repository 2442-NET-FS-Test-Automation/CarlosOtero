using Microsoft.EntityFrameworkCore;
using Library.Data.Entities;
using Azure;
using System.Dynamic;

namespace Library.Data;

// All of the code that does the actual SQL generation, creating a connection to my database,
// doing CRUD, updating the DB based on changes to my models - ALL OF THAT lives in class
// called DbContext. I don't want to modify that class. It comes in from EF Core itself. What I do
// is create a file with a class that INHERITS from it.

public class LibraryDbContext : DbContext
{
    // This class needs a constructor, and it needs to take a certain argument
    // We ourselves will never call this constructor. ASP.NET's DI Container will do it for us
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

    // We need to tell out DbContext what C# classes we are tracking as Entities
    // Reminder - these Entities become our tables
    public DbSet<Product> Products => Set<Product>();
    public DbSet<InventoryItem> Inventory => Set<InventoryItem>();

    public DbSet<Customer> Customer => Set<Customer>();
    public DbSet<Order> Order => Set<Order>();
    public DbSet<OrderLines> OrderLines => Set<OrderLines>();
    // Both ending syntax work just the same
    public DbSet<FulfillmentEvent> FulfillmentEvent {get;set;}

    // If I  want to do things like deeper configurations options or data seeding
    // I can override a method we inherited from DbContext
    // called OnModelCreating() - this is called when EF Core creates a migration
    protected override void OnModelCreating(ModelBuilder b)
    {
        // I can set anything I want as far as constraints, mapping column names and types
        // Inside of here using something called Fluent API. EF Core Lets you do config
        // in 3 ways. Convention < Data Annotations < FluentAPI in OnModelCreating

        // For example here is the same config we did by convention and annotation prior
        b.Entity<Product>(e =>
        {
            // Let's set an index while we're here, the one new thing to make this worth it
            e.HasIndex(p => p.Sku).IsUnique(); // Non key index based on sku's being unique

            // Setting the decimal places on Price
            e.Property(p => p.Price).HasColumnType("decimal(10,2)");

            // Setting the relationship
            e.HasOne(p => p.Inventory)
                .WithOne(i => i.Product)
                .HasForeignKey<InventoryItem>(i => i.ProductId);
        });

        // Setting our RowVersion property as an EF Core Row Version
        b.Entity<InventoryItem>().Property(i=>i.RowVersion).IsRowVersion();

        //This order of operations, setting string Length and  then telling DB that a column
        // is unique is specific to strings + SQL Server.
        b.Entity<Customer>().Property(c=>c.Email).HasMaxLength(256); //Setting Length of email first...
        b.Entity<Customer>().HasIndex(c => c.Email).IsUnique();

        // After you configure your entities (if you do any config in the override)
        // we can use OnModelCreating to seed data
        b.Entity<Product>().HasData(
            new Product { Id = 1, Sku = "BK-001", Name = "Clean Code", Price = 32.00m },
            new Product { Id = 2, Sku = "BK-002", Name = "The Pragmatic Programmer", Price = 38.00m },
            new Product { Id = 3, Sku = "BK-003", Name = "Refactoring", Price = 45.00m }
        );

        b.Entity<InventoryItem>().HasData(
            new InventoryItem {Id = 1, ProductId = 1, CurrentStock = 5},
            new InventoryItem {Id = 2, ProductId = 2, CurrentStock = 3},
            new InventoryItem {Id = 3, ProductId = 3, CurrentStock = 8}
        );

        // HasData runs inside the migration BEFORE SQL Server can hand out identity keys
        // Which is why we give explicit PK's when seeding
        b.Entity<Customer>().HasData(
            new Customer { Id = 1, Name = "Ada Lovelace", Email = "ada@example.com"},
            new Customer { Id = 2, Name = "Alan Turing", Email = "alan@example.com"}
        );
    }
}