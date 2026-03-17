using Mapster;
using MediatR;
using SmartInvestigation.Application.Common;
using SmartInvestigation.Application.DTOs;
using SmartInvestigation.Domain.Entities;
using SmartInvestigation.Domain.Enums;
using SmartInvestigation.Domain.Interfaces;

namespace SmartInvestigation.Application.Features.Cases.Commands;

// ── Create Case ──
public record CreateCaseCommand(CreateCaseRequest Request, string UserId) : IRequest<Result<CaseDto>>;

public class CreateCaseCommandHandler : IRequestHandler<CreateCaseCommand, Result<CaseDto>>
{
    private readonly IUnitOfWork _uow;
    public CreateCaseCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<CaseDto>> Handle(CreateCaseCommand cmd, CancellationToken ct)
    {
        var req = cmd.Request;
        var caseEntity = new Case
        {
            CaseNumber = $"CASE-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}",
            Title = req.Title,
            Description = req.Description,
            FIRId = req.FIRId,
            CrimeTypeId = req.CrimeTypeId,
            CrimeSubTypeId = req.CrimeSubTypeId,
            PoliceStationId = req.PoliceStationId,
            DistrictId = req.DistrictId,
            Priority = req.Priority,
            AssignedOfficerId = req.AssignedOfficerId,
            IsHighProfile = req.IsHighProfile,
            IsSensitive = req.IsSensitive,
            Tags = req.Tags,
            Status = CaseStatus.Registered,
            DateOfRegistration = DateTime.UtcNow,
            CreatedBy = cmd.UserId
        };

        await _uow.Repository<Case>().AddAsync(caseEntity, ct);
        await _uow.SaveChangesAsync(ct);

        var dto = caseEntity.Adapt<CaseDto>();
        return Result<CaseDto>.Created(dto);
    }
}

// ── Update Case Status ──
public record UpdateCaseStatusCommand(Guid CaseId, CaseStatus NewStatus, string? Remarks, string UserId)
    : IRequest<Result<CaseDto>>;

public class UpdateCaseStatusCommandHandler : IRequestHandler<UpdateCaseStatusCommand, Result<CaseDto>>
{
    private readonly IUnitOfWork _uow;
    public UpdateCaseStatusCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<CaseDto>> Handle(UpdateCaseStatusCommand cmd, CancellationToken ct)
    {
        var caseEntity = await _uow.Repository<Case>().GetByIdAsync(cmd.CaseId, ct);
        if (caseEntity == null) return Result<CaseDto>.NotFound("Case not found");

        var history = new CaseStatusHistory
        {
            CaseId = cmd.CaseId,
            OldStatus = caseEntity.Status,
            NewStatus = cmd.NewStatus,
            Remarks = cmd.Remarks,
            ChangedByUserId = Guid.Parse(cmd.UserId),
            CreatedBy = cmd.UserId
        };

        caseEntity.Status = cmd.NewStatus;
        caseEntity.ModifiedBy = cmd.UserId;

        if (cmd.NewStatus == CaseStatus.Closed)
            caseEntity.DateOfClosure = DateTime.UtcNow;

        _uow.Repository<Case>().Update(caseEntity);
        await _uow.Repository<CaseStatusHistory>().AddAsync(history, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<CaseDto>.Success(caseEntity.Adapt<CaseDto>());
    }
}

// ── Transfer Case ──
public record TransferCaseCommand(Guid CaseId, Guid ToOfficerId, Guid? ToStationId,
    string Reason, string UserId) : IRequest<Result<bool>>;

public class TransferCaseCommandHandler : IRequestHandler<TransferCaseCommand, Result<bool>>
{
    private readonly IUnitOfWork _uow;
    public TransferCaseCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<bool>> Handle(TransferCaseCommand cmd, CancellationToken ct)
    {
        var caseEntity = await _uow.Repository<Case>().GetByIdAsync(cmd.CaseId, ct);
        if (caseEntity == null) return Result<bool>.NotFound("Case not found");

        var transfer = new CaseTransfer
        {
            CaseId = cmd.CaseId,
            FromOfficerId = caseEntity.AssignedOfficerId ?? Guid.Parse(cmd.UserId),
            ToOfficerId = cmd.ToOfficerId,
            FromStationId = caseEntity.PoliceStationId,
            ToStationId = cmd.ToStationId,
            Reason = cmd.Reason,
            TransferDate = DateTime.UtcNow,
            CreatedBy = cmd.UserId
        };

        caseEntity.AssignedOfficerId = cmd.ToOfficerId;
        if (cmd.ToStationId.HasValue)
            caseEntity.PoliceStationId = cmd.ToStationId.Value;

        _uow.Repository<Case>().Update(caseEntity);
        await _uow.Repository<CaseTransfer>().AddAsync(transfer, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<bool>.Success(true);
    }
}
