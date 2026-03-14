using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace server.Services;

public class ChatService : IChatService
{
    private readonly IChatCompletionService _chat;
    private readonly IChatHistoryStore _historyStore;

    public ChatService(IConfiguration configuration, IChatHistoryStore historyStore)
    {
        string apiKey = configuration["OPENAI_API_KEY"]
            ?? throw new InvalidOperationException("OPENAI_API_KEY is not configured.");

        Kernel kernel = Kernel.CreateBuilder()
            .AddOpenAIChatCompletion("gpt-4o", apiKey)
            .Build();

        _chat = kernel.GetRequiredService<IChatCompletionService>();
        _historyStore = historyStore;
    }

    public async Task<string> GetResponseAsync(string conversationId, string userMessage, IFormFile? file = null)
    {
        ChatHistory history = await _historyStore.GetHistoryAsync(conversationId);

        string fullUserText = userMessage;

        if (file is not null)
        {
            if (file.ContentType is "image/png" or "image/jpeg")
            {
                using MemoryStream ms = new MemoryStream();
                await file.CopyToAsync(ms);

                // send image to the model for this request
                ChatMessageContentItemCollection items = new ChatMessageContentItemCollection
                {
                    new TextContent(userMessage),
                    new ImageContent(ms.ToArray(), file.ContentType)
                };
                history.AddUserMessage(items);
            }
            else if (file.ContentType is "application/pdf")
            {
                string pdfText = ExtractPdfText(file);
                fullUserText = $"{userMessage}\n\n--- Attached PDF content ---\n{pdfText}";
                history.AddUserMessage(fullUserText);
            }
            else
            {
                history.AddUserMessage(userMessage);
            }
        }
        else
        {
            history.AddUserMessage(userMessage);
        }

        ChatMessageContent result = await _chat.GetChatMessageContentAsync(history);
        string responseText = result.Content ?? "Sorry, I couldn't generate a response.";

        history.AddAssistantMessage(responseText);
        await _historyStore.SaveHistoryAsync(conversationId, history);

        return responseText;
    }

    // this will be a placeholder until we settle on how to handle detecting that a pdf is specifically an audit 
    private static string ExtractPdfText(IFormFile file)
    {
        using Stream stream = file.OpenReadStream();
        using PdfDocument pdf = PdfDocument.Open(stream);
        return string.Join("\n", pdf.GetPages()
            .Select(p => ContentOrderTextExtractor.GetText(p)));
    }
}