using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using server.Services;

namespace server.Tests.Services;

[TestClass]
public class ServiceConfigurationGuardTests
{
    [TestMethod]
    public void ChatService_Constructor_Throws_WhenOpenAiKeyMissing()
    {
        IConfiguration config = BuildConfiguration(new Dictionary<string, string?>
        {
            ["OPENAI_API_KEY"] = null
        });

        Mock<IChatHistoryStore> historyStoreMock = new Mock<IChatHistoryStore>();
        Mock<IEmbeddingService> embeddingServiceMock = new Mock<IEmbeddingService>();
        Mock<IAuditParserService> auditParserServiceMock = new Mock<IAuditParserService>();

        InvalidOperationException ex = Assert.ThrowsException<InvalidOperationException>(() =>
            new ChatService(
                config,
                historyStoreMock.Object,
                embeddingServiceMock.Object,
                auditParserServiceMock.Object));

        StringAssert.Contains(ex.Message, "OPENAI_API_KEY is not configured.");
    }

    [TestMethod]
    public void FlowchartParserService_Constructor_Throws_WhenOpenAiKeyMissing()
    {
        IConfiguration config = BuildConfiguration(new Dictionary<string, string?>
        {
            ["OPENAI_API_KEY"] = null
        });

        InvalidOperationException ex = Assert.ThrowsException<InvalidOperationException>(() =>
            new FlowchartParserService(config));

        StringAssert.Contains(ex.Message, "OPENAI_API_KEY is not configured.");
    }

    [TestMethod]
    public void RedisChatHistoryStore_Constructor_Throws_WhenRedisConnectionMissing()
    {
        IConfiguration config = BuildConfiguration(new Dictionary<string, string?>
        {
            ["REDIS_CONNECTION"] = null
        });

        InvalidOperationException ex = Assert.ThrowsException<InvalidOperationException>(() =>
            new RedisChatHistoryStore(config));

        StringAssert.Contains(ex.Message, "REDIS_CONNECTION is not configured.");
    }

    private static IConfiguration BuildConfiguration(Dictionary<string, string?> values)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
    }
}