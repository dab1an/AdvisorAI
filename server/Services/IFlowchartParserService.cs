using server.Models;

namespace server.Services;

public interface IFlowchartParserService
{
    Task<FlowchartCurriculum> ParseFlowchartAsync(
        List<(byte[] bytes, string mimeType)> images);
}