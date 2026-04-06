using server.Models;
namespace server.Services;

public interface IEmbeddingService
{
    Task UpsertFlowchartAsync(FlowchartCurriculum curriculum);
    Task<IEnumerable<string>> QueryAsync(string question, int topK = 5);
}