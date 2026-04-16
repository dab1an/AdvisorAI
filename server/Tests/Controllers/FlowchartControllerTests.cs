using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using server.Controllers;
using server.Models;
using server.Services;

namespace server.Tests.Controllers;

[TestClass]
public class FlowchartControllerTests
{
    [TestMethod]
    public async Task UploadFlowchartInfoAsync_ReturnsBadRequest_ForNonImageFiles()
    {
        Mock<IFlowchartParserService> parserMock = new Mock<IFlowchartParserService>();
        Mock<IEmbeddingService> embeddingMock = new Mock<IEmbeddingService>();
        FlowchartController controller = new FlowchartController(parserMock.Object, embeddingMock.Object);

        List<IFormFile> files = new List<IFormFile>
        {
            CreateFile("text/plain", new byte[] { 1, 2, 3 }, "flowchart.txt")
        };

        ActionResult<FlowchartCurriculum> result = await controller.UploadFlowchartInfoAsync(files);

        Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        parserMock.Verify(service => service.ParseFlowchartAsync(It.IsAny<List<(byte[] bytes, string mimeType)>>()), Times.Never);
        embeddingMock.Verify(service => service.UpsertFlowchartAsync(It.IsAny<FlowchartCurriculum>()), Times.Never);
    }

    [TestMethod]
    public async Task UploadFlowchartInfoAsync_ReturnsOkResult_ForImageFiles()
    {
        Mock<IFlowchartParserService> parserMock = new Mock<IFlowchartParserService>();
        Mock<IEmbeddingService> embeddingMock = new Mock<IEmbeddingService>();

        FlowchartCurriculum curriculum = new FlowchartCurriculum { Program = "CS" };
        parserMock
            .Setup(service => service.ParseFlowchartAsync(It.IsAny<List<(byte[] bytes, string mimeType)>>()))
            .ReturnsAsync(curriculum);

        embeddingMock
            .Setup(service => service.UpsertFlowchartAsync(curriculum))
            .Returns(Task.CompletedTask);

        FlowchartController controller = new FlowchartController(parserMock.Object, embeddingMock.Object);

        List<IFormFile> files = new List<IFormFile>
        {
            CreateFile("image/png", new byte[] { 137, 80, 78, 71 }, "flowchart.png")
        };

        ActionResult<FlowchartCurriculum> result = await controller.UploadFlowchartInfoAsync(files);

        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        Assert.AreSame(curriculum, ((OkObjectResult)result.Result!).Value);
        parserMock.Verify(service => service.ParseFlowchartAsync(It.IsAny<List<(byte[] bytes, string mimeType)>>()), Times.Once);
        embeddingMock.Verify(service => service.UpsertFlowchartAsync(curriculum), Times.Once);
    }

    private static IFormFile CreateFile(string contentType, byte[] bytes, string fileName)
    {
        MemoryStream stream = new MemoryStream(bytes);
        FormFile file = new FormFile(stream, 0, bytes.Length, "files", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };

        return file;
    }
}