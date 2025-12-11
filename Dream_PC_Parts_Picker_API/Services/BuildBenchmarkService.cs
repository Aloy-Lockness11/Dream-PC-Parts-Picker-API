using Dream_PC_Parts_Picker_API.Data;
using Dream_PC_Parts_Picker_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Dream_PC_Parts_Picker_API.Services;

public class BuildBenchmarkService : IBuildBenchmarkService
{
    private readonly AppDbContext _db;

    public BuildBenchmarkService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<BuildBenchmark>> GetBenchmarksForBuildAsync(int userId, int buildId)
    {
        return await _db.BuildBenchmarks
            .Where(bb => bb.BuildId == buildId && bb.Build.UserId == userId)
            .OrderByDescending(bb => bb.CreatedAt)
            .ToListAsync();
    }

    public async Task<BuildBenchmark?> GetBenchmarkAsync(int userId, int benchmarkId)
    {
        return await _db.BuildBenchmarks
            .Include(bb => bb.Build)
            .Where(bb => bb.Id == benchmarkId && bb.Build.UserId == userId)
            .FirstOrDefaultAsync();
    }

    public async Task<BuildBenchmark?> CreateBenchmarkAsync(
        int userId,
        int buildId,
        int overallScore,
        int? cpuScore,
        int? gpuScore,
        string? notes
    )
    {
        var build = await _db.Builds
            .FirstOrDefaultAsync(b => b.Id == buildId && b.UserId == userId);

        if (build == null) return null;

        var benchmark = new BuildBenchmark
        {
            BuildId = buildId,
            UserId = userId,
            OverallScore = overallScore,
            CpuScore = cpuScore,
            GpuScore = gpuScore,
            Notes = notes,
            CreatedAt = DateTime.UtcNow
        };

        _db.BuildBenchmarks.Add(benchmark);
        await _db.SaveChangesAsync();

        return benchmark;
    }

    public async Task<bool> UpdateBenchmarkAsync(
        int userId,
        int benchmarkId,
        int overallScore,
        int? cpuScore,
        int? gpuScore,
        string? notes
    )
    {
        var benchmark = await _db.BuildBenchmarks
            .Include(bb => bb.Build)
            .FirstOrDefaultAsync(bb => bb.Id == benchmarkId && bb.Build.UserId == userId);

        if (benchmark == null) return false;

        benchmark.OverallScore = overallScore;
        benchmark.CpuScore = cpuScore;
        benchmark.GpuScore = gpuScore;
        benchmark.Notes = notes;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteBenchmarkAsync(int userId, int benchmarkId)
    {
        var benchmark = await _db.BuildBenchmarks
            .Include(bb => bb.Build)
            .FirstOrDefaultAsync(bb => bb.Id == benchmarkId && bb.Build.UserId == userId);

        if (benchmark == null) return false;

        _db.BuildBenchmarks.Remove(benchmark);
        await _db.SaveChangesAsync();
        return true;
    }
}
