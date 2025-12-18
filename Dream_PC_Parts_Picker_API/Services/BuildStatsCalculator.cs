using Dream_PC_Parts_Picker_API.Models;

namespace Dream_PC_Parts_Picker_API.Services;

public record BuildTotals(
    decimal TotalPrice,
    int TotalTdpWatts,
    int TotalPerformanceScore
);

/// <summary>
/// Service for calculating build statistics.
/// </summary>
public class BuildStatsCalculator
{
    /// <summary>
    /// Computes the total price, TDP, and performance score for a given build.
    /// </summary>
    /// <param name="build"></param>
    /// <returns></returns>
    public BuildTotals ComputeTotals(Build build)
    {
        if (build.BuildParts == null || build.BuildParts.Count == 0)
        {
            return new BuildTotals(0m, 0, 0);
        }

        decimal totalPrice = 0m;
        var totalTdp = 0;
        var totalPerf = 0;

        foreach (var bp in build.BuildParts)
        {
            var qty = bp.Quantity;

            var price = bp.Part?.Price ?? 0m;
            var tdp = bp.Part?.TdpWatts ?? 0;
            var perf = bp.Part?.PerformanceScore ?? 0;

            totalPrice += price * qty;
            totalTdp += tdp * qty;
            totalPerf += perf * qty;
        }

        return new BuildTotals(totalPrice, totalTdp, totalPerf);
    }
}