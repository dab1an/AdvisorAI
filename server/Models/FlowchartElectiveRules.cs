namespace server.Models;

public class FlowchartElectiveRules
{
    public int? TotalElectivesRequired { get; set; }
    public List<FlowchartElectiveGroup> ElectiveGroups { get; set; } = [];
}