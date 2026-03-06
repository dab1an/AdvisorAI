using server.Models;
namespace server.Services;

public interface IEmbeddingService
{
    Task UpsertAuditAsync(List<AuditSection> sections, string studentId);
    Task UpsertFlowchartAsync(FlowchartCurriculum curriculum);
}