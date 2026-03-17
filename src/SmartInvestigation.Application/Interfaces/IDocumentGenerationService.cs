namespace SmartInvestigation.Application.Interfaces;

public interface IDocumentGenerationService
{
    Task<string> GenerateDraftDocumentAsync(string documentType, object contextData, CancellationToken cancellationToken = default);
}
