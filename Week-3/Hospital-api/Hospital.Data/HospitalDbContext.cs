using HospitalApi.Models.Pharmacy;
using Microsoft.EntityFrameworkCore;
namespace HospitalApi.Data;

public class HospitalDbContext : DbContext
{
    public HospitalDbContext(DbContextOptions<HospitalDbContext> options) : base(options) { }

    public DbSet<Medication> Medications => Set<Medication>();
    public DbSet<InventoryItem> Inventory => Set<InventoryItem>();
    public DbSet<PrescriptionDetail> PrescriptionDetails => Set<PrescriptionDetail>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Medication>()
            .HasOne(m => m.Inventory)
            .WithOne(i => i.Medication)
            .HasForeignKey<InventoryItem>(i => i.MedicationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Medication>()
            .HasMany(m => m.PrescriptionDetails)
            .WithOne(p => p.Medication)
            .HasForeignKey(p => p.MedicationId)
            .OnDelete(DeleteBehavior.Restrict); 
    }
}
