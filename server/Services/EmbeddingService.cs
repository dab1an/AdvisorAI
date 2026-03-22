using System.Text;
using Pinecone;
using server.Models;
using System.Text.Json;
using System.Text.RegularExpressions;
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

    public async Task UpsertReferenceDocumentAsync(string fileName, string markdownContent)
    {
        var index = pinecone.Index(IndexName);
        List<string> chunks = ChunkMarkdown(markdownContent);
        for (int i = 0; i < chunks.Count; i++)
        {
            string chunk = chunks[i];
            string sanitizedFileName =
                Regex.Replace(Path.GetFileNameWithoutExtension(fileName), @"[^a-zA-z0-9_-]", "_");
            string vectorId = $"ref_{sanitizedFileName}_{i}";
            IList<ReadOnlyMemory<float>> embeddings = await embeddingGenerator.GenerateEmbeddingsAsync([chunk]);
            float[] vector = embeddings[0].ToArray();
            await index.UpsertAsync(new UpsertRequest
                {
                    Vectors =
                    [
                        new Vector
                        {
                            Id = vectorId, Values = vector,
                            Metadata = new Metadata
                            {
                                ["documentType"] = "reference", ["source"] = fileName, ["chunkIndex"] = i.ToString(),
                                ["text"] = chunk
                            }
                        }
                    ]
                }
            );
        }
    }

    public async Task<bool> ReferenceDocumentExistsAsync(string fileName)
    {
        var index = pinecone.Index(IndexName);
        // ??
        string sanitizedFileName = Regex.Replace(Path.GetFileNameWithoutExtension(fileName), @"[^a-zA-z0-9_-]", "_");
        string firstChunkId = $"ref_{sanitizedFileName}_0";

        FetchResponse response = await index.FetchAsync(new FetchRequest { Ids = [firstChunkId] });
        return response.Vectors.ContainsKey(firstChunkId);
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

    private static List<string> ChunkMarkdown(string markdown)
    {
        List<string> chunks = new();
        string[] lines = markdown.Split('\n');
        StringBuilder currentChunk = new();
        string currentHeading = "";

        foreach (string line in lines)
        {
            bool isHeading = line.TrimStart().StartsWith('#');
            if (isHeading && currentChunk.Length > 0)
            {
                AddChunksWithSizeLimit(chunks, currentHeading, currentChunk.ToString());
                currentChunk.Clear();
                currentHeading = line.Trim();
            }
            else if (isHeading)
            {
                currentHeading = line.Trim();
            }

            currentChunk.AppendLine(line);
        }

        if (currentChunk.Length > 0)
        {
            AddChunksWithSizeLimit(chunks, currentHeading, currentChunk.ToString());
        }

        return chunks;
    }

    private static void AddChunksWithSizeLimit(List<string> chunks, string heading, string text)
    {
        const int maxChunkLength = 1500;
        string trimmed = text.Trim();
        if (trimmed.Length <= maxChunkLength)
        {
            chunks.Add(trimmed);
            return;
        }

        string[] paragraphs = Regex.Split(trimmed, @"\n\s*\n");
        StringBuilder buffer = new();
        if (!string.IsNullOrEmpty(heading))
        {
            buffer.AppendLine(heading);
        }

        foreach (string paragraph in paragraphs)
        {
            string para = paragraph.Trim();
            if (string.IsNullOrEmpty(para))
                continue;
            if (buffer.Length + para.Length > maxChunkLength && buffer.Length > 0)
            {
                chunks.Add(buffer.ToString().Trim());
                buffer.Clear();
                if (!string.IsNullOrEmpty(heading))
                {
                    buffer.AppendLine(heading);
                }
            }
            buffer.AppendLine(para);
            buffer.AppendLine();
        }

        if (buffer.Length > 0)
        {
            chunks.Add(buffer.ToString().Trim());
        }
    }
}

