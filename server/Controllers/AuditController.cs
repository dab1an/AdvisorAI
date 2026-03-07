using Microsoft.AspNetCore.Mvc;
using server.Models;
using server.Services;

namespace server.Controllers;

[Route("api/Audit")]
[ApiController]
public class AuditController(IAuditParserService auditParserService, IEmbeddingService embeddingService) : ControllerBase
{
    [HttpPost("upload")]
    public async Task<ActionResult<IEnumerable<AuditSection>>> GetAuditInfo(IFormFile file, [FromQuery] string studentId)
    {
        if (string.IsNullOrWhiteSpace(studentId))
            return BadRequest("studentId is required");
        
        Stream stream = file.OpenReadStream();
        List<AuditSection> auditRequirements = auditParserService.Parse(stream);
        await embeddingService.UpsertAuditAsync(auditRequirements, studentId);
        return Ok(auditRequirements);
    }
}