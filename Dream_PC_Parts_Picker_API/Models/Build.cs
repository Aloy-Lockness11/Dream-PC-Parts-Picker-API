namespace Dream_PC_Parts_Picker_API.Models;
/// Represents a PC build created by a user, containing various parts and benchmarks.
public class Build
{
    //Key for the Build 
    public int Id { get; set; }

    //User who created the Build
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    //Build Information
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    //Timestamp for when the Build was created
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    //Parts included in the Build and associated Benchmarks
    public ICollection<BuildPart> BuildParts { get; set; } = new List<BuildPart>();
    public ICollection<BuildBenchmark> Benchmarks { get; set; } = new List<BuildBenchmark>();
}