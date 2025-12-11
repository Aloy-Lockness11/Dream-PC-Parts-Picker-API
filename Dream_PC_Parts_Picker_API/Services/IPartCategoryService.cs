using Dream_PC_Parts_Picker_API.Models;

namespace Dream_PC_Parts_Picker_API.Services;

/// <summary>
/// Service interface for managing part categories.
/// </summary>
public interface IPartCategoryService
{
    Task<List<PartCategory>> GetAllAsync();
    Task<PartCategory?> GetByIdAsync(int id);
    Task<PartCategory> CreateAsync(string name, string? description);
    Task<bool> UpdateAsync(int id, string name, string? description);
    Task<bool> DeleteAsync(int id);
}