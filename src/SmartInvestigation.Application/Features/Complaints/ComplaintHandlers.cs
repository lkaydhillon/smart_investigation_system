using Mapster;
using MediatR;
using SmartInvestigation.Application.Common;
using SmartInvestigation.Application.DTOs;
using SmartInvestigation.Domain.Entities;
using SmartInvestigation.Domain.Enums;
using SmartInvestigation.Domain.Interfaces;

namespace SmartInvestigation.Application.Features.Complaints;

public record CreateComplaintCommand(CreateComplaintRequest Request, string UserId) : IRequest<Result<ComplaintDto>>;

public class CreateComplaintCommandHandler : IRequestHandler<CreateComplaintCommand, Result<ComplaintDto>>
{
    private readonly IUnitOfWork _uow;
    public CreateComplaintCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<ComplaintDto>> Handle(CreateComplaintCommand cmd, CancellationToken ct)
    {
        var req = cmd.Request;
        // Ensure DateOfIncident is always UTC for PostgreSQL compatibility
        var incidentDate = req.DateOfIncident.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(req.DateOfIncident, DateTimeKind.Utc)
            : req.DateOfIncident.ToUniversalTime();

        var complaint = new Complaint
        {
            ComplaintNumber = $"CMP-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}",
            Type = req.Type,
            Status = ComplaintStatus.Received,
            Description = req.Description,
            ComplainantName = req.ComplainantName,
            ComplainantPhone = req.ComplainantPhone,
            ComplainantAddress = req.ComplainantAddress,
            GpsLatitude = req.GpsLatitude,
            GpsLongitude = req.GpsLongitude,
            LocationAddress = req.LocationAddress,
            DateOfIncident = incidentDate,
            PoliceStationId = req.PoliceStationId,
            IsUrgent = req.IsUrgent,
            VoiceRecordingUrl = req.VoiceRecordingUrl,
            IsOfflineEntry = req.IsOfflineEntry,
            SyncedAt = req.IsOfflineEntry ? DateTime.UtcNow : null,
            ReceivedByUserId = Guid.Parse(cmd.UserId),
            CreatedBy = cmd.UserId
        };

        await _uow.Repository<Complaint>().AddAsync(complaint, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<ComplaintDto>.Created(complaint.Adapt<ComplaintDto>());
    }
}

public record GetComplaintsQuery(int PageNumber = 1, int PageSize = 20, string? Status = null,
    Guid? StationId = null) : IRequest<Result<PagedResult<ComplaintDto>>>;

public class GetComplaintsQueryHandler : IRequestHandler<GetComplaintsQuery, Result<PagedResult<ComplaintDto>>>
{
    private readonly IUnitOfWork _uow;
    public GetComplaintsQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<PagedResult<ComplaintDto>>> Handle(GetComplaintsQuery query, CancellationToken ct)
    {
        var (items, totalCount) = await _uow.Repository<Complaint>().GetPagedAsync(
            query.PageNumber, query.PageSize,
            predicate: c => query.StationId == null || c.PoliceStationId == query.StationId,
            orderBy: q => q.OrderByDescending(c => c.CreatedDate),
            disableTracking: true, cancellationToken: ct);

        return Result<PagedResult<ComplaintDto>>.Success(new PagedResult<ComplaintDto>
        {
            Items = items.Adapt<List<ComplaintDto>>(),
            TotalCount = totalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        });
    }
}

public record UpdateComplaintCommand(Guid Id, CreateComplaintRequest Request, string UserId) : IRequest<Result<ComplaintDto>>;

public class UpdateComplaintCommandHandler : IRequestHandler<UpdateComplaintCommand, Result<ComplaintDto>>
{
    private readonly IUnitOfWork _uow;
    public UpdateComplaintCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<ComplaintDto>> Handle(UpdateComplaintCommand cmd, CancellationToken ct)
    {
        var complaint = await _uow.Repository<Complaint>().GetByIdAsync(cmd.Id, ct);
        if (complaint == null) return Result<ComplaintDto>.NotFound($"Complaint {cmd.Id} not found");

        var req = cmd.Request;
        
        var incidentDate = req.DateOfIncident.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(req.DateOfIncident, DateTimeKind.Utc)
            : req.DateOfIncident.ToUniversalTime();

        complaint.Type = req.Type;
        complaint.Description = req.Description;
        complaint.ComplainantName = req.ComplainantName;
        complaint.ComplainantPhone = req.ComplainantPhone;
        complaint.ComplainantAddress = req.ComplainantAddress;
        complaint.GpsLatitude = req.GpsLatitude;
        complaint.GpsLongitude = req.GpsLongitude;
        complaint.LocationAddress = req.LocationAddress;
        complaint.DateOfIncident = incidentDate;
        complaint.PoliceStationId = req.PoliceStationId;
        complaint.IsUrgent = req.IsUrgent;
        complaint.VoiceRecordingUrl = req.VoiceRecordingUrl;
        complaint.ModifiedBy = cmd.UserId;

        _uow.Repository<Complaint>().Update(complaint);
        await _uow.SaveChangesAsync(ct);

        return Result<ComplaintDto>.Success(complaint.Adapt<ComplaintDto>());
    }
}

public record DeleteComplaintCommand(Guid Id, string UserId) : IRequest<Result<bool>>;

public class DeleteComplaintCommandHandler : IRequestHandler<DeleteComplaintCommand, Result<bool>>
{
    private readonly IUnitOfWork _uow;
    public DeleteComplaintCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<bool>> Handle(DeleteComplaintCommand cmd, CancellationToken ct)
    {
        var complaint = await _uow.Repository<Complaint>().GetByIdAsync(cmd.Id, ct);
        if (complaint == null) return Result<bool>.NotFound($"Complaint {cmd.Id} not found");

        _uow.Repository<Complaint>().Delete(complaint);
        await _uow.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}

