using server.Models;
namespace server.Services;

public interface IEmbeddingService
{
    Task UpsertFlowchartAsync(FlowchartCurriculum curriculum);
    Task UpsertReferenceDocumentAsync(string fileName, string markdownContent);
    Task<IEnumerable<string>> QueryAsync(string question, int topK = 5);
    Task<bool> ReferenceDocumentExistsAsync(string fileName);
}