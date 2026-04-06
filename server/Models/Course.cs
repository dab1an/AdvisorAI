namespace server.Models;

public class Course
{
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"{Code} {Title}";
    }
}