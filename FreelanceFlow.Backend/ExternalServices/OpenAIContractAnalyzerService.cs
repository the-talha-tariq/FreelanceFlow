using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using FreelanceFlow.Backend.ExternalServices.Models;
using FreelanceFlow.Backend.Helpers;
using FreelanceFlow.Backend.Models.Enums;

namespace FreelanceFlow.Backend.ExternalServices;

public class OpenAIContractAnalyzerService : IOpenAIContractAnalyzerService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private const string SystemPrompt = """
        You are a contract risk analyzer for freelancers. Read the contract
        text and identify clauses matching ONLY these categories:
        NonCompete, AutoRenewal, IPOwnership, LiabilityWaiver, UnilateralTermination.

        Respond with ONLY a JSON array (no prose, no markdown code fences).
        Each element must have exactly these fields:
        {
          "clauseType": "<one of the five categories above, exact spelling>",
          "severity": "Low" | "Medium" | "High",
          "extractedText": "<verbatim excerpt from the contract, under 300 characters>",
          "explanation": "<why this is risky for a freelancer specifically, 1-2 sentences>",
          "suggestedAlternative": "<a fairer clause wording, 1-2 sentences>"
        }
        If no clauses in those categories are present, return an empty array: [].
        """;

    private readonly OpenAISettings _settings;

    public OpenAIContractAnalyzerService(IOptions<OpenAISettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task<ContractAnalysisResult> AnalyzeContractTextAsync(string contractText)
    {
        if (string.IsNullOrWhiteSpace(_settings.ApiKey))
        {
            return BuildMockResult();
        }

        try
        {
            var client = new ChatClient(_settings.Model, _settings.ApiKey);

            var completion = await client.CompleteChatAsync(
                new SystemChatMessage(SystemPrompt),
                new UserChatMessage(contractText));

            var rawResponse = completion.Value.Content.Count > 0
                ? completion.Value.Content[0].Text
                : "[]";

            var flags = ParseFlags(rawResponse);
            return new ContractAnalysisResult(flags, rawResponse);
        }
        catch (Exception ex)
        {
            // Bubble up as a plain message the service layer can surface to
            // the API caller without leaking SDK-internal exception types.
            throw new InvalidOperationException($"AI contract analysis failed: {ex.Message}", ex);
        }
    }

    private static List<ClauseFlag> ParseFlags(string rawJson)
    {
        var cleaned = StripMarkdownFence(rawJson);

        try
        {
            return JsonSerializer.Deserialize<List<ClauseFlag>>(cleaned, JsonOptions) ?? new List<ClauseFlag>();
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(
                $"AI response wasn't valid JSON and couldn't be parsed: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Some models wrap JSON in ```json ... ``` fences despite instructions
    /// not to. Strip those before parsing rather than failing outright.
    /// </summary>
    private static string StripMarkdownFence(string text)
    {
        var trimmed = text.Trim();
        if (trimmed.StartsWith("```"))
        {
            trimmed = trimmed.TrimStart('`');
            if (trimmed.StartsWith("json", StringComparison.OrdinalIgnoreCase))
            {
                trimmed = trimmed[4..];
            }
            var closingFenceIndex = trimmed.LastIndexOf("```", StringComparison.Ordinal);
            if (closingFenceIndex >= 0)
            {
                trimmed = trimmed[..closingFenceIndex];
            }
        }

        return trimmed.Trim();
    }

    private static ContractAnalysisResult BuildMockResult()
    {
        var mockFlags = new List<ClauseFlag>
        {
            new(
                ClauseType.AutoRenewal,
                Severity.Medium,
                "This Agreement shall automatically renew for successive one-year terms unless either party provides ninety (90) days written notice prior to the end of the then-current term.",
                "A 90-day notice window is easy to miss, risking an unwanted year-long renewal on the existing terms.",
                "Automatic renewal with a shorter 30-day notice period, or no automatic renewal clause at all."),
            new(
                ClauseType.IPOwnership,
                Severity.High,
                "All work product, including any pre-existing tools, libraries, and methodologies used by Contractor in performing the Services, shall be owned exclusively by Client.",
                "This assigns ownership of the freelancer's own pre-existing tools and code libraries to the client, not just the specific deliverable.",
                "Client owns the deliverable created under this agreement; Contractor retains ownership of pre-existing tools, frameworks, and general know-how.")
        };

        var mockRawResponse = JsonSerializer.Serialize(new
        {
            mock = true,
            reason = "OpenAI:ApiKey is not configured — returning sample data so the analyze/view flow can be tested end to end."
        });

        return new ContractAnalysisResult(mockFlags, mockRawResponse);
    }
}