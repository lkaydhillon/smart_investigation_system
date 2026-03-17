using Mapster;
using MediatR;
using SmartInvestigation.Application.Common;
using SmartInvestigation.Application.DTOs;
using SmartInvestigation.Domain.Entities;
using SmartInvestigation.Domain.Enums;
using SmartInvestigation.Domain.Interfaces;

namespace SmartInvestigation.Application.Features.Legal;

// ── Commands ──

public record AddLegalSectionToCaseCommand(Guid CaseId, Guid LegalSectionId,
    bool IsAIRecommended, double? ConfidenceScore, string? Remarks, string UserId) : IRequest<Result<bool>>;

public record CreateCourtHearingCommand(Guid CaseId, Guid CourtId, DateTime HearingDate,
    string? Purpose, string UserId) : IRequest<Result<Guid>>;

public record RecordCourtHearingOutcomeCommand(Guid HearingId, string? Outcome,
    DateTime? NextHearingDate, string? JudgeName, string? Remarks, string UserId) : IRequest<Result<bool>>;

public record CreateChargesheetCommand(Guid CaseId, List<Guid> AccusedPersonIds,
    string? FileUrl, bool IsSupplementary, Guid? OriginalChargesheetId,
    string UserId) : IRequest<Result<Guid>>;

public record FileBailApplicationCommand(Guid CaseId, Guid PersonId, Guid CourtId,
    decimal? BailAmount, string? Conditions, string UserId) : IRequest<Result<Guid>>;

public record CreateRemandCommand(Guid CaseId, Guid PersonId, RemandType RemandType,
    DateTime StartDate, DateTime EndDate, Guid CourtId, string? CustodyLocation,
    string UserId) : IRequest<Result<Guid>>;

// ── Queries ──

public record SearchLegalSectionsQuery(string? SearchTerm, string? Act, int PageNumber = 1, int PageSize = 20)
    : IRequest<Result<PagedResult<LegalSectionDto>>>;

public record GetCourtHearingsByCaseQuery(Guid CaseId) : IRequest<Result<List<CourtHearingDto>>>;
public record GetChargesheetsByCaseQuery(Guid CaseId) : IRequest<Result<List<ChargesheetDto>>>;

// ── DTOs ──

public record CourtHearingDto(Guid Id, DateTime HearingDate, string? Purpose, string? Outcome,
    DateTime? NextHearingDate, string? JudgeName, string? CourtName, DateTime CreatedDate);

public record ChargesheetDto(Guid Id, string ChargesheetNumber, DateTime FilingDate,
    ChargesheetStatus Status, bool IsSupplementary, string? FiledByName, DateTime CreatedDate);

// ── Handlers ──

public class AddLegalSectionToCaseHandler : IRequestHandler<AddLegalSectionToCaseCommand, Result<bool>>
{
    private readonly IUnitOfWork _uow;
    public AddLegalSectionToCaseHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<bool>> Handle(AddLegalSectionToCaseCommand cmd, CancellationToken ct)
    {
        var exists = await _uow.Repository<CaseLegalSection>().ExistsAsync(
            cls => cls.CaseId == cmd.CaseId && cls.LegalSectionId == cmd.LegalSectionId, ct);
        if (exists) return Result<bool>.Failure("Legal section already added to this case");

        await _uow.Repository<CaseLegalSection>().AddAsync(new CaseLegalSection
        {
            CaseId = cmd.CaseId,
            LegalSectionId = cmd.LegalSectionId,
            AddedByUserId = Guid.Parse(cmd.UserId),
            AddedDate = DateTime.UtcNow,
            IsAIRecommended = cmd.IsAIRecommended,
            ConfidenceScore = cmd.ConfidenceScore,
            Remarks = cmd.Remarks,
            CreatedBy = cmd.UserId
        }, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<bool>.Created(true);
    }
}

public class CreateCourtHearingHandler : IRequestHandler<CreateCourtHearingCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;
    public CreateCourtHearingHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<Guid>> Handle(CreateCourtHearingCommand cmd, CancellationToken ct)
    {
        var hearing = new CourtHearing
        {
            CaseId = cmd.CaseId,
            CourtId = cmd.CourtId,
            HearingDate = cmd.HearingDate,
            Purpose = cmd.Purpose,
            CreatedBy = cmd.UserId
        };
        await _uow.Repository<CourtHearing>().AddAsync(hearing, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<Guid>.Created(hearing.Id);
    }
}

public class RecordCourtHearingOutcomeHandler : IRequestHandler<RecordCourtHearingOutcomeCommand, Result<bool>>
{
    private readonly IUnitOfWork _uow;
    public RecordCourtHearingOutcomeHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<bool>> Handle(RecordCourtHearingOutcomeCommand cmd, CancellationToken ct)
    {
        var hearing = await _uow.Repository<CourtHearing>().GetByIdAsync(cmd.HearingId, ct);
        if (hearing == null) return Result<bool>.NotFound("Hearing not found");

        hearing.Outcome = cmd.Outcome;
        hearing.NextHearingDate = cmd.NextHearingDate;
        hearing.JudgeName = cmd.JudgeName;
        hearing.Remarks = cmd.Remarks;
        hearing.ModifiedBy = cmd.UserId;
        _uow.Repository<CourtHearing>().Update(hearing);
        await _uow.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}

public class CreateChargesheetHandler : IRequestHandler<CreateChargesheetCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;
    public CreateChargesheetHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<Guid>> Handle(CreateChargesheetCommand cmd, CancellationToken ct)
    {
        await _uow.BeginTransactionAsync(ct);
        try
        {
            var cs = new Chargesheet
            {
                CaseId = cmd.CaseId,
                ChargesheetNumber = $"CS-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}",
                FilingDate = DateTime.UtcNow,
                Status = ChargesheetStatus.Draft,
                FiledByUserId = Guid.Parse(cmd.UserId),
                IsSupplementary = cmd.IsSupplementary,
                OriginalChargesheetId = cmd.OriginalChargesheetId,
                FileUrl = cmd.FileUrl,
                CreatedBy = cmd.UserId
            };

            await _uow.Repository<Chargesheet>().AddAsync(cs, ct);

            foreach (var personId in cmd.AccusedPersonIds)
            {
                await _uow.Repository<ChargesheetPerson>().AddAsync(new ChargesheetPerson
                {
                    ChargesheetId = cs.Id,
                    PersonId = personId,
                    CreatedBy = cmd.UserId
                }, ct);
            }

            await _uow.SaveChangesAsync(ct);
            await _uow.CommitTransactionAsync(ct);
            return Result<Guid>.Created(cs.Id);
        }
        catch
        {
            await _uow.RollbackTransactionAsync(ct);
            throw;
        }
    }
}

public class FileBailApplicationHandler : IRequestHandler<FileBailApplicationCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;
    public FileBailApplicationHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<Guid>> Handle(FileBailApplicationCommand cmd, CancellationToken ct)
    {
        var bail = new BailApplication
        {
            CaseId = cmd.CaseId,
            PersonId = cmd.PersonId,
            CourtId = cmd.CourtId,
            ApplicationDate = DateTime.UtcNow,
            Status = BailStatus.Applied,
            BailAmount = cmd.BailAmount,
            Conditions = cmd.Conditions,
            CreatedBy = cmd.UserId
        };

        await _uow.Repository<BailApplication>().AddAsync(bail, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<Guid>.Created(bail.Id);
    }
}

public class CreateRemandHandler : IRequestHandler<CreateRemandCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;
    public CreateRemandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<Guid>> Handle(CreateRemandCommand cmd, CancellationToken ct)
    {
        var remand = new Remand
        {
            CaseId = cmd.CaseId,
            PersonId = cmd.PersonId,
            RemandType = cmd.RemandType,
            StartDate = cmd.StartDate,
            EndDate = cmd.EndDate,
            CourtId = cmd.CourtId,
            CustodyLocation = cmd.CustodyLocation,
            CreatedBy = cmd.UserId
        };

        await _uow.Repository<Remand>().AddAsync(remand, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<Guid>.Created(remand.Id);
    }
}

public class SearchLegalSectionsHandler : IRequestHandler<SearchLegalSectionsQuery, Result<PagedResult<LegalSectionDto>>>
{
    private readonly IUnitOfWork _uow;
    public SearchLegalSectionsHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<PagedResult<LegalSectionDto>>> Handle(SearchLegalSectionsQuery query, CancellationToken ct)
    {
        var (items, total) = await _uow.Repository<LegalSection>().GetPagedAsync(
            query.PageNumber, query.PageSize,
            predicate: l => (string.IsNullOrEmpty(query.SearchTerm) || l.Code.Contains(query.SearchTerm) || l.Title.Contains(query.SearchTerm)) &&
                            (string.IsNullOrEmpty(query.Act) || l.Act == query.Act),
            orderBy: q => q.OrderBy(l => l.Code),
            disableTracking: true, cancellationToken: ct);

        return Result<PagedResult<LegalSectionDto>>.Success(new PagedResult<LegalSectionDto>
        {
            Items = items.Adapt<List<LegalSectionDto>>(),
            TotalCount = total, PageNumber = query.PageNumber, PageSize = query.PageSize
        });
    }
}
