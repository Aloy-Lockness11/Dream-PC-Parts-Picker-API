using Dream_PC_Parts_Picker_API.Data;
using Dream_PC_Parts_Picker_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Dream_PC_Parts_Picker_API.Services;

public class BuildService : IBuildService
{
    private readonly AppDbContext _db;

    public BuildService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Build>> GetBuildsForUserAsync(int userId)
    {
        return await _db.Builds
            .Where(b => b.UserId == userId)
            .Include(b => b.BuildParts)
                .ThenInclude(bp => bp.Part)
                    .ThenInclude(p => p.PartCategory)
            .Include(b => b.Benchmarks)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<Build?> GetBuildForUserAsync(int userId, int buildId)
    {
        return await _db.Builds
            .Where(b => b.UserId == userId && b.Id == buildId)
            .Include(b => b.BuildParts)
                .ThenInclude(bp => bp.Part)
                    .ThenInclude(p => p.PartCategory)
            .Include(b => b.Benchmarks)
            .FirstOrDefaultAsync();
    }

    public async Task<Build?> CreateBuildAsync(
        int userId,
        string name,
        string? description,
        IReadOnlyCollection<(int partId, int quantity)> parts
    )
    {
        // Validate that all referenced parts exist
        var partIds = parts.Select(p => p.partId).Distinct().ToList();
        var existingPartIds = await _db.Parts
            .Where(p => partIds.Contains(p.Id))
            .Select(p => p.Id)
            .ToListAsync();

        if (existingPartIds.Count != partIds.Count)
        {
            return null;
        }

        var build = new Build
        {
            UserId = userId,
            Name = name,
            Description = description,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var p in parts)
        {
            build.BuildParts.Add(new BuildPart
            {
                PartId = p.partId,
                Quantity = p.quantity
            });
        }

        _db.Builds.Add(build);
        await _db.SaveChangesAsync();

        await _db.Entry(build)
            .Collection(b => b.BuildParts)
            .LoadAsync();
        await _db.Entry(build)
            .Collection(b => b.Benchmarks)
            .LoadAsync();
        foreach (var bp in build.BuildParts)
        {
            await _db.Entry(bp)
                .Reference(x => x.Part)
                .LoadAsync();
            await _db.Entry(bp.Part)
                .Reference(x => x.PartCategory)
                .LoadAsync();
        }

        return build;
    }

    public async Task<bool> UpdateBuildAsync(
        int userId,
        int buildId,
        string name,
        string? description,
        IReadOnlyCollection<(int partId, int quantity)> parts
    )
    {
        var build = await _db.Builds
            .Where(b => b.UserId == userId && b.Id == buildId)
            .Include(b => b.BuildParts)
            .FirstOrDefaultAsync();

        if (build == null)
        {
            return false;
        }

        // Validate parts
        var partIds = parts.Select(p => p.partId).Distinct().ToList();
        var existingPartIds = await _db.Parts
            .Where(p => partIds.Contains(p.Id))
            .Select(p => p.Id)
            .ToListAsync();

        if (existingPartIds.Count != partIds.Count)
        {
            return false;
        }

        build.Name = name;
        build.Description = description;

        // Replace BuildParts
        _db.BuildParts.RemoveRange(build.BuildParts);
        build.BuildParts.Clear();

        foreach (var p in parts)
        {
            build.BuildParts.Add(new BuildPart
            {
                BuildId = build.Id,
                PartId = p.partId,
                Quantity = p.quantity
            });
        }

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteBuildAsync(int userId, int buildId)
    {
        var build = await _db.Builds
            .Where(b => b.UserId == userId && b.Id == buildId)
            .FirstOrDefaultAsync();

        if (build == null)
        {
            return false;
        }

        _db.Builds.Remove(build);
        await _db.SaveChangesAsync();
        return true;
    }
}
