using Dream_PC_Parts_Picker_API.Models;

namespace Dream_PC_Parts_Picker_API.Services;

/// <summary>
/// Service interface for managing build benchmarks.
/// </summary>
public interface IBuildBenchmarkService
{
    Task<List<BuildBenchmark>> GetBenchmarksForBuildAsync(int userId, int buildId);
    Task<BuildBenchmark?> GetBenchmarkAsync(int userId, int benchmarkId);
    Task<BuildBenchmark?> CreateBenchmarkAsync(
        int userId,
        int buildId,
        int overallScore,
        int? cpuScore,
        int? gpuScore,
        string? notes
    );
    Task<bool> UpdateBenchmarkAsync(
        int userId,
        int benchmarkId,
        int overallScore,
        int? cpuScore,
        int? gpuScore,
        string? notes
    );
    Task<bool> DeleteBenchmarkAsync(int userId, int benchmarkId);
}