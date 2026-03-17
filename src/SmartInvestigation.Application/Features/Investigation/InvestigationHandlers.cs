using Mapster;
using MediatR;
using SmartInvestigation.Application.Common;
using SmartInvestigation.Domain.Entities;
using SmartInvestigation.Domain.Enums;
using SmartInvestigation.Domain.Interfaces;

namespace SmartInvestigation.Application.Features.Investigation;

// ── Commands ──

public record CreateCaseDiaryEntryCommand(Guid CaseId, string Content, string? AttachmentUrls,
    string UserId) : IRequest<Result<CaseDiaryDto>>;

public record CreateInterrogationCommand(Guid CaseId, Guid PersonId, DateTime StartDateTime,
    string? Location, string? Summary, bool IsLawyerPresent, string? LawyerName,
    string UserId) : IRequest<Result<Guid>>;

public record CreateWitnessStatementCommand(Guid CaseId, Guid PersonId, string StatementText,
    string? StatementType, string? Location, string UserId) : IRequest<Result<Guid>>;

public record UpdateInvestigationStepCommand(Guid StepId, InvestigationStepStatus Status,
    string? CompletionRemarks, string UserId) : IRequest<Result<bool>>;

public record CreateSceneSurveyCommand(Guid CaseId, string Description, double? GpsLatitude,
    double? GpsLongitude, string? WeatherConditions, string? Observations,
    string UserId) : IRequest<Result<Guid>>;

public record AssignInvestigationTeamCommand(Guid CaseId, Guid LeadOfficerId, string? TeamName,
    List<Guid> MemberUserIds, string UserId) : IRequest<Result<Guid>>;

// ── Queries ──

public record GetCaseDiaryQuery(Guid CaseId, int PageNumber = 1, int PageSize = 20) : IRequest<Result<PagedResult<CaseDiaryDto>>>;
public record GetSOPsByCrimeTypeQuery(Guid CrimeTypeId) : IRequest<Result<List<SOPListDto>>>;
public record GetInvestigationStepsQuery(Guid CaseId) : IRequest<Result<List<InvestigationStepListDto>>>;

// ── DTOs ──

public record CaseDiaryDto(Guid Id, Guid CaseId, DateTime EntryDate, int EntryNumber,
    string Content, string? OfficerName, bool IsSubmittedToSupervisor, DateTime CreatedDate);

public record SOPListDto(Guid Id, string Name, string? Description, int Version,
    bool IsActive, int StepCount);

public record InvestigationStepListDto(Guid Id, string StepTitle, InvestigationStepStatus Status,
    string? AssignedToName, DateTime? DueDate, DateTime? CompletedDate, int StepOrder, bool IsMandatory);

// ── Handlers ──

public class CreateCaseDiaryEntryHandler : IRequestHandler<CreateCaseDiaryEntryCommand, Result<CaseDiaryDto>>
{
    private readonly IUnitOfWork _uow;
    public CreateCaseDiaryEntryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<CaseDiaryDto>> Handle(CreateCaseDiaryEntryCommand cmd, CancellationToken ct)
    {
        // Get next entry number
        var existing = await _uow.Repository<CaseDiary>().CountAsync(d => d.CaseId == cmd.CaseId, ct);

        var entry = new CaseDiary
        {
            CaseId = cmd.CaseId,
            EntryDate = DateTime.UtcNow,
            OfficerId = Guid.Parse(cmd.UserId),
            Content = cmd.Content,
            EntryNumber = existing + 1,
            AttachmentUrls = cmd.AttachmentUrls,
            CreatedBy = cmd.UserId
        };

        await _uow.Repository<CaseDiary>().AddAsync(entry, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<CaseDiaryDto>.Created(entry.Adapt<CaseDiaryDto>());
    }
}

public class CreateInterrogationHandler : IRequestHandler<CreateInterrogationCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;
    public CreateInterrogationHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<Guid>> Handle(CreateInterrogationCommand cmd, CancellationToken ct)
    {
        var interrogation = new Interrogation
        {
            CaseId = cmd.CaseId,
            PersonId = cmd.PersonId,
            InterrogatorUserId = Guid.Parse(cmd.UserId),
            StartDateTime = cmd.StartDateTime,
            Location = cmd.Location,
            Summary = cmd.Summary,
            IsLawyerPresent = cmd.IsLawyerPresent,
            LawyerName = cmd.LawyerName,
            CreatedBy = cmd.UserId
        };

        await _uow.Repository<Interrogation>().AddAsync(interrogation, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<Guid>.Created(interrogation.Id);
    }
}

public class CreateWitnessStatementHandler : IRequestHandler<CreateWitnessStatementCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;
    public CreateWitnessStatementHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<Guid>> Handle(CreateWitnessStatementCommand cmd, CancellationToken ct)
    {
        var statement = new WitnessStatement
        {
            CaseId = cmd.CaseId,
            PersonId = cmd.PersonId,
            StatementText = cmd.StatementText,
            RecordedByUserId = Guid.Parse(cmd.UserId),
            RecordedDate = DateTime.UtcNow,
            StatementType = cmd.StatementType,
            Location = cmd.Location,
            CreatedBy = cmd.UserId
        };

        await _uow.Repository<WitnessStatement>().AddAsync(statement, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<Guid>.Created(statement.Id);
    }
}

public class UpdateInvestigationStepHandler : IRequestHandler<UpdateInvestigationStepCommand, Result<bool>>
{
    private readonly IUnitOfWork _uow;
    public UpdateInvestigationStepHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<bool>> Handle(UpdateInvestigationStepCommand cmd, CancellationToken ct)
    {
        var step = await _uow.Repository<InvestigationStep>().GetByIdAsync(cmd.StepId, ct);
        if (step == null) return Result<bool>.NotFound("Step not found");

        step.Status = cmd.Status;
        step.CompletionRemarks = cmd.CompletionRemarks;
        if (cmd.Status == InvestigationStepStatus.Completed)
            step.CompletedDate = DateTime.UtcNow;
        step.ModifiedBy = cmd.UserId;

        _uow.Repository<InvestigationStep>().Update(step);
        await _uow.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}

public class CreateSceneSurveyHandler : IRequestHandler<CreateSceneSurveyCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;
    public CreateSceneSurveyHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<Guid>> Handle(CreateSceneSurveyCommand cmd, CancellationToken ct)
    {
        var survey = new SceneSurvey
        {
            CaseId = cmd.CaseId,
            SurveyDate = DateTime.UtcNow,
            OfficerId = Guid.Parse(cmd.UserId),
            Description = cmd.Description,
            GpsLatitude = cmd.GpsLatitude,
            GpsLongitude = cmd.GpsLongitude,
            WeatherConditions = cmd.WeatherConditions,
            Observations = cmd.Observations,
            CreatedBy = cmd.UserId
        };

        await _uow.Repository<SceneSurvey>().AddAsync(survey, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<Guid>.Created(survey.Id);
    }
}

public class AssignInvestigationTeamHandler : IRequestHandler<AssignInvestigationTeamCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;
    public AssignInvestigationTeamHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<Guid>> Handle(AssignInvestigationTeamCommand cmd, CancellationToken ct)
    {
        await _uow.BeginTransactionAsync(ct);
        try
        {
            var team = new InvestigationTeam
            {
                CaseId = cmd.CaseId,
                LeadOfficerId = cmd.LeadOfficerId,
                TeamName = cmd.TeamName,
                FormationDate = DateTime.UtcNow,
                CreatedBy = cmd.UserId
            };

            await _uow.Repository<InvestigationTeam>().AddAsync(team, ct);

            foreach (var memberId in cmd.MemberUserIds)
            {
                await _uow.Repository<InvestigationTeamMember>().AddAsync(new InvestigationTeamMember
                {
                    TeamId = team.Id,
                    UserId = memberId,
                    JoinDate = DateTime.UtcNow,
                    CreatedBy = cmd.UserId
                }, ct);
            }

            await _uow.SaveChangesAsync(ct);
            await _uow.CommitTransactionAsync(ct);
            return Result<Guid>.Created(team.Id);
        }
        catch
        {
            await _uow.RollbackTransactionAsync(ct);
            throw;
        }
    }
}

public class GetCaseDiaryHandler : IRequestHandler<GetCaseDiaryQuery, Result<PagedResult<CaseDiaryDto>>>
{
    private readonly IUnitOfWork _uow;
    public GetCaseDiaryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<PagedResult<CaseDiaryDto>>> Handle(GetCaseDiaryQuery query, CancellationToken ct)
    {
        var (items, total) = await _uow.Repository<CaseDiary>().GetPagedAsync(
            query.PageNumber, query.PageSize,
            predicate: d => d.CaseId == query.CaseId,
            orderBy: q => q.OrderByDescending(d => d.EntryNumber),
            disableTracking: true, cancellationToken: ct);

        return Result<PagedResult<CaseDiaryDto>>.Success(new PagedResult<CaseDiaryDto>
        {
            Items = items.Adapt<List<CaseDiaryDto>>(),
            TotalCount = total, PageNumber = query.PageNumber, PageSize = query.PageSize
        });
    }
}
