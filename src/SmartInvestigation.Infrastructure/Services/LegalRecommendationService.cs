using System.Text.Json;
using SmartInvestigation.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace SmartInvestigation.Infrastructure.Services;

public class LegalRecommendationService : ILegalRecommendationService
{
    private readonly GeminiAIService _gemini;
    private readonly ILogger<LegalRecommendationService> _logger;

    public LegalRecommendationService(GeminiAIService gemini, ILogger<LegalRecommendationService> logger)
    {
        _gemini = gemini;
        _logger = logger;
    }

    public async Task<List<string>> RecommendLegalSectionsAsync(string caseDescription, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Calling Gemini AI to analyze FIR text for applicable BNS/IPC sections.");

        var prompt = $$"""
            You are a senior Indian Police legal advisor specializing in the Bharatiya Nyaya Sanhita (BNS) and IPC.
            Analyze the following FIR/case description and recommend the most applicable legal sections.

            Case Description:
            {{caseDescription}}

            Return ONLY a JSON array of objects with this structure (no markdown, no explanation, just raw JSON):
            [
              { "section": "103 BNS", "title": "Murder", "confidence": 0.92, "reasoning": "One line reason" },
              { "section": "115 BNS", "title": "Voluntarily causing hurt", "confidence": 0.78, "reasoning": "One line reason" }
            ]
            Provide 3-5 most relevant sections.
            """;

        var raw = await _gemini.GenerateAsync(prompt, cancellationToken);

        // Try to parse the AI's JSON response
        try
        {
            var trimmed = raw.Trim();
            // Extract JSON array if there's any extra text wrapping it
            var start = trimmed.IndexOf('[');
            var end = trimmed.LastIndexOf(']');
            if (start >= 0 && end > start)
                trimmed = trimmed.Substring(start, end - start + 1);

            var doc = JsonDocument.Parse(trimmed);
            var results = new List<string>();
            foreach (var item in doc.RootElement.EnumerateArray())
            {
                var section = item.GetProperty("section").GetString() ?? "?";
                var title = item.GetProperty("title").GetString() ?? "?";
                var confidence = item.TryGetProperty("confidence", out var conf) ? conf.GetDouble() : 0.5;
                var reasoning = item.TryGetProperty("reasoning", out var r) ? r.GetString() : "";
                results.Add($"{section} ({title}) — {Math.Round(confidence * 100)}% match. {reasoning}");
            }
            return results.Count > 0 ? results : Fallback();
        }
        catch
        {
            _logger.LogWarning("Failed to parse Gemini JSON response. Falling back.");
            return Fallback();
        }
    }

    private static List<string> Fallback() => new()
    {
        "IPC 504 (Intentional insult)",
        "IPC 506 (Criminal intimidation)",
        "IPC 34 (Acts done by several persons in furtherance of common intention)"
    };
}
