using server.Models;
namespace server.Services;

public interface IEmbeddingService
{
    Task UpsertFlowchartAsync(FlowchartCurriculum curriculum);
}