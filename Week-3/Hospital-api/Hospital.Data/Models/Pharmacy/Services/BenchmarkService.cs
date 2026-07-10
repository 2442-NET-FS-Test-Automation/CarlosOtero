using System.Diagnostics;
using HospitalApi.DTOs.Pharmacy;
using HospitalApi.Services.Infrastructure;
using Serilog;

namespace HospitalApi.Services.Pharmacy;

public class BenchmarkService : IBenchmarkService
{
    private readonly IFulfillmentService _fulfillmentService;
    private readonly ISeederService _seederService;

    public BenchmarkService(IFulfillmentService fulfillmentService, ISeederService seederService)
    {
        _fulfillmentService = fulfillmentService;
        _seederService = seederService;
    }

    public async Task<BenchmarkMetricsResult> RunLoadTestBenchmarkAsync(int totalRequests, CancellationToken ct)
    {
        var loadPack = Enumerable.Range(1, totalRequests).Select(index => new BurstRequestPayload(
            AppointmentID: index,
            InventoryID: (index % 2 == 0) ? 1 : 2,
            QuantityRequested: 1
        )).ToList();

        await _seederService.ResetDatabaseAsync(); 
        
        var seqTimer = Stopwatch.StartNew();
        foreach (var req in loadPack)
        {
            ct.ThrowIfCancellationRequested(); 
            await _fulfillmentService.FulfillOneAsync(req.AppointmentID, req.InventoryID, req.QuantityRequested, ct);
        }
        seqTimer.Stop();

        await _seederService.ResetDatabaseAsync(); 
        
        var parTimer = Stopwatch.StartNew();
        await _fulfillmentService.FulfillBurstAsync(loadPack, ct);
        parTimer.Stop();

        double seqMs = seqTimer.ElapsedMilliseconds;
        double parMs = parTimer.ElapsedMilliseconds;
        double speedup = seqMs / (parMs > 0 ? parMs : 1);

        Log.Information("[BENCHMARK] Sequential Run: {SeqTime}ms | Parallel Burst Run: {ParTime}ms", seqMs, parMs);
        Log.Information("[BENCHMARK] .NET 10 Parallel Core Engine provided {Speedup}x velocity acceleration factor", Math.Round(speedup, 2));

        return new BenchmarkMetricsResult(seqMs, parMs, Math.Round(speedup, 2), totalRequests);
    }
}