using System.Text;

namespace server.Models;

public class Audit
{
    public List<AuditSection> Sections { get; set; } = new();

    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine("--- Student Audit ---");

        foreach (AuditSection section in Sections)
        {
            builder.AppendLine(
                $"Section: {section.Title} " +
                $"({(section.IsSatisfied ? "Satisfied" : "Not Satisfied")})");

            foreach (AuditRequirement requirement in section.Requirements)
            {
                builder.AppendLine(
                    $"  Requirement: {requirement.Title} " +
                    $"({(requirement.IsSatisfied ? "Satisfied" : "Not Satisfied")})");

                if (requirement.CoursesTaken.Any())
                {
                    string courses = string.Join(", ", requirement.CoursesTaken.Select(c => c.ToString()));
                    builder.AppendLine($"  Courses taken: {courses}");
                }

                if (requirement.InProgressCourses.Any())
                {
                    string inProgressCourses = string.Join(", ", requirement.InProgressCourses.Select(c => c.ToString()));
                    builder.AppendLine($"  In-Progress courses: {inProgressCourses}");
                }
            }
        }

        return builder.ToString();
    }
}