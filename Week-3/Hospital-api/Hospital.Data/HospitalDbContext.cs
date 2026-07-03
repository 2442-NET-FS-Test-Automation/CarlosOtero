using Microsoft.EntityFrameworkCore;
using Hospital.Data.Entities;

namespace Hospital.Data;

// All of the code that does the actual SQL generation, creating a connection to my database,
// doing CRUD, updating the DB based on changes to my models - ALL OF THAT lives in class
// called DbContext. I don't want to modify that class. It comes in from EF Core itself. What I do
// is create a file with a class that INHERITS from it.

public class HospitalDbContext : DbContext
{
    // This class needs a constructor, and it needs to take a certain argument
    // We ourselves will never call this constructor. ASP.NET's DI Container will do it for us
    public HospitalDbContext(DbContextOptions<HospitalDbContext> options) : base(options) { }

    // We need to tell out DbContext what C# classes we are tracking as Entities
    // Reminder - these Entities become our tables
    public DbSet<Medication> Medication => Set<Medication>();
    public DbSet<InventoryItem> Inventory => Set<InventoryItem>();

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Order> Order => Set<Order>();
    public DbSet<OrderLines> OrderLines => Set<OrderLines>();
    // Both ending syntax work just the same
    public DbSet<FulfillmentEvent> FulfillmentEvent { get; set; }

    // If I  want to do things like deeper configurations options or data seeding
    // I can override a method we inherited from DbContext
    // called OnModelCreating() - this is called when EF Core creates a migration
    protected override void OnModelCreating(ModelBuilder b)
    {
        // I can set anything I want as far as constraints, mapping column names and types
        // Inside of here using something called Fluent API. EF Core Lets you do config
        // in 3 ways. Convention < Data Annotations < FluentAPI in OnModelCreating

        // For example here is the same config we did by convention and annotation prior
        b.Entity<Medication>(e =>
        {
            // Let's set an index while we're here, the one new thing to make this worth it

            // Setting the decimal places on Price
            e.Property(p => p.UnitPrice).HasColumnType("decimal(10,2)");

            // Setting the relationship
            e.HasOne(p => p.Inventory)
                .WithOne(i => i.Medication)
                .HasForeignKey<InventoryItem>(i => i.MedicationId);
        });

        // Setting our RowVersion property as an EF Core Row Version
        b.Entity<InventoryItem>().Property(i => i.RowVersion).IsRowVersion();

        //This order of operations, setting string Length and  then telling DB that a column
        // is unique is specific to strings + SQL Server.
        b.Entity<Patient>().Property(p => p.Email).HasMaxLength(256); //Setting Length of email first...
        b.Entity<Patient>().HasIndex(p => p.Email).IsUnique();

        // After you configure your entities (if you do any config in the override)
        // we can use OnModelCreating to seed data
        b.Entity<Medication>().HasData(
           new Medication { Id = 1, Name = "Metoprolol Succinate", GenericName = "Metoprolol", BrandName = "Toprol XL", DosageForm = "Tablet", Strength = "50mg", UnitPrice = 0.75m },
            new Medication { Id = 2, Name = "Amoxicillin Trihydrate", GenericName = "Amoxicillin", BrandName = "Amoxil", DosageForm = "Capsule", Strength = "500mg", UnitPrice = 0.40m },
         new Medication { Id = 3, Name = "Ibuprofen", GenericName = "Ibuprofen", BrandName = "Advil", DosageForm = "Tablet", Strength = "400mg", UnitPrice = 0.15m },
        new Medication { Id = 4, Name = "Lipitor", GenericName = "Atorvastatin", BrandName = "Lipitor", DosageForm = "Tablet", Strength = "20mg", UnitPrice = 1.25m }

        );

        b.Entity<InventoryItem>().HasData(
            new InventoryItem { Id = 1, MedicationId = 1, StockAmount = 5, BatchNumber = "BATCH-M12", ExpirationDate = new DateTime(2028, 12, 1), Supplier = "PharmaCorp Inc" },
            new InventoryItem { Id = 2, MedicationId = 2, StockAmount = 3, BatchNumber = "BATCH-A99", ExpirationDate = new DateTime(2027, 6, 15), Supplier = "Global Meds Dist" },
            new InventoryItem { Id = 3, MedicationId = 3, StockAmount = 8, BatchNumber = "BATCH-I44", ExpirationDate = new DateTime(2029, 1, 20), Supplier = "PharmaCorp Inc" }
        );

        // HasData runs inside the migration BEFORE SQL Server can hand out identity keys
        // Which is why we give explicit PK's when seeding
        b.Entity<Patient>().HasData(
            new Patient { Id = 1, FirstName = "Ada", LastName = "Lovelace", DateOfBirth = new DateTime(1815, 12, 10), Insurance = "Blue Cross", Email = "ada@example.com", ContactNumber = "555-1234", EmergencyContactName = "John Doe", EmergencyContactNumber = "555-5678", BloodType = "O-", Address = "123 Main St", Allergies = "Penicillin" },
            new Patient { Id = 2, FirstName = "Alan", LastName = "Turing", DateOfBirth = new DateTime(1912, 6, 23), Insurance = "Cigna", Email = "alan@example.com", ContactNumber = "555-9012", EmergencyContactName = "Jane Smith", EmergencyContactNumber = "555-3456", BloodType = "A+", Address = "456 Oak Ave", Allergies = "Shellfish" }
        );
    }
}