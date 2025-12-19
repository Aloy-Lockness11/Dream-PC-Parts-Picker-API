using Dream_PC_Parts_Picker_API.Models;

namespace Dream_PC_Parts_Picker_API.Services;

/// <summary>
/// Service interface for managing parts.
/// </summary>
public interface IPartService
{
    Task<IEnumerable<Part>> GetAllAsync(int? categoryId, string? sortBy, string? sortDir);
    Task<Part?> GetByIdAsync(int id);
    Task<Part?> CreateAsync(int partCategoryId, string name, string manufacturer, string? modelNumber,
        decimal price, int? performanceScore, int? tdpWatts);
    Task<bool> UpdateAsync(int id, int partCategoryId, string name, string manufacturer, string? modelNumber,
        decimal price, int? performanceScore, int? tdpWatts);
    Task<bool> DeleteAsync(int id);
}
