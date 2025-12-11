namespace Dream_PC_Parts_Picker_API.Models;

public class User
{
    // Primary Key
    public int Id { get; set; }
    // Account Information
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    
    //Creation Timestamp
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Created Pc Builds and Benchmarks by User
    public ICollection<Build> Builds { get; set; } = new List<Build>();
    public ICollection<BuildBenchmark> Benchmarks { get; set; } = new List<BuildBenchmark>();
}
