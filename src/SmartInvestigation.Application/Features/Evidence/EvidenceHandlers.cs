using Mapster;
using MediatR;
using SmartInvestigation.Application.Common;
using SmartInvestigation.Application.DTOs;
using SmartInvestigation.Domain.Entities;
using SmartInvestigation.Domain.Enums;
using SmartInvestigation.Domain.Interfaces;

namespace SmartInvestigation.Application.Features.Evidence;

// ── Commands ──

public record CreateEvidenceCommand(UploadEvidenceRequest Request, string UserId) : IRequest<Result<EvidenceSummaryDto>>;
public record UpdateEvidenceStatusCommand(Guid EvidenceId, EvidenceStatus Status, string? Remarks, string UserId) : IRequest<Result<bool>>;
public record AddChainOfCustodyCommand(Guid EvidenceId, string Action, string? StorageLocation, string? Remarks, string UserId) : IRequest<Result<bool>>;
public record CreateForensicLabRequestCommand(Guid CaseId, Guid EvidenceId, string LabName, string? RequestedAnalysis, string UserId) : IRequest<Result<Guid>>;

// ── Queries ──

public record GetEvidenceByCaseQuery(Guid CaseId, int PageNumber = 1, int PageSize = 20) : IRequest<Result<PagedResult<EvidenceSummaryDto>>>;
public record GetEvidenceByIdQuery(Guid Id) : IRequest<Result<EvidenceSummaryDto>>;
public record GetChainOfCustodyQuery(Guid EvidenceId) : IRequest<Result<List<ChainOfCustodyDto>>>;

// ── Additional DTOs ──

public record ChainOfCustodyDto(Guid Id, string Action, string? HandledBy, string? ReceivedBy,
    string? StorageLocation, string? Remarks, DateTime Timestamp);

// ── Handlers ──

public class CreateEvidenceHandler : IRequestHandler<CreateEvidenceCommand, Result<EvidenceSummaryDto>>
{
    private readonly IUnitOfWork _uow;
    public CreateEvidenceHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<EvidenceSummaryDto>> Handle(CreateEvidenceCommand cmd, CancellationToken ct)
    {
        var r = cmd.Request;
        var evidence = new Domain.Entities.Evidence
        {
            EvidenceNumber = $"EV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}",
            CaseId = r.CaseId,
            Type = r.Type,
            Description = r.Description,
            Status = EvidenceStatus.Collected,
            CollectedBy = r.CollectedBy,
            CollectionLocation = r.CollectionLocation,
            GpsLatitude = r.GpsLatitude,
            GpsLongitude = r.GpsLongitude,
            CollectionDate = DateTime.UtcNow,
            CategoryId = r.CategoryId,
            CreatedBy = cmd.UserId
        };

        await _uow.Repository<Domain.Entities.Evidence>().AddAsync(evidence, ct);

        // Initial chain of custody entry
        await _uow.Repository<EvidenceChainOfCustody>().AddAsync(new EvidenceChainOfCustody
        {
            EvidenceId = evidence.Id,
            Purpose = "Collected",
            ReceivedBy = cmd.UserId,
            ReceivedFrom = "Crime Scene",
            Timestamp = DateTime.UtcNow,
            Location = r.CollectionLocation,
            CreatedBy = cmd.UserId
        }, ct);

        await _uow.SaveChangesAsync(ct);
        return Result<EvidenceSummaryDto>.Created(evidence.Adapt<EvidenceSummaryDto>());
    }
}

public class UpdateEvidenceStatusHandler : IRequestHandler<UpdateEvidenceStatusCommand, Result<bool>>
{
    private readonly IUnitOfWork _uow;
    public UpdateEvidenceStatusHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<bool>> Handle(UpdateEvidenceStatusCommand cmd, CancellationToken ct)
    {
        var ev = await _uow.Repository<Domain.Entities.Evidence>().GetByIdAsync(cmd.EvidenceId, ct);
        if (ev == null) return Result<bool>.NotFound("Evidence not found");

        ev.Status = cmd.Status;
        ev.ModifiedBy = cmd.UserId;
        _uow.Repository<Domain.Entities.Evidence>().Update(ev);

        await _uow.Repository<EvidenceChainOfCustody>().AddAsync(new EvidenceChainOfCustody
        {
            EvidenceId = cmd.EvidenceId,
            Purpose = $"Status changed to {cmd.Status}",
            ReceivedBy = cmd.UserId,
            ReceivedFrom = "System",
            Timestamp = DateTime.UtcNow,
            Remarks = cmd.Remarks,
            CreatedBy = cmd.UserId
        }, ct);

        await _uow.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}

public class AddChainOfCustodyHandler : IRequestHandler<AddChainOfCustodyCommand, Result<bool>>
{
    private readonly IUnitOfWork _uow;
    public AddChainOfCustodyHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<bool>> Handle(AddChainOfCustodyCommand cmd, CancellationToken ct)
    {
        var exists = await _uow.Repository<Domain.Entities.Evidence>().ExistsAsync(e => e.Id == cmd.EvidenceId, ct);
        if (!exists) return Result<bool>.NotFound("Evidence not found");

        await _uow.Repository<EvidenceChainOfCustody>().AddAsync(new EvidenceChainOfCustody
        {
            EvidenceId = cmd.EvidenceId,
            Purpose = cmd.Action,
            ReceivedBy = cmd.UserId,
            ReceivedFrom = "Transfer",
            Location = cmd.StorageLocation,
            Remarks = cmd.Remarks,
            Timestamp = DateTime.UtcNow,
            CreatedBy = cmd.UserId
        }, ct);

        await _uow.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}

public class CreateForensicLabRequestHandler : IRequestHandler<CreateForensicLabRequestCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;
    public CreateForensicLabRequestHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<Guid>> Handle(CreateForensicLabRequestCommand cmd, CancellationToken ct)
    {
        var request = new ForensicLabRequest
        {
            CaseId = cmd.CaseId,
            EvidenceId = cmd.EvidenceId,
            LabName = cmd.LabName,
            RequestType = cmd.RequestedAnalysis,
            Status = ForensicRequestStatus.Pending,
            RequestDate = DateTime.UtcNow,
            RequestedBy = cmd.UserId,
            CreatedBy = cmd.UserId
        };

        await _uow.Repository<ForensicLabRequest>().AddAsync(request, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<Guid>.Created(request.Id);
    }
}

public class GetEvidenceByCaseHandler : IRequestHandler<GetEvidenceByCaseQuery, Result<PagedResult<EvidenceSummaryDto>>>
{
    private readonly IUnitOfWork _uow;
    public GetEvidenceByCaseHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<PagedResult<EvidenceSummaryDto>>> Handle(GetEvidenceByCaseQuery query, CancellationToken ct)
    {
        var (items, total) = await _uow.Repository<Domain.Entities.Evidence>().GetPagedAsync(
            query.PageNumber, query.PageSize,
            predicate: e => e.CaseId == query.CaseId,
            orderBy: q => q.OrderByDescending(e => e.CollectionDate),
            disableTracking: true, cancellationToken: ct);

        return Result<PagedResult<EvidenceSummaryDto>>.Success(new PagedResult<EvidenceSummaryDto>
        {
            Items = items.Adapt<List<EvidenceSummaryDto>>(),
            TotalCount = total, PageNumber = query.PageNumber, PageSize = query.PageSize
        });
    }
}

public class GetEvidenceByIdHandler : IRequestHandler<GetEvidenceByIdQuery, Result<EvidenceSummaryDto>>
{
    private readonly IUnitOfWork _uow;
    public GetEvidenceByIdHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<EvidenceSummaryDto>> Handle(GetEvidenceByIdQuery query, CancellationToken ct)
    {
        var ev = await _uow.Repository<Domain.Entities.Evidence>().GetByIdAsync(query.Id, ct);
        if (ev == null) return Result<EvidenceSummaryDto>.NotFound();
        return Result<EvidenceSummaryDto>.Success(ev.Adapt<EvidenceSummaryDto>());
    }
}
