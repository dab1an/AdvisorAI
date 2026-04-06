namespace server.Services;

public interface IChatService
{
    Task<string> GetResponseAsync(string conversationId, string userMessage, IFormFile? file = null, string? fileType = null);
}
