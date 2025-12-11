namespace Dream_PC_Parts_Picker_API.Models;

public class PartCategory
{
    //Parts Category Id
    public int Id { get; set; }
    
    //Parts ie CPU, GPU, RAM etc
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    //Parts in this Category
    public ICollection<Part> Parts { get; set; } = new List<Part>();
}