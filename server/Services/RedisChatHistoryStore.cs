using System.Text.Json;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using StackExchange.Redis;

namespace server.Services;

public class RedisChatHistoryStore : IChatHistoryStore
{
    private readonly IDatabase _db;

    public RedisChatHistoryStore(IConfiguration configuration)
    {
        string connectionString = configuration["REDIS_CONNECTION"]
            ?? throw new InvalidOperationException("REDIS_CONNECTION is not configured.");

        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(connectionString);
        _db = redis.GetDatabase();
    }

    public async Task<ChatHistory> GetHistoryAsync(string conversationId)
    {
        string key = $"chat:{conversationId}";
        RedisValue data = await _db.StringGetAsync(key);

        ChatHistory history = new ChatHistory();

        if (data.IsNullOrEmpty)
            return history;

        List<ChatEntry>? entries = JsonSerializer.Deserialize<List<ChatEntry>>((string)data!);

        if (entries is null)
            return history;

        foreach (ChatEntry entry in entries)
        {
            if (entry.Role == "user")
                history.AddUserMessage(entry.Content);
            else if (entry.Role == "assistant")
                history.AddAssistantMessage(entry.Content);
        }

        return history;
    }

    public async Task SaveHistoryAsync(string conversationId, ChatHistory history)
    {
        string key = $"chat:{conversationId}";

        List<ChatEntry> entries = history
            .Where(m => m.Role == AuthorRole.User || m.Role == AuthorRole.Assistant)
            .Select(m => new ChatEntry
            {
                Role = m.Role == AuthorRole.User ? "user" : "assistant",
                Content = m.Content ?? ""
            })
            .ToList();

        string json = JsonSerializer.Serialize(entries);
        await _db.StringSetAsync(key, json, TimeSpan.FromHours(5)); //entries in cache expire after 5 hrs
    }

    private class ChatEntry
    {
        public string Role { get; set; } = "";
        public string Content { get; set; } = "";
    }
}
