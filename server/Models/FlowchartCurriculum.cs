namespace server.Models;

public class FlowchartCurriculum
{
    public string Program { get; set; } = "";
    public List<FlowchartCourse> Courses { get; set; } = [];
    public FlowchartElectiveRules ElectiveRules { get; set; } = new();
}