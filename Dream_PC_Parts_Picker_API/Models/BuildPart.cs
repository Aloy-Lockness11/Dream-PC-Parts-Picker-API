namespace Dream_PC_Parts_Picker_API.Models;

/// Join table between Build and Part Many-to-Many relationship contains quantity
public class BuildPart
{
    // Composite Key: BuildId + PartId
    public int BuildId { get; set; }
    public Build Build { get; set; } = null!;

    // Foreign Key to Part
    public int PartId { get; set; }
    public Part Part { get; set; } = null!;

    // Quantity of this Part in the Build
    public int Quantity { get; set; } = 1;
}