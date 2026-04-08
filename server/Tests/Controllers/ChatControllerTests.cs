using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using server.Controllers;
using server.Services;

namespace server.Tests.Controllers;

[TestClass]
public class ChatControllerTests
{
    [TestMethod]
    public async Task Chat_ReturnsBadRequest_WhenMessageIsEmpty()
    {
        Mock<IChatService> chatServiceMock = new Mock<IChatService>();
        ChatController controller = new ChatController(chatServiceMock.Object);

        ActionResult<string> result = await controller.Chat(string.Empty, "conversation-1");

        Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task Chat_ReturnsBadRequest_WhenConversationIdIsEmpty()
    {
        Mock<IChatService> chatServiceMock = new Mock<IChatService>();
        ChatController controller = new ChatController(chatServiceMock.Object);

        ActionResult<string> result = await controller.Chat("Hello", string.Empty);

        Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task Chat_ReturnsOkResult_WithWrappedResponse()
    {
        Mock<IChatService> chatServiceMock = new Mock<IChatService>();
        chatServiceMock
            .Setup(service => service.GetResponseAsync("conversation-1", "Hello", null, null))
            .ReturnsAsync("response text");

        ChatController controller = new ChatController(chatServiceMock.Object);

        ActionResult<string> result = await controller.Chat("Hello", "conversation-1");

        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));

        OkObjectResult okResult = (OkObjectResult)result.Result!;
        Assert.IsNotNull(okResult.Value);

        object value = okResult.Value;
        string response = (string)value.GetType().GetProperty("response")!.GetValue(value)!;

        Assert.AreEqual("response text", response);
        chatServiceMock.Verify(service => service.GetResponseAsync("conversation-1", "Hello", null, null), Times.Once);
    }
}