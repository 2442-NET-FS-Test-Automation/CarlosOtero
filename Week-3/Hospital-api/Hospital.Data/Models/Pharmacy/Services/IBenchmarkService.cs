using HospitalApi.DTOs.Pharmacy;

namespace HospitalApi.Services.Pharmacy;

public record BenchmarkMetricsResult(
    double SequentialTimeMs,
    double ParallelTimeMs,
    double CalculatedSpeedupFactor,
    int TotalProcessed
);

public interface IBenchmarkService
{
    Task<BenchmarkMetricsResult> RunLoadTestBenchmarkAsync(int totalRequests, CancellationToken ct);
}