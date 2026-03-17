using Mapster;
using MediatR;
using SmartInvestigation.Application.Common;
using SmartInvestigation.Domain.Entities;
using SmartInvestigation.Domain.Interfaces;

namespace SmartInvestigation.Application.Features.Locations;

// ── Queries ──

public record GetStatesQuery() : IRequest<Result<List<StateDto>>>;
public record GetDistrictsQuery(Guid StateId) : IRequest<Result<List<DistrictDto>>>;
public record GetPoliceStationsQuery(Guid? DistrictId = null) : IRequest<Result<List<PoliceStationDto>>>;

// ── DTOs ──

public record StateDto(Guid Id, string Name, string Code);
public record DistrictDto(Guid Id, string Name, string Code, Guid StateId);
public record PoliceStationDto(Guid Id, string Name, string Code, Guid DistrictId,
    string? Address, string? ContactNumber, double? GpsLatitude, double? GpsLongitude);

// ── Handlers ──

public class GetStatesHandler : IRequestHandler<GetStatesQuery, Result<List<StateDto>>>
{
    private readonly IUnitOfWork _uow;
    public GetStatesHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<List<StateDto>>> Handle(GetStatesQuery query, CancellationToken ct)
    {
        var items = await _uow.Repository<State>().GetAsync(orderBy: q => q.OrderBy(s => s.Name), includeString: null, disableTracking: true, cancellationToken: ct);
        return Result<List<StateDto>>.Success(items.Adapt<List<StateDto>>());
    }
}

public class GetDistrictsHandler : IRequestHandler<GetDistrictsQuery, Result<List<DistrictDto>>>
{
    private readonly IUnitOfWork _uow;
    public GetDistrictsHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<List<DistrictDto>>> Handle(GetDistrictsQuery query, CancellationToken ct)
    {
        var items = await _uow.Repository<District>().GetAsync(
            predicate: d => d.StateId == query.StateId,
            orderBy: q => q.OrderBy(d => d.Name), includeString: null, disableTracking: true, cancellationToken: ct);
        return Result<List<DistrictDto>>.Success(items.Adapt<List<DistrictDto>>());
    }
}

public class GetPoliceStationsHandler : IRequestHandler<GetPoliceStationsQuery, Result<List<PoliceStationDto>>>
{
    private readonly IUnitOfWork _uow;
    public GetPoliceStationsHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<List<PoliceStationDto>>> Handle(GetPoliceStationsQuery query, CancellationToken ct)
    {
        var items = await _uow.Repository<PoliceStation>().GetAsync(
            predicate: query.DistrictId.HasValue ? p => p.DistrictId == query.DistrictId.Value : null,
            orderBy: q => q.OrderBy(p => p.Name), includeString: null, disableTracking: true, cancellationToken: ct);
        return Result<List<PoliceStationDto>>.Success(items.Adapt<List<PoliceStationDto>>());
    }
}
