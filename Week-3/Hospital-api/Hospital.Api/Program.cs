using HospitalApi.Data;
using HospitalApi.DTOs.Pharmacy;
using HospitalApi.Models.Pharmacy;

//using HospitalApi.Models.Medical.Services;
using HospitalApi.Models.Pharmacy.Services;
using HospitalApi.Repositories.Pharmacy;
using HospitalApi.Services.Infrastructure;
using HospitalApi.Services.Pharmacy;
using Microsoft.EntityFrameworkCore;
using Serilog;

// Builder area
var builder = WebApplication.CreateBuilder(args);

// the first thing that we need is to give our builder a connection string to our database
var conn_string = "Server=localhost,1433;Database=HospitalDb;User ID=sa;Password=Pass-123;Trust Server Certificate=True";

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
builder.Services.AddDbContext<HospitalDbContext>(Options => Options.UseSqlServer(conn_string),
ServiceLifetime.Scoped, ServiceLifetime.Singleton); // Scoped is the default, but we can be explicit - and allow for SingletonScope when needed

// we know we will need more than one HospitalDbContext in one or more of these methods. But we don't know how many
// before runtime. So we can use a DbContextFactory to create as many as we need at runtime.
builder.Services.AddDbContextFactory<HospitalDbContext>(options => options.UseSqlServer(conn_string));

//builder.Services.AddScoped<IFulfillmentService, FulfillmentService>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IInventoryItemService, InventoryItemService>();
builder.Services.AddScoped<IMedicationRepository, MedicationRepository>();
builder.Services.AddScoped<IMedicationService, MedicationService>();
builder.Services.AddScoped<ISeederService, SeederService>();
builder.Services.AddScoped<IFulfillmentService, FulfillmentService>();
builder.Services.AddScoped<IBenchmarkService, BenchmarkService>();
builder.Services.AddScoped<ISeederService, SeederService>();
builder.Services.AddScoped<IBurstPlanner, BurstPlanner>();
//builder.Services.AddScoped<IMedicalRecordsService,MedicalRecordsService>();
//builder.Services.AddScoped<IPatientService,PatientService>();
//builder.Services.AddScoped<IAppointmentService,AppointmentService>();

builder.Services.AddAuthorization();
builder.Services.AddControllers();
// Swagger stuff added to builder
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// App area
var app = builder.Build();

// Host the interactive user workspace visualization blocks during development passes
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Hospital Domain API v1");
        options.RoutePrefix = string.Empty; // Launches your Swagger interface page right at http://localhost:XXXX/
    });
}

app.UseAuthorization();
app.MapControllers(); // 🟢 Discovers and opens up your medications and inventory controllers automatically

// Minimal API Diagnostic Health Verification Endpoint
app.MapGet("/", () => "Hospital Enterprise API Gateway Online running on .NET 10.");

// ==========================================
// 3. MILESTONE M5: GRACEFUL SHUTDOWN ENGINE
// ==========================================
AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) =>
{
    Log.Information("Hospital API Server process stopping gracefully. Flushing telemetry logs buffers...");
    Log.CloseAndFlush();
};

app.Run();