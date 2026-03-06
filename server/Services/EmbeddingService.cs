using Microsoft.Extensions.AI;
using Pinecone;
using server.Models;
using System.Text.Json;

namespace server.Services;

public class EmbeddingService(
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
    PineconeClient pinecone) : IEmbeddingService
{
    private const string IndexName = "advisor-ai";

    public async Task UpsertAuditAsync(List<AuditSection> sections, string studentId)
    {
        var index = pinecone.Index(IndexName);

        foreach (var section in sections)
        {
            foreach (var req in section.Requirements)
            {
                string chunkText = BuildAuditChunkText(section, req);
                var result = await embeddingGenerator.GenerateAsync([chunkText]);
                var vector = result.First().Vector.ToArray();

                await index.UpsertAsync(new UpsertRequest
                {
                    Vectors =
                    [
                        new Vector
                        {
                            Id = $"{studentId}_{section.Title}_{req.Title}".Replace(" ", "_"),
                            Values = vector,
                            Metadata = new Metadata
                            {
                                ["studentId"] = studentId,
                                ["documentType"] = "audit",
                                ["section"] = section.Title,
                                ["requirement"] = req.Title,
                                ["isSatisfied"] = req.IsSatisfied.ToString(),
                                ["text"] = chunkText
                            }
                        }
                    ]
                });
            }
        }
    }
    public async Task UpsertFlowchartAsync(FlowchartCurriculum curriculum)
    {
        var index = pinecone.Index(IndexName);
        foreach (var course in curriculum.Courses)
        {
            string chunkText = $"Course: {course.Code} - {course.Title}\n" +
                               $"Credits: {course.Credits}\n" +
                               $"Category: {course.Category}\n" +
                               $"Prerequisites: {JsonSerializer.Serialize(course.Prerequisites)}\n" +
                               $"Notes: {course.Notes}";

            var result = await embeddingGenerator.GenerateAsync([chunkText]);
            var vector = result.First().Vector.ToArray();
            await index.UpsertAsync(new UpsertRequest
            {
                Vectors =
                [
                    new Vector
                    {
                        Id = $"flowchart_{course.Code}",
                        Values = vector,
                        Metadata = new Metadata
                        {
                            ["documentType"] = "flowchart",
                            ["program"] = curriculum.Program,
                            ["courseCode"] = course.Code,
                            ["category"] = course.Category,
                            ["text"] = chunkText
                        }
                    }
                ]
            });
        }
    }

    private static string BuildAuditChunkText(AuditSection section, AuditRequirement req)
    {
        var courses = string.Join(", ", req.CoursesTaken.Select(c => c.ToString()));
        return $"Section: {section.Title}\n" +
               $"Requirement: {req.Title}\n" +
               $"Status: {(req.IsSatisfied ? "Satisfied" : "Not Satisfied")}\n" +
               $"Courses taken: {(courses.Length > 0 ? courses : "None")}";
    }
}