namespace Dream_PC_Parts_Picker_API.DTOs.Benchmarks;

/// <summary>
/// Benchmark summary data returned from the API.
/// </summary>
public record BuildBenchmarkSummaryDto(
    int Id,
    int OverallScore,
    int? CpuScore,
    int? GpuScore,
    DateTime CreatedAt
);

/// <summary>
/// Detailed benchmark data returned from the API.
/// </summary>
public record BuildBenchmarkDto(
    int Id,
    int BuildId,
    int OverallScore,
    int? CpuScore,
    int? GpuScore,
    string? Notes,
    DateTime CreatedAt
);

/// <summary>
/// Coollection of benchmarks for a specific build.
/// </summary>
public record CreateBuildBenchmarkRequest(
    int OverallScore,
    int? CpuScore,
    int? GpuScore,
    string? Notes
);

/// <summary>
/// Yo update benchmark data for a specific build.
/// </summary>
public record UpdateBuildBenchmarkRequest(
    int OverallScore,
    int? CpuScore,
    int? GpuScore,
    string? Notes
);