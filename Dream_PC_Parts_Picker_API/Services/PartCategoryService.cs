using Dream_PC_Parts_Picker_API.Data;
using Dream_PC_Parts_Picker_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Dream_PC_Parts_Picker_API.Services;

/// Service for managing part categories in the Dream PC Parts Picker API
public class PartCategoryService : IPartCategoryService
{
    private readonly AppDbContext _db;
    /// <summary>
    /// Initializes a new instance of the <see cref="PartCategoryService"/> class
    /// </summary>
    public PartCategoryService(AppDbContext db)
    {
        _db = db;
    }
    
    /// <summary>
    ///  Gets all part categories.
    /// </summary>
    /// <returns></returns>
    public async Task<List<PartCategory>> GetAllAsync()
    {
        return await _db.PartCategories
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Gets a part category by its ID.
    /// </summary>
    public async Task<PartCategory?> GetByIdAsync(int id)
    {
        return await _db.PartCategories
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    /// <summary>
    /// Creates a new part category.
    /// </summary>
    public async Task<PartCategory> CreateAsync(string name, string? description)
    {
        var entity = new PartCategory
        {
            Name = name,
            Description = description
        };

        _db.PartCategories.Add(entity);
        await _db.SaveChangesAsync();

        return entity;
    }

    
    /// <summary>
    /// Updates an existing part category.
    /// </summary>
    public async Task<bool> UpdateAsync(int id, string name, string? description)
    {
        var entity = await _db.PartCategories.FindAsync(id);
        if (entity == null) return false;

        entity.Name = name;
        entity.Description = description;

        await _db.SaveChangesAsync();
        return true;
    }
    
    /// <summary>
    /// Deletes a part category by its ID.
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _db.PartCategories.FindAsync(id);
        if (entity == null) return false;

        _db.PartCategories.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }
}