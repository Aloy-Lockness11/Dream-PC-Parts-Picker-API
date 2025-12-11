namespace Dream_PC_Parts_Picker_API.DTOs.PartCategories;
// DTOs for Part Categories

public record PartCategoryDto(int Id, string Name, string? Description);

public record CreatePartCategoryRequest(string Name, string? Description);

public record UpdatePartCategoryRequest(string Name, string? Description);