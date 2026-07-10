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


var builder = WebApplication.CreateBuilder(args);


var conn_string = "Server=localhost,1433;Database=HospitalDb;User ID=sa;Password=Pass-123;Trust Server Certificate=True";

Log.Logger = new LoggerConfiguration()
.WriteTo.Console() 
.WriteTo.File("logs/fulfillment-log-.txt", rollingInterval: RollingInterval.Day)
.CreateLogger();

builder.Host.UseSerilog(); 




builder.Services.AddDbContext<HospitalDbContext>(Options => Options.UseSqlServer(conn_string),
ServiceLifetime.Scoped, ServiceLifetime.Singleton); 

builder.Services.AddDbContextFactory<HospitalDbContext>(options => options.UseSqlServer(conn_string));

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
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Hospital Domain API v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseAuthorization();
app.MapControllers(); 

app.MapGet("/", () => "Hospital Enterprise API Gateway Online running on .NET 10.");


AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) =>
{
    Log.Information("Hospital API Server process stopping gracefully. Flushing telemetry logs buffers...");
    Log.CloseAndFlush();
};

app.Run();