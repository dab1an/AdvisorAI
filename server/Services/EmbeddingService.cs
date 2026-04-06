using Pinecone;
using server.Models;
using System.Text.Json;
using Microsoft.SemanticKernel.Embeddings;

namespace server.Services;

public class EmbeddingService(
    ITextEmbeddingGenerationService embeddingGenerator,
    PineconeClient pinecone) : IEmbeddingService
{
    private const string IndexName = "advisor-ai";

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

            IList<ReadOnlyMemory<float>> embeddings = await embeddingGenerator.GenerateEmbeddingsAsync([chunkText]);
            float[] vector = embeddings[0].ToArray();
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
    
    public async Task<IEnumerable<string>> QueryAsync(string question, int topK = 5)
    {
        var index = pinecone.Index(IndexName);

        IList<ReadOnlyMemory<float>> embeddings = await embeddingGenerator.GenerateEmbeddingsAsync([question]);
        float[] questionVector = embeddings[0].ToArray();

        QueryResponse response = await index.QueryAsync(new QueryRequest
        {
            Vector = questionVector,
            TopK = (uint)topK,
            IncludeMetadata = true
        });

        return response.Matches
            .Where(m => m.Metadata != null && m.Metadata.ContainsKey("text"))
            .Select(m => m.Metadata!["text"].ToString()!);
    }
}