using System.Text.RegularExpressions;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
using server.Models;
using UglyToad.PdfPig.Content;

namespace server.Services;

public class AuditParserService : IAuditParserService
{
    private readonly HashSet<string> MainHeadings = new()
    {
        "FIU UNIVERSITY CORE CURRICULUM",
        "UNDERGRADUATE REQUIREMENTS",
        "UNDERGRADUATE TOTAL HOURS AND GPA",
        "COLLEGE OF ENGINEERING AND COMPUTING",
        "BACHELOR OF SCIENCE IN COMPUTER SCIENCE",
        "MATHEMATICS MINOR"
    };

    private readonly HashSet<string> SubHeadings = new()
    {
        "FIRST YEAR EXPERIENCE",
        "COMMUNICATION",
        "HUMANITIES - GROUP ONE",
        "HUMANITIES - GROUP TWO",
        "MATHEMATICS",
        "SOCIAL SCIENCE",
        "NATURAL SCIENCES - GROUP ONE",
        "NATURAL SCIENCE - GROUP TWO",
        "ARTS",
        "GORDON RULE WITH WRITING (GRW) REQUIREMENT",
        "FLENT/FLEX",
        "SUMMER ENROLLMENT",
        "CIVICS REQUIREMENT",
        "GLOBAL LEARNING REQUIREMENT",
        "FIU CUM GPA",
        "TOTAL EARNED UNITS",
        "COMPUTER SCIENCE PREREQUISITES",
        "COMPUTER SCIENCE TRACK",
        "STATISTICS",
        "COMPUTER SCIENCE BS ELECTIVES",
        "MATHEMATICS MINOR REQUIRED"
    };

    private static readonly Regex CourseRegex = new(@"^[A-Z]{3}\d{4}");

    public Audit Parse(Stream pdfStream)
    {
        PdfDocument pdf = PdfDocument.Open(pdfStream);

        List<AuditSection> sections = new List<AuditSection>();

        AuditSection? currentSection = null;
        AuditRequirement? currentRequirement = null;

        foreach (Page page in pdf.GetPages())
        {
            string text = ContentOrderTextExtractor.GetText(page);

            IEnumerable<string> lines = text
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim());

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                // MAIN SECTION
                if (MainHeadings.Contains(line))
                {
                    currentSection = new AuditSection
                    {
                        Title = line
                    };

                    sections.Add(currentSection);
                    currentRequirement = null;
                    continue;
                }

                // SUB SECTION
                if (SubHeadings.Contains(line) && currentSection != null)
                {
                    currentRequirement = new AuditRequirement
                    {
                        Title = line
                    };

                    currentSection.Requirements.Add(currentRequirement);
                    continue;
                }

                // STATUS
                if (line.StartsWith("Status:", StringComparison.OrdinalIgnoreCase))
                {
                    bool satisfied = line.Contains("Satisfied", StringComparison.OrdinalIgnoreCase);

                    if (currentRequirement != null)
                        currentRequirement.IsSatisfied = satisfied;
                    else if (currentSection != null)
                        currentSection.IsSatisfied = satisfied;

                    continue;
                }

                // COURSE
                if (CourseRegex.IsMatch(line) && currentRequirement != null)
                {
                    Course? course = ParseCourse(line);

                    if (course != null)
                        currentRequirement.CoursesTaken.Add(course);

                    continue;
                }
            }
        }

        return new Audit { Sections = sections };
    }

    private static Course? ParseCourse(string line)
    {
        string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 2)
            return null;

        string code = parts[0];

        List<string> titleParts = new List<string>();

        for (int i = 1; i < parts.Length; i++)
        {
            if (double.TryParse(parts[i], out _))
                break;

            titleParts.Add(parts[i]);
        }

        return new Course
        {
            Code = code,
            Title = string.Join(" ", titleParts)
        };
    }
}