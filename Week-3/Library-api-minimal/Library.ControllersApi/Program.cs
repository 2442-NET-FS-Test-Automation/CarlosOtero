using Library.Data;
using Microsoft.EntityFrameworkCore;
using Library.ControllersApi.Mapping;
using Library.ControllersApi.Services;
using Serilog;
using Library.ControllerApi.Middleware;
using Library.ControllerApi.Filters;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Adding connection string
var conn_string = "Server=localhost,1433;Database=LibraryMinimalDb;User ID=sa;Password=Pass-123;Trust Server Certificate=True";

Log.Logger = new LoggerConfiguration()
.WriteTo.Console() // Write to console, and write to a file - starting a new file each day
.WriteTo.File("logs/fulfillment-log-.txt", rollingInterval: RollingInterval.Day)
.CreateLogger();

builder.Host.UseSerilog(); // tell the builder to use Serilog for logging

builder.Services.AddDbContextFactory<LibraryDbContext>(o => o.UseSqlServer(conn_string));

// Registering our custom Repo and Service Layer methods like we did before
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IInventoryService, InventoryService>();

// Adding our mapping profile for AutoMapper
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(MappingProfile).Assembly));

// Having our filter apply to every controller
builder.Services.AddControllers(o => o.Filters.Add<TimingFilter>());

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Adding swagger back
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>(); // Wraps all middleware below it, catches their exceptions

// Swagger stuff added to the app
app.UseSwagger();
app.UseSwaggerUI();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// This is a simple diagnostic middleware. All it will do is time our requests for us and log that.
// It takes in ctx (HttpContext -> everything about the request AND the response)
// next - represents a call to the subsequent middleware
app.Use(async (ctx, next) =>
{
    var sw = System.Diagnostics.Stopwatch.StartNew();

    await next();

    sw.Stop();
    Log.Information("{Method} {Path} -> {StatusCode} in {Elapsed} ms",
        ctx.Request.Method, ctx.Request.Path, ctx.Response.StatusCode, sw.ElapsedMilliseconds
    );
});

app.Use(async (ctx, next) =>
{
    if (ctx.Request.Headers.ContainsKey("X-Maintenance"))
    {
        ctx.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
        await ctx.Response.WriteAsync("Down for maintenance");
        return; // Don't call next() - never hits controllers
    }

    await next(ctx);
});

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();
