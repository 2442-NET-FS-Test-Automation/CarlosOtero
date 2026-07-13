using Library.Data;
using Microsoft.EntityFrameworkCore;
using Library.ControllersApi.Mapping;
using Library.ControllersApi.Services;
using Serilog;
using Library.ControllerApi.Middleware;
using Library.ControllerApi.Filters;
using Library.ControllersApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Adding connection string
var conn_string = "Server=localhost,1433;Database=LibraryMinimalDb;User ID=sa;Password=Pass-123;Trust Server Certificate=True";


Log.Logger = new LoggerConfiguration()
.WriteTo.Console() // Write to console, and write to a file - starting a new file each day
.WriteTo.File("logs/fulfillment-log-.txt", rollingInterval: RollingInterval.Day)
.CreateLogger();

builder.Host.UseSerilog(); // tell the builder to use Serilog for logging

// Adding CORS
const string SpaCorsPolicy = "spa"; // string nme for our policy

// Configuring our CORS policy
builder.Services.AddCors(o=>o.AddPolicy(SpaCorsPolicy, p=> p
    .WithOrigins("http://localhost:3000")
    .AllowAnyHeader()
    .AllowAnyMethod()
));


// VALIDATION side of JWT. Issuance lives in TokenService
var jwtKey = builder.Configuration["Jwt:Key"]; //from appsettings.Development.json


// Hardcoding the issuer and audience - these have to match the ones we set on the token
const string jwtIssuer = "library-fulfillment";
const string jwtAudience = "library-fulfillment-clients";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o=>o.TokenValidationParameters =new TokenValidationParameters
    {
        ValidateIssuer = true, ValidIssuer = jwtIssuer,
        ValidateAudience = true, ValidAudience = jwtAudience,
        ValidateIssuerSigningKey = true, IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateLifetime = true
    });

builder.Services.AddAuthorization(); // Goes after authentication

// token Issuance is a plain injectable service. It's stateless so we can use a singleton
builder.Services.AddSingleton<ITokenService,TokenService>();

// Adding our HttpClient
builder.Services.AddHttpClient<ISupplierClient, SupplierClient>(c=>
    c.BaseAddress = new Uri("https://dummyjson.com/") // all calls append to this URL
);

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

builder.Services.AddMemoryCache(); // adding cache-ing to our server
builder.Services.AddResponseCaching(); // adding response cache-ing - asking the front-end to save request results

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

app.UseResponseCaching(); // using the response cache middleware

app.UseCors(SpaCorsPolicy); // Using our policy with the CORS middleware

// Must be in this order for Authn/Authz
app.UseAuthentication(); // read and validate thr tokens -> set User
app.UseAuthorization(); // enforces the [Authorize] / RequireAuthorization() decorators on endpoints

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

Log.CloseAndFlush(); // Close and flush the logs (serilog)