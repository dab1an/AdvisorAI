using Microsoft.AspNetCore.Mvc;
using server.Models;
using server.Services;

namespace server.Controllers;

[Route("api/flowchart")]
[ApiController]
public class FlowchartController(IFlowchartParserService flowchartParserService, IEmbeddingService embeddingService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<FlowchartCurriculum>> UploadFlowchartInfoAsync(
        List<IFormFile> files)
    {
        var images = new List<(byte[], string)>();

        foreach (var file in files)
        {
            if (!file.ContentType.StartsWith("image/"))
                return BadRequest("Only PNG or JPEG images are allowed.");

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);

            images.Add((ms.ToArray(), file.ContentType));
        }

        FlowchartCurriculum result = await flowchartParserService.ParseFlowchartAsync(images);
        await embeddingService.UpsertFlowchartAsync(result);
        return Ok(result);
    }
}