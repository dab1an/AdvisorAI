using Microsoft.AspNetCore.Mvc;
using server.Models;
using server.Services;

namespace server.Controllers;

[Route("api/Audit")]
[ApiController]
public class AuditController(IAuditParserService auditParserService) : ControllerBase
{
    [HttpPost("upload")]
    public ActionResult<IEnumerable<AuditSection>> GetAuditInfo(IFormFile file)
    {
        Stream stream = file.OpenReadStream();
        IEnumerable<AuditSection> auditRequirements = auditParserService.Parse(stream);
        return Ok(auditRequirements);
    }
}