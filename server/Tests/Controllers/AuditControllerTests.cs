using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using server.Controllers;
using server.Models;
using server.Services;

namespace server.Tests.Controllers;

[TestClass]
public class AuditControllerTests
{
    [TestMethod]
    public void GetAuditInfo_ReturnsOkResult_WithParsedAudit()
    {
        Mock<IAuditParserService> parserMock = new Mock<IAuditParserService>();
        Audit expectedAudit = new Audit();
        parserMock.Setup(service => service.Parse(It.IsAny<Stream>())).Returns(expectedAudit);

        AuditController controller = new AuditController(parserMock.Object);

        using MemoryStream stream = new MemoryStream(new byte[] { 1, 2, 3 });
        IFormFile file = new FormFile(stream, 0, stream.Length, "file", "audit.pdf");

        ActionResult<Audit> result = controller.GetAuditInfo(file);

        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));

        OkObjectResult okResult = (OkObjectResult)result.Result!;
        Assert.AreSame(expectedAudit, okResult.Value);
        parserMock.Verify(service => service.Parse(It.IsAny<Stream>()), Times.Once);
    }
}