using System.Text.RegularExpressions;
using server.Models;

namespace server.Services;

public interface IAuditParserService
{
    public List<AuditSection> Parse(Stream pdfStream);
}