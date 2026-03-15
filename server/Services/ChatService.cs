using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using server.Models;
using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace server.Services;

public class ChatService : IChatService
{
    private readonly IChatCompletionService _chat;
    private readonly IChatHistoryStore _historyStore;
    private readonly IEmbeddingService _embeddingService;
    private readonly IAuditParserService _auditParserService;

    private const string SystemPrompt = """
        ROLE:
        You are an automated academic advisor for Florida International University (FIU).

        TASK:
        Answer student questions about semester planning using their uploaded records.
        Recommend the most relevant next courses and planning actions.

        CONTEXT:
        You will receive:
        1) user_query: the student's question
        2) parsed_flowchart: extracted degree flowchart data
        3) parsed_degree_audit: extracted Panther Degree Audit data

        The student expects clear guidance based on completed courses, prerequisites, and remaining requirements.

        CONSTRAINTS:
        - Be professional, direct, and student-friendly.
        - Explain recommendations clearly and briefly.
        - Keep summary concise (2-4 sentences).
        - Keep bullet points short and actionable (2-5 items).
        - If flowchart or audit data is missing/insufficient, set status="needs_more_info".
        - In needs_more_info, include clear next_steps explaining what the student should upload or clarify.
        - If status is success, recommended_courses should be ordered by relevance.
        - Never output null for arrays; use [].
        - opening_message should be present and student-friendly.
        - closing_message should thank the student and invite follow-up feedback.

        OUTPUT FORMAT:
        Return only valid JSON with this schema:
        {
          "status": "success | needs_more_info | error",
          "opening_message": "string",
          "summary_paragraph": "string",
          "bullet_points": ["string"],
          "recommended_courses": [
            {
              "code": "string",
              "title": "string",
              "credits": integer,
              "reason": "string"
            }
          ],
          "warnings": ["string"],
          "next_steps": ["string"],
          "closing_message": "string"
        }
        """;

    public ChatService(
        IConfiguration configuration,
        IChatHistoryStore historyStore,
        IEmbeddingService embeddingService,
        IAuditParserService auditParserService)
    {
        string apiKey = configuration["OPENAI_API_KEY"]
            ?? throw new InvalidOperationException("OPENAI_API_KEY is not configured.");

        Kernel kernel = Kernel.CreateBuilder()
            .AddOpenAIChatCompletion("gpt-4o", apiKey)
            .Build();

        _chat = kernel.GetRequiredService<IChatCompletionService>();
        _historyStore = historyStore;
        _embeddingService = embeddingService;
        _auditParserService = auditParserService;
    }

    public async Task<string> GetResponseAsync(
        string conversationId,
        string userMessage,
        IFormFile? file = null,
        string? fileType = null)
    {
        ChatHistory history = await _historyStore.GetHistoryAsync(conversationId);

        if (!history.Any())
            history.AddSystemMessage(SystemPrompt);

        IEnumerable<string> relevantChunks = await _embeddingService.QueryAsync(userMessage);

        StringBuilder contextBuilder = new StringBuilder();
        contextBuilder.AppendLine("--- Relevant Degree Requirements ---");
        foreach (string chunk in relevantChunks)
        {
            contextBuilder.AppendLine(chunk);
            contextBuilder.AppendLine();
        }

        if (file is not null)
        {
            if (file.ContentType is "image/png" or "image/jpeg")
            {
                using MemoryStream ms = new MemoryStream();
                await file.CopyToAsync(ms);

                ChatMessageContentItemCollection imageItems = new ChatMessageContentItemCollection
                {
                    new TextContent($"{contextBuilder}\n\nStudent question: {userMessage}"),
                    new ImageContent(ms.ToArray(), file.ContentType)
                };

                history.AddUserMessage(imageItems);
                return await GetAndSaveResponseAsync(conversationId, history);
            }

            if (file.ContentType is "application/pdf")
            {
                string pdfContext;

                if (fileType == "audit")
                {
                    using Stream auditStream = file.OpenReadStream();
                    Audit auditContent = _auditParserService.Parse(auditStream);
                    pdfContext = auditContent.ToString();
                }
                else
                {
                    pdfContext = $"--- Attached Document ---\n{ExtractPdfText(file)}";
                }

                {
                    string fullMessage = $"{contextBuilder}\n\n{pdfContext}\n\nStudent question: {userMessage}";
                    history.AddUserMessage(fullMessage);
                    return await GetAndSaveResponseAsync(conversationId, history);
                }
            }
        }

        string messageWithContext = $"{contextBuilder}\n\nStudent question: {userMessage}";
        history.AddUserMessage(messageWithContext);
        return await GetAndSaveResponseAsync(conversationId, history);
    }

    private async Task<string> GetAndSaveResponseAsync(string conversationId, ChatHistory history)
    {
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