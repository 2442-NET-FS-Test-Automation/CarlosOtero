using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Hospital.Data.Migrations
{
    /// <inheritdoc />
    public partial class MedicationItemIdentifier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventory_Medications_MedicationId",
                table: "Inventory");

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "AppointmentID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Inventory",
                keyColumn: "InventoryID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Inventory",
                keyColumn: "InventoryID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Inventory",
                keyColumn: "InventoryID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "MedicalRecords",
                keyColumn: "RecordID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "MedicalRecords",
                keyColumn: "RecordID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "AppointmentID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Medications",
                keyColumn: "MedicationID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Medications",
                keyColumn: "MedicationID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Medications",
                keyColumn: "MedicationID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "PatientID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "PatientID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "PatientID",
                keyValue: 1);

            migrationBuilder.RenameColumn(
                name: "MedicationId",
                table: "Inventory",
                newName: "MedicationID");

            migrationBuilder.RenameIndex(
                name: "IX_Inventory_MedicationId",
                table: "Inventory",
                newName: "IX_Inventory_MedicationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventory_Medications_MedicationID",
                table: "Inventory",
                column: "MedicationID",
                principalTable: "Medications",
                principalColumn: "MedicationID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventory_Medications_MedicationID",
                table: "Inventory");

            migrationBuilder.RenameColumn(
                name: "MedicationID",
                table: "Inventory",
                newName: "MedicationId");

            migrationBuilder.RenameIndex(
                name: "IX_Inventory_MedicationID",
                table: "Inventory",
                newName: "IX_Inventory_MedicationId");

            migrationBuilder.InsertData(
                table: "Medications",
                columns: new[] { "MedicationID", "BrandName", "DosageForm", "GenericName", "Name", "Strength", "UnitPrice" },
                values: new object[,]
                {
                    { 1, "Toprol XL", "Tablet", "Metoprolol", "Metoprolol Succinate", "50mg", 0.75m },
                    { 2, "Amoxil", "Capsule", "Amoxicillin", "Amoxicillin Trihydrate", "500mg", 0.40m },
                    { 3, "Advil", "Tablet", "Ibuprofen", "Ibuprofen", "400mg", 0.15m }
                });

            migrationBuilder.InsertData(
                table: "Patients",
                columns: new[] { "PatientID", "Allergies", "BloodType", "DateOfBirth", "FirstName", "Gender", "Insurance", "LastName" },
                values: new object[,]
                {
                    { 1, "Peanuts", "A+", new DateTime(1985, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "John", "Male", "Blue Cross", "Doe" },
                    { 2, "Penicillin", "O-", new DateTime(1992, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mary", "Female", "Cigna", "Jane" },
                    { 3, null, "B+", new DateTime(2018, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Timmy", "Male", "Medicaid", "Turner" }
                });

            migrationBuilder.InsertData(
                table: "Appointments",
                columns: new[] { "AppointmentID", "AppointmentDate", "AppointmentTime", "DoctorID", "PatientID", "ReasonForVisit", "Status" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 9, 30, 0, 0), 1, 1, "Chest pain follow-up", "Completed" },
                    { 2, new DateTime(2026, 7, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 14, 0, 0, 0), 2, 3, "Routine pediatric checkup", "Scheduled" }
                });

            migrationBuilder.InsertData(
                table: "Inventory",
                columns: new[] { "InventoryID", "BatchNumber", "ExpiryDate", "MedicationId", "StockQuantity", "SupplierName" },
                values: new object[,]
                {
                    { 1, "BATCH-M12", new DateTime(2028, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 500, "PharmaCorp Inc" },
                    { 2, "BATCH-A99", new DateTime(2027, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1200, "Global Meds Dist" },
                    { 3, "BATCH-I44", new DateTime(2029, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 2000, "PharmaCorp Inc" }
                });

            migrationBuilder.InsertData(
                table: "MedicalRecords",
                columns: new[] { "RecordID", "AppointmentID", "ClinicalNotes", "Diagnosis", "DoctorID", "PatientID", "Symptoms", "TreatmentPlan", "VisitDate" },
                values: new object[,]
                {
                    { 2, null, "ER walk-in. Patient transferred directly to surgery floor.", "Appendicitis", 3, 2, "Acute appendicitis symptoms, severe lower right quadrant pain", "Emergency Appendectomy scheduled immediately", new DateTime(2026, 7, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 1, 1, "Patient responding well to lifestyle adjustments.", "Stable Angina", 1, 1, "Mild chest tightness during exercise", "Prescribed Beta-Blockers. Low sodium diet.", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Inventory_Medications_MedicationId",
                table: "Inventory",
                column: "MedicationId",
                principalTable: "Medications",
                principalColumn: "MedicationID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
