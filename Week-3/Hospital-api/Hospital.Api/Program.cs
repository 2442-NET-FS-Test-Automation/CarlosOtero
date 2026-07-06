using HospitalApi.Data;
using HospitalApi.Models.Pharmacy;
using Microsoft.EntityFrameworkCore;
using Serilog;

// Builder area
var builder = WebApplication.CreateBuilder(args);

// the first thing that we need is to give our builder a connection string to our database
var conn_string = "Server=localhost,1433;Database=HospitalDb;User ID=sa;Password=Pass-123;Trust Server Certificate=True";

Log.Logger = new LoggerConfiguration()
.WriteTo.Console() // Write to console, and write to a file - starting a new file each day
.WriteTo.File("logs/fulfillment-log-.txt", rollingInterval : RollingInterval.Day)
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
builder.Services.AddAuthorization(); 
builder.Services.AddControllers(); 
// Swagger stuff added to builder
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// App area
var app = builder.Build();

// Swagger stuff added to app
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.MapControllers();


// Endpoint area
app.MapGet("/", () => "Hello World!");

// My file always ends with app.Run() - minimal API or Controller API

app.Run();
Log.CloseAndFlush();