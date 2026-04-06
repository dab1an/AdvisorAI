namespace server.Models;

public class FlowchartElectiveCourse
{
    public string Name { get; set; } = "";

    public object? Prerequisites { get; set; }
    
    public object? Corequisites { get; set; }
}