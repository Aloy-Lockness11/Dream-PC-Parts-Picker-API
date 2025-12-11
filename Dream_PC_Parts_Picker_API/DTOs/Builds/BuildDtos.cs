using Dream_PC_Parts_Picker_API.DTOs.Benchmarks;

namespace Dream_PC_Parts_Picker_API.DTOs.Builds;

/// <summary>
/// A single part inside a build returned by the API.
/// </summary>
public record BuildPartItemDto(
    int PartId,
    string PartName,
    string PartCategoryName,
    decimal Price,
    int Quantity
);

/// <summary>
/// Summary information for a user's build.
/// </summary>
public record BuildSummaryDto(
    int Id,
    string Name,
    string? Description,
    decimal TotalPrice,
    int PartsCount,
    DateTime CreatedAt
);

/// <summary>
/// Detailed information for a user's build, including parts and benchmarks.
/// </summary>
public record BuildDetailDto(
    int Id,
    string Name,
    string? Description,
    decimal TotalPrice,
    DateTime CreatedAt,
    IReadOnlyCollection<BuildPartItemDto> Parts,
    IReadOnlyCollection<BuildBenchmarkSummaryDto> Benchmarks
);

/// <summary>
/// DTO for creating a new build with its parts.
/// </summary>
public record CreateBuildRequest(
    string Name,
    string? Description,
    IReadOnlyCollection<CreateBuildPartItemRequest> Parts
);

/// <summary>
/// DtO for updating an existing build and its parts.
/// </summary>
public record UpdateBuildRequest(
    string Name,
    string? Description,
    IReadOnlyCollection<CreateBuildPartItemRequest> Parts
);

/// <summary>
/// DTO for adding a part to a build.
/// </summary>
public record CreateBuildPartItemRequest(
    int PartId,
    int Quantity
);