using Microsoft.SemanticKernel.ChatCompletion;

namespace server.Services;

public interface IChatHistoryStore
{
    Task<ChatHistory> GetHistoryAsync(string conversationId);
    Task SaveHistoryAsync(string conversationId, ChatHistory history);
}
