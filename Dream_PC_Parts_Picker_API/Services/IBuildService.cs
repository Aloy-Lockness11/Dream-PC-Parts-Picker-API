using Dream_PC_Parts_Picker_API.Models;

namespace Dream_PC_Parts_Picker_API.Services;

/// <summary>
/// d Service interface for managing user builds.
/// </summary>
public interface IBuildService
{
    Task<List<Build>> GetBuildsForUserAsync(int userId);
    Task<Build?> GetBuildForUserAsync(int userId, int buildId);

    Task<Build?> CreateBuildAsync(
        int userId,
        string name,
        string? description,
        IReadOnlyCollection<(int partId, int quantity)> parts
    );

    Task<bool> UpdateBuildAsync(
        int userId,
        int buildId,
        string name,
        string? description,
        IReadOnlyCollection<(int partId, int quantity)> parts
    );

    Task<bool> DeleteBuildAsync(int userId, int buildId);
}