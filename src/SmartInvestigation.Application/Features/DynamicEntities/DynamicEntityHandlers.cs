using Mapster;
using MediatR;
using SmartInvestigation.Application.Common;
using SmartInvestigation.Application.DTOs;
using SmartInvestigation.Domain.Entities;
using SmartInvestigation.Domain.Enums;
using SmartInvestigation.Domain.Interfaces;

namespace SmartInvestigation.Application.Features.DynamicEntities;

// ── Commands ──

public record CreateDynamicEntityDefinitionCommand(string Name, string DisplayName,
    string? Description, string? IconName, List<CreateDynFieldReq> Fields,
    string UserId) : IRequest<Result<DynamicEntityDefinitionDto>>;

public record CreateDynFieldReq(string FieldName, string DisplayLabel, DynamicEntityFieldType FieldType,
    bool IsRequired, string? Options, string? DefaultValue, int DisplayOrder);

public record AddFieldToEntityCommand(Guid EntityDefinitionId, string FieldName, string DisplayLabel,
    DynamicEntityFieldType FieldType, bool IsRequired, string? Options,
    string? DefaultValue, int DisplayOrder, string UserId) : IRequest<Result<Guid>>;

public record SaveDynamicRecordCommand(Guid EntityDefinitionId, string? DisplayTitle,
    Dictionary<string, string?> FieldValues, string UserId) : IRequest<Result<Guid>>;

public record UpdateDynamicRecordCommand(Guid RecordId, string? DisplayTitle,
    Dictionary<string, string?> FieldValues, string UserId) : IRequest<Result<bool>>;

public record DeleteDynamicRecordCommand(Guid RecordId, string UserId) : IRequest<Result<bool>>;

// ── Queries ──

public record GetDynamicEntityDefinitionsQuery() : IRequest<Result<List<DynamicEntityDefinitionDto>>>;
public record GetDynamicEntityByIdQuery(Guid Id) : IRequest<Result<DynamicEntityDefinitionDto>>;
public record GetDynamicRecordsQuery(Guid EntityDefinitionId, int PageNumber = 1, int PageSize = 20)
    : IRequest<Result<PagedResult<DynamicEntityRecordDto>>>;
public record GetDynamicRecordByIdQuery(Guid RecordId) : IRequest<Result<DynamicEntityRecordDto>>;

// ── Handlers ──

public class CreateDynamicEntityDefinitionHandler : IRequestHandler<CreateDynamicEntityDefinitionCommand, Result<DynamicEntityDefinitionDto>>
{
    private readonly IUnitOfWork _uow;
    public CreateDynamicEntityDefinitionHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<DynamicEntityDefinitionDto>> Handle(CreateDynamicEntityDefinitionCommand cmd, CancellationToken ct)
    {
        var exists = await _uow.Repository<DynamicEntityDefinition>().ExistsAsync(
            d => d.Name == cmd.Name, ct);
        if (exists) return Result<DynamicEntityDefinitionDto>.Failure("Entity with this name already exists");

        await _uow.BeginTransactionAsync(ct);
        try
        {
            var entity = new DynamicEntityDefinition
            {
                Name = cmd.Name,
                DisplayName = cmd.DisplayName,
                Description = cmd.Description,
                IconName = cmd.IconName,
                IsActive = true,
                CreatedBy = cmd.UserId
            };

            await _uow.Repository<DynamicEntityDefinition>().AddAsync(entity, ct);

            foreach (var f in cmd.Fields)
            {
                await _uow.Repository<DynamicEntityField>().AddAsync(new DynamicEntityField
                {
                    EntityDefinitionId = entity.Id,
                    FieldName = f.FieldName,
                    DisplayLabel = f.DisplayLabel,
                    FieldType = f.FieldType,
                    IsRequired = f.IsRequired,
                    Options = f.Options,
                    DefaultValue = f.DefaultValue,
                    DisplayOrder = f.DisplayOrder,
                    IsActive = true,
                    CreatedBy = cmd.UserId
                }, ct);
            }

            await _uow.SaveChangesAsync(ct);
            await _uow.CommitTransactionAsync(ct);

            // Reload with fields
            var fields = await _uow.Repository<DynamicEntityField>().GetAsync(
                f => f.EntityDefinitionId == entity.Id, includeString: null, cancellationToken: ct);

            var dto = new DynamicEntityDefinitionDto(entity.Id, entity.Name, entity.DisplayName,
                entity.Description, entity.IsActive, fields.Adapt<List<DynamicEntityFieldDto>>());

            return Result<DynamicEntityDefinitionDto>.Created(dto);
        }
        catch
        {
            await _uow.RollbackTransactionAsync(ct);
            throw;
        }
    }
}

public class AddFieldToEntityHandler : IRequestHandler<AddFieldToEntityCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;
    public AddFieldToEntityHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<Guid>> Handle(AddFieldToEntityCommand cmd, CancellationToken ct)
    {
        var exists = await _uow.Repository<DynamicEntityDefinition>().ExistsAsync(
            d => d.Id == cmd.EntityDefinitionId, ct);
        if (!exists) return Result<Guid>.NotFound("Entity definition not found");

        var field = new DynamicEntityField
        {
            EntityDefinitionId = cmd.EntityDefinitionId,
            FieldName = cmd.FieldName,
            DisplayLabel = cmd.DisplayLabel,
            FieldType = cmd.FieldType,
            IsRequired = cmd.IsRequired,
            Options = cmd.Options,
            DefaultValue = cmd.DefaultValue,
            DisplayOrder = cmd.DisplayOrder,
            IsActive = true,
            CreatedBy = cmd.UserId
        };

        await _uow.Repository<DynamicEntityField>().AddAsync(field, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<Guid>.Created(field.Id);
    }
}

public class SaveDynamicRecordHandler : IRequestHandler<SaveDynamicRecordCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;
    public SaveDynamicRecordHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<Guid>> Handle(SaveDynamicRecordCommand cmd, CancellationToken ct)
    {
        // Validate required fields
        var fields = await _uow.Repository<DynamicEntityField>().GetAsync(
            f => f.EntityDefinitionId == cmd.EntityDefinitionId && f.IsActive, includeString: null, cancellationToken: ct);

        foreach (var f in fields.Where(f => f.IsRequired))
        {
            if (!cmd.FieldValues.ContainsKey(f.FieldName) || string.IsNullOrEmpty(cmd.FieldValues[f.FieldName]))
                return Result<Guid>.Failure($"Required field '{f.DisplayLabel}' is missing");
        }

        await _uow.BeginTransactionAsync(ct);
        try
        {
            var record = new DynamicEntityRecord
            {
                EntityDefinitionId = cmd.EntityDefinitionId,
                DisplayTitle = cmd.DisplayTitle,
                CreatedBy = cmd.UserId
            };

            await _uow.Repository<DynamicEntityRecord>().AddAsync(record, ct);

            foreach (var (key, value) in cmd.FieldValues)
            {
                var field = fields.FirstOrDefault(f => f.FieldName == key);
                if (field == null) continue;

                await _uow.Repository<DynamicEntityRecordValue>().AddAsync(new DynamicEntityRecordValue
                {
                    RecordId = record.Id,
                    FieldId = field.Id,
                    TextValue = value,
                    CreatedBy = cmd.UserId
                }, ct);
            }

            await _uow.SaveChangesAsync(ct);
            await _uow.CommitTransactionAsync(ct);
            return Result<Guid>.Created(record.Id);
        }
        catch
        {
            await _uow.RollbackTransactionAsync(ct);
            throw;
        }
    }
}

public class UpdateDynamicRecordHandler : IRequestHandler<UpdateDynamicRecordCommand, Result<bool>>
{
    private readonly IUnitOfWork _uow;
    public UpdateDynamicRecordHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<bool>> Handle(UpdateDynamicRecordCommand cmd, CancellationToken ct)
    {
        var record = await _uow.Repository<DynamicEntityRecord>().GetByIdAsync(cmd.RecordId, ct);
        if (record == null) return Result<bool>.NotFound("Record not found");

        record.DisplayTitle = cmd.DisplayTitle;
        record.ModifiedBy = cmd.UserId;
        _uow.Repository<DynamicEntityRecord>().Update(record);

        // Update field values
        var existingValues = await _uow.Repository<DynamicEntityRecordValue>().GetAsync(
            v => v.RecordId == cmd.RecordId, includeString: null, cancellationToken: ct);
        var fields = await _uow.Repository<DynamicEntityField>().GetAsync(
            f => f.EntityDefinitionId == record.EntityDefinitionId, includeString: null, cancellationToken: ct);

        foreach (var (key, value) in cmd.FieldValues)
        {
            var field = fields.FirstOrDefault(f => f.FieldName == key);
            if (field == null) continue;

            var existing = existingValues.FirstOrDefault(v => v.FieldId == field.Id);
            if (existing != null)
            {
                existing.TextValue = value;
                existing.ModifiedBy = cmd.UserId;
                _uow.Repository<DynamicEntityRecordValue>().Update(existing);
            }
            else
            {
                await _uow.Repository<DynamicEntityRecordValue>().AddAsync(new DynamicEntityRecordValue
                {
                    RecordId = cmd.RecordId,
                    FieldId = field.Id,
                    TextValue = value,
                    CreatedBy = cmd.UserId
                }, ct);
            }
        }

        await _uow.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}

public class DeleteDynamicRecordHandler : IRequestHandler<DeleteDynamicRecordCommand, Result<bool>>
{
    private readonly IUnitOfWork _uow;
    public DeleteDynamicRecordHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<bool>> Handle(DeleteDynamicRecordCommand cmd, CancellationToken ct)
    {
        var record = await _uow.Repository<DynamicEntityRecord>().GetByIdAsync(cmd.RecordId, ct);
        if (record == null) return Result<bool>.NotFound("Record not found");

        _uow.Repository<DynamicEntityRecord>().SoftDelete(record, cmd.UserId);
        await _uow.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}

public class GetDynamicEntityDefinitionsHandler : IRequestHandler<GetDynamicEntityDefinitionsQuery, Result<List<DynamicEntityDefinitionDto>>>
{
    private readonly IUnitOfWork _uow;
    public GetDynamicEntityDefinitionsHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<List<DynamicEntityDefinitionDto>>> Handle(GetDynamicEntityDefinitionsQuery query, CancellationToken ct)
    {
        var entities = await _uow.Repository<DynamicEntityDefinition>().GetAsync(
            predicate: d => d.IsActive,
            orderBy: q => q.OrderBy(d => d.DisplayName),
            includeString: null,
            disableTracking: true, cancellationToken: ct);
        return Result<List<DynamicEntityDefinitionDto>>.Success(entities.Adapt<List<DynamicEntityDefinitionDto>>());
    }
}

public class GetDynamicRecordsHandler : IRequestHandler<GetDynamicRecordsQuery, Result<PagedResult<DynamicEntityRecordDto>>>
{
    private readonly IUnitOfWork _uow;
    public GetDynamicRecordsHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<PagedResult<DynamicEntityRecordDto>>> Handle(GetDynamicRecordsQuery query, CancellationToken ct)
    {
        var (items, total) = await _uow.Repository<DynamicEntityRecord>().GetPagedAsync(
            query.PageNumber, query.PageSize,
            predicate: r => r.EntityDefinitionId == query.EntityDefinitionId,
            orderBy: q => q.OrderByDescending(r => r.CreatedDate),
            disableTracking: true, cancellationToken: ct);

        return Result<PagedResult<DynamicEntityRecordDto>>.Success(new PagedResult<DynamicEntityRecordDto>
        {
            Items = items.Adapt<List<DynamicEntityRecordDto>>(),
            TotalCount = total, PageNumber = query.PageNumber, PageSize = query.PageSize
        });
    }
}
