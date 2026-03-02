namespace server.Models;

public class FlowchartElectiveGroup
{
    public string Name { get; set; } = "";
    
    public int? GroupMinimumRequired { get; set; }
    
    public List<FlowchartElectiveCourse> Courses { get; set; } = [];
}