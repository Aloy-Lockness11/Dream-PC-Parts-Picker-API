namespace Dream_PC_Parts_Picker_API.Models;

public class BuildBenchmark
{
    // Key for the BuildBenchmark
    public int Id { get; set; }

    // Build being benchmarked
    public int BuildId { get; set; }
    public Build Build { get; set; } = null!;

    // User who ran the Benchmark
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    // Benchmark Scores
    public int OverallScore { get; set; }
    public int? CpuScore { get; set; }
    public int? GpuScore { get; set; }

    // Additional Notes
    public string? Notes { get; set; }

    // Timestamp for when the Benchmark was created
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}