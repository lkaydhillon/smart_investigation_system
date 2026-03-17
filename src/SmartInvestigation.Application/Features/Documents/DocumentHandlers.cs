using Mapster;
using MediatR;
using SmartInvestigation.Application.Common;
using SmartInvestigation.Application.DTOs;
using SmartInvestigation.Domain.Entities;
using SmartInvestigation.Domain.Interfaces;

namespace SmartInvestigation.Application.Features.Documents;

// ── Commands ──

public record CreateTemplateCommand(string Name, Guid? CategoryId, string? Description,
    string Format, string TemplateContent, string UserId) : IRequest<Result<Guid>>;

public record GenerateDocumentCommand(GenerateDocumentRequest Request, string UserId)
    : IRequest<Result<Guid>>;

// ── Queries ──

public record GetTemplatesQuery(Guid? CategoryId = null) : IRequest<Result<List<DocumentTemplateDto>>>;
public record GetGeneratedDocumentsQuery(Guid CaseId) : IRequest<Result<List<GeneratedDocumentDto>>>;

// ── DTOs ──

public record GeneratedDocumentDto(Guid Id, Guid? CaseId, string? Remarks, string? FileUrl, DateTime CreatedDate);

// ── Handlers ──

public class CreateTemplateHandler : IRequestHandler<CreateTemplateCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;
    public CreateTemplateHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<Guid>> Handle(CreateTemplateCommand cmd, CancellationToken ct)
    {
        var template = new DocumentTemplate
        {
            Name = cmd.Name, CategoryId = cmd.CategoryId, Description = cmd.Description,
            Format = cmd.Format, TemplateContent = cmd.TemplateContent, Version = 1, IsActive = true,
            CreatedBy = cmd.UserId
        };
        await _uow.Repository<DocumentTemplate>().AddAsync(template, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<Guid>.Created(template.Id);
    }
}

public class GenerateDocumentHandler : IRequestHandler<GenerateDocumentCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;
    public GenerateDocumentHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<Guid>> Handle(GenerateDocumentCommand cmd, CancellationToken ct)
    {
        var template = await _uow.Repository<DocumentTemplate>().GetByIdAsync(cmd.Request.TemplateId, ct);
        if (template == null) return Result<Guid>.NotFound("Template not found");

        var doc = new GeneratedDocument
        {
            CaseId = cmd.Request.CaseId,
            TemplateId = cmd.Request.TemplateId,
            Remarks = $"{template.Name} Document",
            FileUrl = "https://example.com/generated/doc.pdf",
            GeneratedByUserId = Guid.Parse(cmd.UserId),
            CreatedBy = cmd.UserId
        };

        await _uow.Repository<GeneratedDocument>().AddAsync(doc, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<Guid>.Created(doc.Id);
    }
}

public class GetTemplatesHandler : IRequestHandler<GetTemplatesQuery, Result<List<DocumentTemplateDto>>>
{
    private readonly IUnitOfWork _uow;
    public GetTemplatesHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<List<DocumentTemplateDto>>> Handle(GetTemplatesQuery query, CancellationToken ct)
    {
        var items = await _uow.Repository<DocumentTemplate>().GetAsync(
            predicate: t => (query.CategoryId == null || t.CategoryId == query.CategoryId) && t.IsActive,
            includeString: null,
            disableTracking: true, cancellationToken: ct);
        return Result<List<DocumentTemplateDto>>.Success(items.Adapt<List<DocumentTemplateDto>>());
    }
}

public class GetGeneratedDocumentsHandler : IRequestHandler<GetGeneratedDocumentsQuery, Result<List<GeneratedDocumentDto>>>
{
    private readonly IUnitOfWork _uow;
    public GetGeneratedDocumentsHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<List<GeneratedDocumentDto>>> Handle(GetGeneratedDocumentsQuery query, CancellationToken ct)
    {
        var items = await _uow.Repository<GeneratedDocument>().GetAsync(
            predicate: d => d.CaseId == query.CaseId,
            orderBy: q => q.OrderByDescending(d => d.CreatedDate),
            includeString: null,
            disableTracking: true, cancellationToken: ct);
        return Result<List<GeneratedDocumentDto>>.Success(items.Adapt<List<GeneratedDocumentDto>>());
    }
}
