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
[Route("api/pharmacy/[controller]")] 
[Produces("application/json")]
public class InventoryItemController : ControllerBase
{
    private readonly ISeederService _seederService; 
    private readonly IInventoryItemService _service;
    private readonly IFulfillmentService _fulfillmentService;
    private readonly IDbContextFactory<HospitalDbContext> _factory;
    private readonly IBenchmarkService _benchmarkService; 

    public InventoryItemController(
        IInventoryItemService service,
        ISeederService seederService, 
        IFulfillmentService fulfillmentService,
        IDbContextFactory<HospitalDbContext> factory,
        IBenchmarkService benchmarkService) 
    {
        _service = service;
        _seederService = seederService;
        _fulfillmentService = fulfillmentService;
        _factory = factory;
        _benchmarkService = benchmarkService; 
    }


    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var items = await _service.AllAsync();
        return Ok(items);
    }


    [HttpPost("reset")]
    public async Task<IActionResult> ResetInventoryDomain()
    {
        await _seederService.ResetDatabaseAsync();
        return Ok(new { message = "Stock records successfully reverted to default mock inventory items." });
    }


    [HttpGet("reports/supplier-search")]
    public async Task<IActionResult> SearchSupplierStock([FromQuery] string targetSupplier)
    {
        await using var db = await _factory.CreateDbContextAsync();

        var reportData = await db.Inventory
            .GroupBy(i => i.SupplierName)
            .Select(g => new SupplierReportDto(
                g.Key,
                g.Count(),
                g.Sum(i => i.StockQuantity)
            ))
            .ToListAsync();


        var sortedReport = reportData.OrderBy(r => r.SupplierName).ToList();


        int index = sortedReport.BinarySearch(
            new SupplierReportDto(targetSupplier, 0, 0),
            Comparer<SupplierReportDto>.Create((x, y) => string.Compare(x.SupplierName, y.SupplierName, StringComparison.OrdinalIgnoreCase))
        );

        if (index < 0)
            return NotFound($"Supplier '{targetSupplier}' was not found in the sorted index.");

        return Ok(sortedReport[index]);
    }

    [HttpGet("reports/supplier-stock")]
    public async Task<IActionResult> GetSupplierStockReport()
    {
        await using var db = await _factory.CreateDbContextAsync();

        var report = await db.Inventory
            .GroupBy(i => i.SupplierName)
            .Select(g => new
            {
                Supplier = g.Key,
                TotalBatchesTracked = g.Count(),
                TotalStockAvailable = g.Sum(i => i.StockQuantity) 
            })
            .ToListAsync();

        return Ok(report);
    }

    [HttpPost("benchmark-test")]
    public async Task<IActionResult> ExecuteLoadBenchmark(CancellationToken ct, [FromQuery] int requestsCount = 100)
    {
        if (requestsCount <= 0) return BadRequest("Request count volume parameters must be greater than zero.");

        var metricAnalysis = await _benchmarkService.RunLoadTestBenchmarkAsync(requestsCount, ct);
        return Ok(metricAnalysis);
    }


    [HttpPost("inventory/{medicationId}")]
    public async Task<IActionResult> CreateInventory(int medicationId, [FromBody] CreateInventoryItemDto payload)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var createdDto = await _service.AddInventoryAsync(medicationId, payload);

        return CreatedAtAction(nameof(GetInventoryById), new { id = createdDto.Id }, createdDto);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetInventoryById(int id)
    {
        var inventoryItem = await _service.ByIdAsync(id);

        if (inventoryItem == null) return NotFound($"Inventory item tracking batch with ID {id} was not found.");
        return Ok(inventoryItem);
    }


    [HttpPost("burst-process")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BurstResult))]
    public async Task<IActionResult> ProcessBurst([FromBody] IEnumerable<BurstRequestPayload> payloads, CancellationToken ct)
    {
        var metricsResult = await _fulfillmentService.FulfillBurstAsync(payloads, ct);
        return Ok(metricsResult); 
    }


    [HttpDelete("{id}")]
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