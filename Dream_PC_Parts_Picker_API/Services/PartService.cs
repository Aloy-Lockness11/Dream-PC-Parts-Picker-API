using Dream_PC_Parts_Picker_API.Data;
using Dream_PC_Parts_Picker_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Dream_PC_Parts_Picker_API.Services;

public class PartService : IPartService
{
    private readonly AppDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="PartService"/> class
    /// </summary>
    /// <param name="db"></param>
    public PartService(AppDbContext db)
    {
        _db = db;
    }
    
    /// <summary>
    ///  Gets all parts, optionally filtered by category ID.
    /// </summary>
    /// <param name="categoryId"></param>
    /// <returns></returns>
    public async Task<List<Part>> GetAllAsync(int? categoryId = null)
    {
        var query = _db.Parts
            .Include(p => p.PartCategory)
            .AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.PartCategoryId == categoryId.Value);
        }

        return await query
            .OrderBy(p => p.PartCategory.Name)
            .ThenBy(p => p.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Gets a part by its ID.
    /// </summary>
    public async Task<Part?> GetByIdAsync(int id)
    {
        return await _db.Parts
            .Include(p => p.PartCategory)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
    
    /// <summary>
    /// Creates a new part.
    /// </summary>
    public async Task<Part?> CreateAsync(
        int partCategoryId,
        string name,
        string manufacturer,
        string? modelNumber,
        decimal price,
        int? performanceScore,
        int? tdpWatts
    )
    {
        var categoryExists = await _db.PartCategories
            .AnyAsync(c => c.Id == partCategoryId);

        if (!categoryExists)
        {
            return null;
        }

        var entity = new Part
        {
            PartCategoryId = partCategoryId,
            Name = name,
            Manufacturer = manufacturer,
            ModelNumber = modelNumber,
            Price = price,
            PerformanceScore = performanceScore,
            TdpWatts = tdpWatts
        };

        _db.Parts.Add(entity);
        await _db.SaveChangesAsync();

        // Reload with category
        await _db.Entry(entity)
            .Reference(p => p.PartCategory)
            .LoadAsync();

        return entity;
    }
    
    /// <summary>
    /// Updates an existing part.
    /// </summary>
    public async Task<bool> UpdateAsync(
        int id,
        int partCategoryId,
        string name,
        string manufacturer,
        string? modelNumber,
        decimal price,
        int? performanceScore,
        int? tdpWatts
    )
    {
        var entity = await _db.Parts.FindAsync(id);
        if (entity == null) return false;

        var categoryExists = await _db.PartCategories
            .AnyAsync(c => c.Id == partCategoryId);

        if (!categoryExists)
        {
            return false;
        }

        entity.PartCategoryId = partCategoryId;
        entity.Name = name;
        entity.Manufacturer = manufacturer;
        entity.ModelNumber = modelNumber;
        entity.Price = price;
        entity.PerformanceScore = performanceScore;
        entity.TdpWatts = tdpWatts;

        await _db.SaveChangesAsync();
        return true;
    }
    
    /// <summary>
    /// Deletes a part by its ID.
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _db.Parts.FindAsync(id);
        if (entity == null) return false;

        _db.Parts.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }
}
