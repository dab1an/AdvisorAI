namespace server.Services;

public interface IChatService
{
    Task<string> GetResponseAsync(string userMessage, IFormFile? file = null);
}
