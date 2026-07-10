using HospitalApi.Data;
using HospitalApi.DTOs.Pharmacy;
using HospitalApi.Models.Pharmacy;
using HospitalApi.Services.Infrastructure;
using HospitalApi.Services.Pharmacy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalApi.Controllers.Pharmacy;

[ApiController]
[Route("api/pharmacy/[controller]")] // Base route path URL: api/pharmacy/inventoryitem
[Produces("application/json")]
public class InventoryItemController : ControllerBase
{
    private readonly ISeederService _seederService; // 🟢 FIXED: Using explicit infrastructure interface spelling
    private readonly IInventoryItemService _service;
    private readonly IFulfillmentService _fulfillmentService;
    private readonly IDbContextFactory<HospitalDbContext> _factory;
    private readonly IBenchmarkService _benchmarkService; // 🟢 FIXED: Field tracks abstract interface type

    public InventoryItemController(
        IInventoryItemService service,
        ISeederService seederService, // 🟢 FIXED: Direct interface parameter typing mapping
        IFulfillmentService fulfillmentService,
        IDbContextFactory<HospitalDbContext> factory,
        IBenchmarkService benchmarkService) // 🟢 FIXED: Parameter tracks abstract interface type
    {
        _service = service;
        _seederService = seederService;
        _fulfillmentService = fulfillmentService;
        _factory = factory;
        _benchmarkService = benchmarkService; // 🟢 FIXED: Assigned benchmark service correctly to local field
    }

    /// <summary>
    /// Retrieves a list of all active pharmacy warehouse inventory items.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<InventoryItemDto>))] // 🟢 FIXED: Type safety matching response DTOs
    public async Task<IActionResult> Get()
    {
        var items = await _service.AllAsync();
        return Ok(items);
    }

    /// <summary>
    /// Resets the pharmacy inventory records back to pristine mock data metrics.
    /// </summary>
    [HttpPost("reset")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ResetInventoryDomain()
    {
        await _seederService.ResetDatabaseAsync();
        return Ok(new { message = "Stock records successfully reverted to default mock inventory items." });
    }

    /// <summary>
    /// Milestone M2: Analytical LINQ report summarizing inventory volumes grouped by vendor supplier metrics.
    /// </summary>
    [HttpGet("reports/supplier-stock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSupplierStockReport()
    {
        await using var db = await _factory.CreateDbContextAsync();

        var report = await db.Inventory
            .GroupBy(i => i.SupplierName)
            .Select(g => new
            {
                Supplier = g.Key,
                TotalBatchesTracked = g.Count(),
                TotalStockAvailable = g.Sum(i => i.StockQuantity) // M2 Requirement: Valid LINQ aggregation summation
            })
            .ToListAsync();

        return Ok(report);
    }

    /// <summary>
    /// Milestone M4 Target: Triggers an automated heavy-concurrency load benchmark test loop.
    /// Resets stock, runs sequential vs parallel tests, and logs performance speedup calculations.
    /// </summary>
    [HttpPost("benchmark-test")]
    public async Task<IActionResult> ExecuteLoadBenchmark(CancellationToken ct, [FromQuery] int requestsCount = 100)
    {
        if (requestsCount <= 0) return BadRequest("Request count volume parameters must be greater than zero.");

        var metricAnalysis = await _benchmarkService.RunLoadTestBenchmarkAsync(requestsCount, ct);
        return Ok(metricAnalysis);
    }

    /// <summary>
    /// Manually appends a new tracking batch onto an existing medication identifier root catalog.
    /// </summary>
    [HttpPost("inventory/{medicationId}")]
    public async Task<IActionResult> CreateInventory(int medicationId, [FromBody] CreateInventoryItemDto payload)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var createdDto = await _service.AddInventoryAsync(medicationId, payload);

        return CreatedAtAction(nameof(GetInventoryById), new { id = createdDto.Id }, createdDto);
    }

    /// <summary>
    /// Looks up a unique inventory batch data card matching a specific identification key.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetInventoryById(int id)
    {
        var inventoryItem = await _service.ByIdAsync(id);

        if (inventoryItem == null) return NotFound($"Inventory item tracking batch with ID {id} was not found.");
        return Ok(inventoryItem);
    }

    /// <summary>
    /// Milestone M3 Burst Endpoint: Offloads complex multi-threaded order transactions concurrently over the factory layer pipelines.
    /// </summary>
    [HttpPost("burst-process")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BurstResult))]
    public async Task<IActionResult> ProcessBurst([FromBody] IEnumerable<BurstRequestPayload> payloads, CancellationToken ct)
    {
        var metricsResult = await _fulfillmentService.FulfillBurstAsync(payloads, ct);
        return Ok(metricsResult); // Returns the total number of fulfilled vs backordered records
    }

    /// <summary>
    /// Deletes a specific inventory stock batch by its unique ID.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteInventory(int id)
    {
        var wasDeleted = await _service.RemoveAsync(id);

        if (!wasDeleted)
        {
            return NotFound($"Inventory item with ID {id} does not exist inside the active tracking context.");
        }

        return NoContent();
    }
}