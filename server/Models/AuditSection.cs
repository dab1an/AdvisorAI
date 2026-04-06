namespace server.Models;

public class AuditSection
{
    public string Title { get; set; } = string.Empty;
    public bool IsSatisfied { get; set; }
    public List<AuditRequirement> Requirements { get; set; } = new();
}