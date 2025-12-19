namespace Dream_PC_Parts_Picker_Web.Models;

public record PartDto(
    int Id,
    int PartCategoryId,
    string PartCategoryName,
    string Name,
    string Manufacturer,
    string ModelNumber,
    decimal Price,
    int PerformanceScore,
    int TdpWatts
);