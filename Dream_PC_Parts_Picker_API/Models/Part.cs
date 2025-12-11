namespace Dream_PC_Parts_Picker_API.Models;

public class Part
{
    // Primary Key
    public int Id { get; set; }

    // Foreign Key to PartCategory
    public int PartCategoryId { get; set; }
    public PartCategory PartCategory { get; set; } = null!;

    // Part Details
    public string Name { get; set; } = null!;
    public string Manufacturer { get; set; } = null!;
    public string? ModelNumber { get; set; }

    //Performance Metrics
    public decimal Price { get; set; }
    public int? PerformanceScore { get; set; }
    public int? TdpWatts { get; set; }
    
    // Date added
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Builds that include this Part
    public ICollection<BuildPart> BuildParts { get; set; } = new List<BuildPart>();
}