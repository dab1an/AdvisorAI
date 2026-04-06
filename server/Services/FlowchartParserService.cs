using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Text;
using System.Text.Json;
using server.Models;

namespace server.Services;

public class FlowchartParserService : IFlowchartParserService
{
    private readonly Kernel _kernel;
    private readonly IChatCompletionService _chatService;

    private const string Model = "gpt-4o"; // vision-capable model

    public FlowchartParserService(IConfiguration configuration)
    {
        string apiKey = configuration["OPENAI_API_KEY"]
            ?? throw new InvalidOperationException("OPENAI_API_KEY is not configured.");
        
        IKernelBuilder builder = Kernel.CreateBuilder();
        builder.AddOpenAIChatCompletion(Model, apiKey, httpClient: new HttpClient
        {
            Timeout = TimeSpan.FromMinutes(5)
        });

        _kernel = builder.Build();
        _chatService = _kernel.GetRequiredService<IChatCompletionService>();
    }
    
    public async Task<FlowchartCurriculum> ParseFlowchartAsync(
        List<(byte[] bytes, string mimeType)> images)
    {
        ChatHistory chatHistory = new ChatHistory();
        chatHistory.AddSystemMessage(Prompt);

        ChatMessageContentItemCollection contentItems = new ChatMessageContentItemCollection
        {
            new TextContent("""
                            You are given multiple images that together form ONE degree flowchart.
                            Interpret them as a single continuous diagram.
                            Return only valid JSON.
                            """)
        };

        foreach (var image in images)
        {
            contentItems.Add(new ImageContent(image.bytes, image.mimeType));
        }

        chatHistory.AddUserMessage(contentItems);

        OpenAIPromptExecutionSettings settings = new OpenAIPromptExecutionSettings
        {
            Temperature = 0,
            ResponseFormat = "json_object"
        };

        ChatMessageContent result = await _chatService.GetChatMessageContentAsync(
            chatHistory,
            settings,
            _kernel
        );

        if (string.IsNullOrWhiteSpace(result.Content))
            throw new Exception("AI returned empty response.");

        JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        try
        {
            FlowchartCurriculum? curriculum = JsonSerializer.Deserialize<FlowchartCurriculum>(
                result.Content,
                options);

            if (curriculum == null)
                throw new Exception("Deserialization failed.");

            return curriculum;
        }
        catch (JsonException ex)
        {
            throw new Exception("AI returned invalid JSON.", ex);
        }
    }

    private const string Prompt = """
        You are a university curriculum extraction engine.

        You are given a degree flowchart PDF that contains:
        - Course boxes
        - Arrows indicating prerequisites
        - Diamonds indicating co-requisites
        - Grouped elective sections
        - Notes describing elective rules

        Your task is to convert the visual flowchart into structured JSON.

        Interpret the diagram as follows:

        1. Each course box represents a course.
        2. A downward arrow means:
           - The course above is a prerequisite for the course below.
        3. A diamond symbol indicates:
           - The connected course is a co-requisite.
        4. Grouped sections such as "Foundations", "Systems", or "CS Electives"
           represent elective categories.
        5. Notes describing rules (e.g., "Must take 7 electives" or
           "Must take at least one from each group") must be extracted.

        For each course, extract:
        - code (e.g., COP3530)
        - title
        - credits (default to 3 unless specified)
        - prerequisites (structured)
        - corequisites (structured)
        - category (Core Required, Foundations Elective, Systems Elective, etc.)
        - standing requirements (e.g., Senior standing, Junior standing)
        - special notes
        
        Note: Elective courses may only have prerequisites, not corequisites, in said case leave the array empty

        Return ONLY valid JSON in this schema:

        {
          "program": "",
          "courses": [
            {
              "code": "",
              "title": "",
              "credits": 3,
              "prerequisites": [],
              "corequisites": [],
              "category": "",
              "standingRequirement": "",
              "notes": ""
            }
          ],
          "electiveRules": {
          "totalElectivesRequired": null,
          "electiveGroups": [
            {
              "name": "",
              "groupMinimumRequired": null,
              "courses": [
                {
                  "name": "",
                  "prerequisites": [],
                  "corequisites": []
                }
              ]
            }
          ]
          }
        }

        Important rules:
        - Normalize course codes (uppercase, no spaces).
        - Convert prerequisites like:
            (A and B) → { "and": ["A","B"] }
            (A or B)  → { "or": ["A","B"] }
        - If prerequisite lists multiple acceptable courses separated by slash,
          return them as OR.
        - If prerequisite includes both AND and OR, preserve logical structure.
        - Do not invent missing prerequisites.
        - Do not summarize — extract structurally.
        - Return only JSON.
        
        CRITICAL RULE:
        Every visible course box in the diagram must be extracted as a course,
        even if:
        - It has no arrows
        - It has no prerequisites
        - It is labeled as an elective
        - It is a general education requirement
        - It appears visually separate from the CS flow (1a)
        
        If it is a box containing a course title, it must appear in the "courses" array.
        Do not omit standalone boxes.
        
        1a. Standalone boxes:
        Any box that is visually separate and has no arrows
        must still be extracted as a course if it represents
        a graduation requirement.
        Examples include:
        - Natural Science Group 1 Elective
        - Natural Science Group 2 Elective
        
        """;
}