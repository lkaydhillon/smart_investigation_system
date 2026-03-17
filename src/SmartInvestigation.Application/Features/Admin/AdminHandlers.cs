using Mapster;
using MediatR;
using SmartInvestigation.Application.Common;
using SmartInvestigation.Application.DTOs;
using SmartInvestigation.Domain.Entities;
using SmartInvestigation.Domain.Enums;
using SmartInvestigation.Domain.Interfaces;

namespace SmartInvestigation.Application.Features.Admin;

// ── Notifications ──

public record MarkNotificationReadCommand(Guid NotificationId, string UserId) : IRequest<Result<bool>>;
public record GetUserNotificationsQuery(string UserId, bool UnreadOnly = false, int PageNumber = 1, int PageSize = 20)
    : IRequest<Result<PagedResult<NotificationDto>>>;

// ── Lookups & Config ──

public record CreateLookupValueCommand(string Category, string Code, string DisplayName,
    string? ParentCode, int SortOrder, string UserId) : IRequest<Result<Guid>>;

public record UpdateLookupValueCommand(Guid Id, string DisplayName, int SortOrder,
    bool IsActive, string UserId) : IRequest<Result<bool>>;

public record GetLookupsByCategoryQuery(string Category) : IRequest<Result<List<LookupValueDto>>>;
public record GetAllLookupCategoriesQuery() : IRequest<Result<List<string>>>;

// ── Crime Types ──

public record CreateCrimeTypeCommand(string Name, string Code, Guid? ParentId,
    string? Severity, string? Description, string UserId) : IRequest<Result<Guid>>;

public record GetCrimeTypesQuery(bool ActiveOnly = true) : IRequest<Result<List<CrimeTypeDto>>>;

// ── Custom Fields ──

public record CreateCustomFieldCommand(string EntityType, string FieldName, string DisplayLabel,
    CustomFieldType FieldType, string? Options, bool IsRequired, int DisplayOrder,
    string? ValidationRegex, string? GroupName, bool IsSearchable, string UserId) : IRequest<Result<Guid>>;

public record GetCustomFieldsByEntityQuery(string EntityType) : IRequest<Result<List<CustomFieldDefDto>>>;

// ── System Config ──

public record UpdateSystemConfigCommand(string Key, string? Value, string UserId) : IRequest<Result<bool>>;
public record GetSystemConfigsQuery(string? Category = null) : IRequest<Result<List<SystemConfigDto>>>;

// ── DTOs ──

public record LookupValueDto(Guid Id, string Category, string Code, string DisplayName,
    string? ParentCode, int SortOrder, bool IsActive);

public record CrimeTypeDto(Guid Id, string Name, string Code, Guid? ParentId,
    string? Severity, string? Description, bool IsActive, List<CrimeTypeDto>? Children);

public record CustomFieldDefDto(Guid Id, string EntityType, string FieldName, string DisplayLabel,
    CustomFieldType FieldType, string? Options, bool IsRequired, int DisplayOrder,
    string? GroupName, bool IsSearchable, bool IsActive);

public record SystemConfigDto(Guid Id, string Key, string? Value, string Category,
    string DataType, string? Description, bool IsEditable);

// ── Handlers ──

public class MarkNotificationReadHandler : IRequestHandler<MarkNotificationReadCommand, Result<bool>>
{
    private readonly IUnitOfWork _uow;
    public MarkNotificationReadHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<bool>> Handle(MarkNotificationReadCommand cmd, CancellationToken ct)
    {
        var notif = await _uow.Repository<Notification>().GetByIdAsync(cmd.NotificationId, ct);
        if (notif == null) return Result<bool>.NotFound();

        notif.IsRead = true;
        notif.ReadDate = DateTime.UtcNow;
        _uow.Repository<Notification>().Update(notif);
        await _uow.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}

public class GetUserNotificationsHandler : IRequestHandler<GetUserNotificationsQuery, Result<PagedResult<NotificationDto>>>
{
    private readonly IUnitOfWork _uow;
    public GetUserNotificationsHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<PagedResult<NotificationDto>>> Handle(GetUserNotificationsQuery query, CancellationToken ct)
    {
        var userId = Guid.Parse(query.UserId);
        var (items, total) = await _uow.Repository<Notification>().GetPagedAsync(
            query.PageNumber, query.PageSize,
            predicate: n => n.UserId == userId && (!query.UnreadOnly || !n.IsRead),
            orderBy: q => q.OrderByDescending(n => n.CreatedDate),
            disableTracking: true, cancellationToken: ct);

        return Result<PagedResult<NotificationDto>>.Success(new PagedResult<NotificationDto>
        {
            Items = items.Adapt<List<NotificationDto>>(),
            TotalCount = total, PageNumber = query.PageNumber, PageSize = query.PageSize
        });
    }
}

public class CreateLookupValueHandler : IRequestHandler<CreateLookupValueCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;
    public CreateLookupValueHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<Guid>> Handle(CreateLookupValueCommand cmd, CancellationToken ct)
    {
        var exists = await _uow.Repository<LookupValue>().ExistsAsync(
            l => l.Category == cmd.Category && l.Code == cmd.Code, ct);
        if (exists) return Result<Guid>.Failure("Lookup value with this code already exists in this category");

        var lookup = new LookupValue
        {
            Category = cmd.Category, Code = cmd.Code, DisplayName = cmd.DisplayName,
            ParentCode = cmd.ParentCode, SortOrder = cmd.SortOrder, CreatedBy = cmd.UserId
        };
        await _uow.Repository<LookupValue>().AddAsync(lookup, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<Guid>.Created(lookup.Id);
    }
}

public class UpdateLookupValueHandler : IRequestHandler<UpdateLookupValueCommand, Result<bool>>
{
    private readonly IUnitOfWork _uow;
    public UpdateLookupValueHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<bool>> Handle(UpdateLookupValueCommand cmd, CancellationToken ct)
    {
        var lookup = await _uow.Repository<LookupValue>().GetByIdAsync(cmd.Id, ct);
        if (lookup == null) return Result<bool>.NotFound();

        lookup.DisplayName = cmd.DisplayName;
        lookup.SortOrder = cmd.SortOrder;
        lookup.IsActive = cmd.IsActive;
        lookup.ModifiedBy = cmd.UserId;
        _uow.Repository<LookupValue>().Update(lookup);
        await _uow.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}

public class GetLookupsByCategoryHandler : IRequestHandler<GetLookupsByCategoryQuery, Result<List<LookupValueDto>>>
{
    private readonly IUnitOfWork _uow;
    public GetLookupsByCategoryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<List<LookupValueDto>>> Handle(GetLookupsByCategoryQuery query, CancellationToken ct)
    {
        var items = await _uow.Repository<LookupValue>().GetAsync(
            predicate: l => l.Category == query.Category && l.IsActive,
            orderBy: q => q.OrderBy(l => l.SortOrder),
            includeString: null,
            disableTracking: true, cancellationToken: ct);
        return Result<List<LookupValueDto>>.Success(items.Adapt<List<LookupValueDto>>());
    }
}

public class CreateCrimeTypeHandler : IRequestHandler<CreateCrimeTypeCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;
    public CreateCrimeTypeHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<Guid>> Handle(CreateCrimeTypeCommand cmd, CancellationToken ct)
    {
        var crimeType = new CrimeType
        {
            Name = cmd.Name, Code = cmd.Code, ParentId = cmd.ParentId,
            Severity = cmd.Severity, Description = cmd.Description, CreatedBy = cmd.UserId
        };
        await _uow.Repository<CrimeType>().AddAsync(crimeType, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<Guid>.Created(crimeType.Id);
    }
}

public class GetCrimeTypesHandler : IRequestHandler<GetCrimeTypesQuery, Result<List<CrimeTypeDto>>>
{
    private readonly IUnitOfWork _uow;
    public GetCrimeTypesHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<List<CrimeTypeDto>>> Handle(GetCrimeTypesQuery query, CancellationToken ct)
    {
        var items = await _uow.Repository<CrimeType>().GetAsync(
            predicate: query.ActiveOnly ? t => t.IsActive : null,
            orderBy: q => q.OrderBy(t => t.SortOrder),
            includeString: null,
            disableTracking: true, cancellationToken: ct);
        return Result<List<CrimeTypeDto>>.Success(items.Adapt<List<CrimeTypeDto>>());
    }
}

public class CreateCustomFieldHandler : IRequestHandler<CreateCustomFieldCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;
    public CreateCustomFieldHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<Guid>> Handle(CreateCustomFieldCommand cmd, CancellationToken ct)
    {
        var field = new CustomFieldDefinition
        {
            EntityType = cmd.EntityType, FieldName = cmd.FieldName, DisplayLabel = cmd.DisplayLabel,
            FieldType = cmd.FieldType, Options = cmd.Options, IsRequired = cmd.IsRequired,
            DisplayOrder = cmd.DisplayOrder, ValidationRegex = cmd.ValidationRegex,
            GroupName = cmd.GroupName, IsSearchable = cmd.IsSearchable, CreatedBy = cmd.UserId
        };
        await _uow.Repository<CustomFieldDefinition>().AddAsync(field, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<Guid>.Created(field.Id);
    }
}

public class GetCustomFieldsByEntityHandler : IRequestHandler<GetCustomFieldsByEntityQuery, Result<List<CustomFieldDefDto>>>
{
    private readonly IUnitOfWork _uow;
    public GetCustomFieldsByEntityHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<List<CustomFieldDefDto>>> Handle(GetCustomFieldsByEntityQuery query, CancellationToken ct)
    {
        var items = await _uow.Repository<CustomFieldDefinition>().GetAsync(
            predicate: f => f.EntityType == query.EntityType && f.IsActive,
            orderBy: q => q.OrderBy(f => f.DisplayOrder),
            includeString: null,
            disableTracking: true, cancellationToken: ct);
        return Result<List<CustomFieldDefDto>>.Success(items.Adapt<List<CustomFieldDefDto>>());
    }
}

public class UpdateSystemConfigHandler : IRequestHandler<UpdateSystemConfigCommand, Result<bool>>
{
    private readonly IUnitOfWork _uow;
    public UpdateSystemConfigHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<bool>> Handle(UpdateSystemConfigCommand cmd, CancellationToken ct)
    {
        var configs = await _uow.Repository<SystemConfiguration>().GetAsync(c => c.Key == cmd.Key, ct);
        var config = configs.FirstOrDefault();
        if (config == null) return Result<bool>.NotFound("Config key not found");
        if (!config.IsEditable) return Result<bool>.Failure("This configuration cannot be edited");

        config.Value = cmd.Value;
        config.ModifiedBy = cmd.UserId;
        _uow.Repository<SystemConfiguration>().Update(config);
        await _uow.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}

public class GetSystemConfigsHandler : IRequestHandler<GetSystemConfigsQuery, Result<List<SystemConfigDto>>>
{
    private readonly IUnitOfWork _uow;
    public GetSystemConfigsHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<List<SystemConfigDto>>> Handle(GetSystemConfigsQuery query, CancellationToken ct)
    {
        var items = await _uow.Repository<SystemConfiguration>().GetAsync(
            predicate: c => query.Category == null || c.Category == query.Category,
            orderBy: q => q.OrderBy(c => c.Category).ThenBy(c => c.Key),
            includeString: null,
            disableTracking: true, cancellationToken: ct);
        return Result<List<SystemConfigDto>>.Success(items.Adapt<List<SystemConfigDto>>());
    }
}
