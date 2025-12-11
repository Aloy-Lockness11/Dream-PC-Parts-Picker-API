namespace Dream_PC_Parts_Picker_API.DTOs.Parts;

/// <summary>
/// Part Data Transfer Object
/// </summary>
public record PartDto(
    int Id,
    int PartCategoryId,
    string PartCategoryName,
    string Name,
    string Manufacturer,
    string? ModelNumber,
    decimal Price,
    int? PerformanceScore,
    int? TdpWatts
);

/// <summary>
/// Create Part Request DTO
/// </summary>
public record CreatePartRequest(
    int PartCategoryId,
    string Name,
    string Manufacturer,
    string? ModelNumber,
    decimal Price,
    int? PerformanceScore,
    int? TdpWatts
);

/// <summary>
/// Request body for updating an  PC part
/// </summary>
public record UpdatePartRequest(
    int PartCategoryId,
    string Name,
    string Manufacturer,
    string? ModelNumber,
    decimal Price,
    int? PerformanceScore,
    int? TdpWatts
);