using SmartInvestigation.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace SmartInvestigation.Infrastructure.Services;

public class DocumentGenerationService : IDocumentGenerationService
{
    private readonly GeminiAIService _gemini;
    private readonly ILogger<DocumentGenerationService> _logger;

    public DocumentGenerationService(GeminiAIService gemini, ILogger<DocumentGenerationService> logger)
    {
        _gemini = gemini;
        _logger = logger;
    }

    public async Task<string> GenerateDraftDocumentAsync(string documentType, object contextData, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating {DocumentType} via Gemini AI.", documentType);

        var contextJson = System.Text.Json.JsonSerializer.Serialize(contextData);

        var prompt = $$"""
            You are an expert Indian police officer and legal document drafter.
            Generate a complete, professional {{documentType}} in English suitable for Indian law enforcement.

            Case context (JSON):
            {{contextJson}}

            Requirements:
            - Use proper Indian legal document format
            - Include standard headings/sections for a {{documentType}}
            - Mention relevant BNS/IPC sections where appropriate
            - Keep it formal and concise (within 400 words)
            - Start with: --- {{documentType.ToUpperInvariant()}} DRAFT ---
            """;

        var draft = await _gemini.GenerateAsync(prompt, cancellationToken);

        if (string.IsNullOrWhiteSpace(draft))
        {
            return $"[AI GENERATED DRAFT: {documentType}]\n\nTo: The Station House Officer\n" +
                   $"Subject: {documentType} pertaining to the incident\n\n" +
                   $"Facts of the case:\n{contextJson}\n\n" +
                   $"Applicable Sections: [To be determined by IO]\n\n" +
                   $"Date: {DateTime.UtcNow:dd/MM/yyyy}\n--- END OF DRAFT ---";
        }

        return draft;
    }
}
