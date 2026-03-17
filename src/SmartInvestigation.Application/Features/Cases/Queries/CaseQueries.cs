using Mapster;
using MediatR;
using SmartInvestigation.Application.Common;
using SmartInvestigation.Application.DTOs;
using SmartInvestigation.Domain.Entities;
using SmartInvestigation.Domain.Interfaces;

namespace SmartInvestigation.Application.Features.Cases.Queries;

// ── Get Case By Id ──
public record GetCaseByIdQuery(Guid Id) : IRequest<Result<CaseDto>>;

public class GetCaseByIdQueryHandler : IRequestHandler<GetCaseByIdQuery, Result<CaseDto>>
{
    private readonly IUnitOfWork _uow;
    public GetCaseByIdQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<CaseDto>> Handle(GetCaseByIdQuery query, CancellationToken ct)
    {
        var caseEntity = await _uow.Repository<Case>().GetByIdAsync(query.Id, ct);
        if (caseEntity == null) return Result<CaseDto>.NotFound("Case not found");
        return Result<CaseDto>.Success(caseEntity.Adapt<CaseDto>());
    }
}

// ── Get Paged Cases ──
public record GetCasesQuery(int PageNumber = 1, int PageSize = 20, string? Status = null,
    string? Search = null, Guid? OfficerId = null, Guid? StationId = null) : IRequest<Result<PagedResult<CaseDto>>>;

public class GetCasesQueryHandler : IRequestHandler<GetCasesQuery, Result<PagedResult<CaseDto>>>
{
    private readonly IUnitOfWork _uow;
    public GetCasesQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<PagedResult<CaseDto>>> Handle(GetCasesQuery query, CancellationToken ct)
    {
        var (items, totalCount) = await _uow.Repository<Case>().GetPagedAsync(
            query.PageNumber, query.PageSize,
            predicate: c =>
                (query.OfficerId == null || c.AssignedOfficerId == query.OfficerId) &&
                (query.StationId == null || c.PoliceStationId == query.StationId) &&
                (string.IsNullOrEmpty(query.Search) || c.CaseNumber.Contains(query.Search) || c.Title.Contains(query.Search)),
            orderBy: q => q.OrderByDescending(c => c.CreatedDate),
            disableTracking: true, cancellationToken: ct);

        var result = new PagedResult<CaseDto>
        {
            Items = items.Adapt<List<CaseDto>>(),
            TotalCount = totalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };

        return Result<PagedResult<CaseDto>>.Success(result);
    }
}

// ── Get Dashboard Stats ──
public record GetDashboardStatsQuery(Guid? OfficerId = null, Guid? StationId = null)
    : IRequest<Result<DashboardStatsDto>>;

public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, Result<DashboardStatsDto>>
{
    private readonly IUnitOfWork _uow;
    public GetDashboardStatsQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<DashboardStatsDto>> Handle(GetDashboardStatsQuery query, CancellationToken ct)
    {
        var caseRepo = _uow.Repository<Case>();
        var complaintRepo = _uow.Repository<Complaint>();

        var totalCases = await caseRepo.CountAsync(cancellationToken: ct);
        var pendingCases = await caseRepo.CountAsync(c =>
            c.Status == Domain.Enums.CaseStatus.UnderInvestigation ||
            c.Status == Domain.Enums.CaseStatus.PendingSupervision, ct);
        var solvedCases = await caseRepo.CountAsync(c =>
            c.Status == Domain.Enums.CaseStatus.ChargesheetFiled ||
            c.Status == Domain.Enums.CaseStatus.Convicted, ct);
        var totalComplaints = await complaintRepo.CountAsync(cancellationToken: ct);
        var pendingComplaints = await complaintRepo.CountAsync(c =>
            c.Status == Domain.Enums.ComplaintStatus.Received ||
            c.Status == Domain.Enums.ComplaintStatus.UnderReview, ct);

        var clearanceRate = totalCases > 0 ? (double)solvedCases / totalCases * 100 : 0;

        return Result<DashboardStatsDto>.Success(new DashboardStatsDto(
            totalCases, pendingCases, solvedCases,
            totalComplaints, pendingComplaints, 0, 0, Math.Round(clearanceRate, 2)));
    }
}
