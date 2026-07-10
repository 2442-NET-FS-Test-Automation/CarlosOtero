using HospitalApi.Models.Medical;
using HospitalApi.Models.Pharmacy;
using Microsoft.EntityFrameworkCore;

namespace HospitalApi.Data;

public class HospitalDbContext : DbContext
{
    public HospitalDbContext(DbContextOptions<HospitalDbContext> options) : base(options) { }

    public DbSet<Medication> Medications => Set<Medication>();
    public DbSet<InventoryItem> Inventory => Set<InventoryItem>();
    public DbSet<PrescriptionDetail> PrescriptionDetails => Set<PrescriptionDetail>();

    public DbSet<MedicalRecord> MedicalRecord => Set<MedicalRecord>();
    public DbSet<Patient> Patient => Set<Patient>();
    public DbSet<Appointment> Appointment => Set<Appointment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<InventoryItem>()
            .Property<byte[]>("RowVersion")
            .IsRowVersion()
            .IsConcurrencyToken();


        modelBuilder.Entity<InventoryItem>()
            .HasIndex(i => i.BatchNumber)
            .HasDatabaseName("IX_Inventory_BatchNumber");


        modelBuilder.Entity<Medication>().HasData(
            new Medication
            {
                MedicationID = 1,
                Name = "Metoprolol Succinate",
                GenericName = "Metoprolol",
                BrandName = "Toprol XL",
                DosageForm = "Tablet",
                Strength = "50mg",
                UnitPrice = 0.75m
            },
            new Medication
            {
                MedicationID = 2,
                Name = "Amoxicillin Trihydrate",
                GenericName = "Amoxicillin",
                BrandName = "Amoxil",
                DosageForm = "Capsule",
                Strength = "500mg",
                UnitPrice = 0.40m
            }
        );


        modelBuilder.Entity<InventoryItem>().HasData(
            new InventoryItem
            {
                InventoryID = 1,
                MedicationID = 1,
                BatchNumber = "BATCH-M12",
                StockQuantity = 500,
                ExpiryDate = new DateTime(2028, 12, 1),
                SupplierName = "PharmaCorp Inc"
            },
            new InventoryItem
            {
                InventoryID = 2,
                MedicationID = 2,
                BatchNumber = "BATCH-A99",
                StockQuantity = 1200,
                ExpiryDate = new DateTime(2027, 6, 15),
                SupplierName = "Global Meds Dist"
            }
        );

        modelBuilder.Entity<Medication>()
            .HasOne(m => m.Inventory)
            .WithOne(i => i.Medication)
            .HasForeignKey<InventoryItem>(i => i.MedicationID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Medication>()
            .HasMany(m => m.PrescriptionDetails)
            .WithOne(p => p.Medication)
            .HasForeignKey(p => p.MedicationId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<Patient>()
            .HasMany(p => p.Appointments)
            .WithOne(a => a.Patient)
            .HasForeignKey(a => a.PatientID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Patient>()
            .HasMany(p => p.MedicalRecords)
            .WithOne(m => m.Patient)
            .HasForeignKey(m => m.PatientID)
            .OnDelete(DeleteBehavior.Restrict);


    }
}
