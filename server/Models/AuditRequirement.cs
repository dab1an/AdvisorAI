namespace server.Models;

public class AuditRequirement
{
    public string Title { get; set; } = string.Empty;
    public bool IsSatisfied { get; set; }
    public List<Course> CoursesTaken { get; set; } = new();
}