using MediatR;
using SmartInvestigation.Application.Common;
using SmartInvestigation.Domain.Entities;
using SmartInvestigation.Domain.Interfaces;

namespace SmartInvestigation.Application.Features.Analytics;

public record GetCrimeStatsQuery(Guid? DistrictId, Guid? StationId) : IRequest<Result<object>>;

public class GetCrimeStatsHandler : IRequestHandler<GetCrimeStatsQuery, Result<object>>
{
    private readonly IUnitOfWork _uow;
    public GetCrimeStatsHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<object>> Handle(GetCrimeStatsQuery query, CancellationToken ct)
    {
        var stats = await _uow.Repository<CrimeStatistic>().GetAsync(
            predicate: s => (query.DistrictId == null || s.DistrictId == query.DistrictId) &&
                            (query.StationId == null || s.StationId == query.StationId),
            orderBy: q => q.OrderByDescending(s => s.Period),
            includeString: null,
            disableTracking: true, cancellationToken: ct);

        // Grouping logic for dashboard
        var summary = new
        {
            TotalCases = stats.Sum(s => s.TotalCases),
            SolvedCases = stats.Sum(s => s.SolvedCases),
            PendingCases = stats.Sum(s => s.PendingCases),
            AverageClearanceRate = stats.Any() ? stats.Average(s => s.ClearanceRate) : 0,
            Trends = stats.GroupBy(s => s.Period).Select(g => new
            {
                Period = g.Key,
                Total = g.Sum(x => x.TotalCases),
                Solved = g.Sum(x => x.SolvedCases)
            }).OrderBy(x => x.Period).ToList()
        };

        return Result<object>.Success(summary);
    }
}
