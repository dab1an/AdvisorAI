using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace server.Services;

public class ChatService : IChatService
{
    private readonly IChatCompletionService _chat;

    public ChatService(IConfiguration configuration)
    {
        string apiKey = configuration["OPENAI_API_KEY"]
            ?? throw new InvalidOperationException("OPENAI_API_KEY is not configured.");

        Kernel kernel = Kernel.CreateBuilder()
            .AddOpenAIChatCompletion("gpt-4o", apiKey)
            .Build();

        _chat = kernel.GetRequiredService<IChatCompletionService>();
    }

    public async Task<string> GetResponseAsync(string userMessage, IFormFile? file = null)
    {
        ChatHistory history = new ChatHistory();
        ChatMessageContentItemCollection items = new ChatMessageContentItemCollection { new TextContent(userMessage) };

        if (file is not null)
        {
            if (file.ContentType is "image/png" or "image/jpeg")
            {
                using MemoryStream ms = new MemoryStream();
                await file.CopyToAsync(ms);
                items.Add(new ImageContent(ms.ToArray(), file.ContentType));
            }
            else if (file.ContentType is "application/pdf")
            {
                string pdfText = ExtractPdfText(file);
                items.Add(new TextContent($"\n\n--- Attached PDF content ---\n{pdfText}"));
            }
        }

        history.AddUserMessage(items);

        ChatMessageContent result = await _chat.GetChatMessageContentAsync(history);
        return result.Content ?? "Sorry, I couldn't generate a response.";
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