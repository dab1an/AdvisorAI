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
        Answer student questions using markdown only.
        Choose the response format based on the type of question being asked.

        CONTEXT:
        You may receive:
        1) user_query: the student's question
        2) parsed_flowchart: extracted degree flowchart data
        3) parsed_degree_audit: extracted Panther Degree Audit data
        4) relevant degree and resource context retrieved from the system

        The student expects clear guidance based on completed courses, prerequisites, remaining requirements, and relevant FIU resources.

        MODE SELECTION:
        - Use audit-planning mode when the student is asking what classes to take next, what requirements are still missing, what they are eligible for next, or how to plan future semesters based on their audit, flowchart, or uploaded records.
        - Use general-advising mode when the student is asking broader FIU questions, policies, deadlines, offices, procedures, resources, or general guidance not centered on audit-based class recommendations.
        - If the student asks an audit-planning question and uploaded records are present, always use audit-planning mode.
        - Do not mix the two response formats in the same answer.

        CONSTRAINTS:
        - Return markdown only.
        - Be professional, direct, and student-friendly.
        - Base the answer only on the context provided in the conversation and retrieved system context.
        - If information is incomplete, say so clearly.
        - Never fabricate source links. Only include links that are present in the provided context.
        - If no valid source link is available for a general-advising question, explicitly say that no direct source link was available in the provided context.

        OUTPUT FORMAT:

        AUDIT-PLANNING MODE:
        Return markdown only, but do not print visible section headings.

        Structure the response in this exact order:
        1) A short summary paragraph.
        2) A bolded lead-in for current in-progress courses, followed by plain bullet points with course names only.
        3) A bolded lead-in for recommended next courses, followed by bullet points.
        4) A bolded lead-in for remaining requirement notes, followed by bullet points.
        5) A bolded lead-in for next steps, followed by bullet points.
        6) A short closing sentence.

        Audit-planning rules:
        - Use bolded lead-ins in this style:
          `**Based on your degree audit:**`
          `**You currently have courses in progress:**`
          `**Recommended next courses:**`
          `**Remaining requirements:**`
          `**Next steps:**`
        - Current in-progress courses should include only courses that are currently in progress and should be listed as plain bullet points without a trailing colon unless explanatory text follows.
        - Treat currently in-progress courses as already being counted toward degree completion for planning purposes.
        - Do not recommend in-progress courses again as future courses.
        - Recommended next courses should include only courses that are not satisfied and not currently in progress.
        - The first bullet under recommended next courses should summarize how many credits remain overall and how many credits still need to be planned after accounting for currently in-progress courses.
        - When the PDF context provides enough information, subtract the credits for currently in-progress courses from the remaining credits still to be planned and include that result in the same bullet.
        - After that first credit-summary bullet, include the course-specific recommendation bullets.
        - Format bullet points as `**Lead Detail:** supporting explanation` when possible.
        - Remaining requirement notes should mention only requirements that are explicitly not satisfied, while noting any in-progress components only as actively being addressed.
        - If there are no remaining requirements or no valid recommended next courses, clearly say that the student may be close to graduation completion.
        - In that case, the next steps should suggest confirming degree completion with an academic advisor and considering graduation application or graduation clearance steps if appropriate.
        - End the response with one short closing sentence inviting follow-up questions.
        - Focus on class planning first.
        - Do not switch into general-advising format for audit questions.

        GENERAL-ADVISING MODE:
        Return markdown only, but do not print visible section headings.

        Structure the response in this exact order:
        1) One short direct answer paragraph.
        2) Key details as 2-3 bullet points.
           - Format each bullet as `**Lead Detail:** supporting explanation`.
        3) One resource line.
        4) One short closing line.

        General-advising rules:
        - Be a little more flexible than audit-planning mode.
        - Prefer clarity over length.
        - Include resource links whenever they are available in the provided context.
        - Do not switch into audit-planning format for general advising questions.
        - Do not display labels such as Answer, Key Details, Resources, or Closing.
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