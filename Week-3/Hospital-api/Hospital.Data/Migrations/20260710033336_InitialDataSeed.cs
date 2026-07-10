using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Hospital.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialDataSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Medications",
                columns: new[] { "MedicationID", "BrandName", "DosageForm", "GenericName", "Name", "Strength", "UnitPrice" },
                values: new object[,]
                {
                    { 1, "Toprol XL", "Tablet", "Metoprolol", "Metoprolol Succinate", "50mg", 0.75m },
                    { 2, "Amoxil", "Capsule", "Amoxicillin", "Amoxicillin Trihydrate", "500mg", 0.40m }
                });

            migrationBuilder.InsertData(
                table: "Inventory",
                columns: new[] { "InventoryID", "BatchNumber", "ExpiryDate", "MedicationID", "StockQuantity", "SupplierName" },
                values: new object[,]
                {
                    { 1, "BATCH-M12", new DateTime(2028, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 500, "PharmaCorp Inc" },
                    { 2, "BATCH-A99", new DateTime(2027, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1200, "Global Meds Dist" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Inventory",
                keyColumn: "InventoryID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Inventory",
                keyColumn: "InventoryID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Medications",
                keyColumn: "MedicationID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Medications",
                keyColumn: "MedicationID",
                keyValue: 2);
        }
    }
}
