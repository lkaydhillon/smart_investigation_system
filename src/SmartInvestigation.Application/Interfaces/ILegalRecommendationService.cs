namespace SmartInvestigation.Application.Interfaces;

public interface ILegalRecommendationService
{
    Task<List<string>> RecommendLegalSectionsAsync(string caseDescription, CancellationToken cancellationToken = default);
}
