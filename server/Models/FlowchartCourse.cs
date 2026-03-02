namespace server.Models;

public class FlowchartCourse
{
    public string Code { get; set; } = "";
    public string Title { get; set; } = "";
    public int Credits { get; set; } = 3;

    public object? Prerequisites { get; set; } // object? since JSON can be "{ and: [...] }" or "{ or: [...] }"
    public object? Corequisites { get; set; }

    public string Category { get; set; } = "";
    public string StandingRequirement { get; set; } = "";
    public string Notes { get; set; } = "";
}