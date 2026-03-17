using Mapster;
using MediatR;
using SmartInvestigation.Application.Common;
using SmartInvestigation.Application.DTOs;
using SmartInvestigation.Domain.Entities;
using SmartInvestigation.Domain.Enums;
using SmartInvestigation.Domain.Interfaces;

namespace SmartInvestigation.Application.Features.People;

// ── Commands ──

public record CreatePersonCommand(string FullName, string? FatherName, Gender? Gender,
    DateTime? DateOfBirth, string? Nationality, string? AadhaarHash, string? IdentificationMarks,
    string UserId) : IRequest<Result<PersonSummaryDto>>;

public record LinkPersonToCaseCommand(Guid CaseId, Guid PersonId, PersonRole Role,
    bool IsMainAccused, string UserId) : IRequest<Result<bool>>;

public record AddPersonAddressCommand(Guid PersonId, string AddressType, string? AddressLine1,
    string? City, string? State, string? PinCode, bool IsCurrent, string UserId) : IRequest<Result<bool>>;

public record AddPersonContactCommand(Guid PersonId, string ContactType, string Value,
    bool IsPrimary, string UserId) : IRequest<Result<bool>>;

public record UpdatePersonStatusInCaseCommand(Guid CasePersonId, PersonStatus Status,
    DateTime? ArrestDate, string? Remarks, string UserId) : IRequest<Result<bool>>;

// ── Queries ──

public record GetPersonByIdQuery(Guid Id) : IRequest<Result<PersonSummaryDto>>;
public record SearchPersonsQuery(string? SearchTerm, int PageNumber = 1, int PageSize = 20) : IRequest<Result<PagedResult<PersonSummaryDto>>>;
public record GetPersonsByCaseQuery(Guid CaseId) : IRequest<Result<List<CasePersonDto>>>;

// ── DTOs ──

public record PersonSummaryDto(Guid Id, string FullName, string? FatherName, Gender? Gender,
    DateTime? DateOfBirth, string? Nationality, bool IsRepeatOffender, int CaseCount, DateTime CreatedDate);

public record PersonDetailDto(Guid Id, string FullName, string? FatherName, Gender? Gender,
    DateTime? DateOfBirth, string? Nationality, string? IdentificationMarks, bool IsRepeatOffender,
    List<PersonAddressDto> Addresses, List<PersonContactDto> Contacts, List<PersonCaseDto> Cases);

public record PersonAddressDto(Guid Id, string? AddressType, string? AddressLine1, string? City,
    string? State, string? PinCode, bool IsCurrent);

public record PersonContactDto(Guid Id, string ContactType, string Value, bool IsPrimary);

public record PersonCaseDto(Guid CaseId, string CaseNumber, PersonRole Role, PersonStatus Status);

// ── Handlers ──

public class CreatePersonHandler : IRequestHandler<CreatePersonCommand, Result<PersonSummaryDto>>
{
    private readonly IUnitOfWork _uow;
    public CreatePersonHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<PersonSummaryDto>> Handle(CreatePersonCommand cmd, CancellationToken ct)
    {
        var person = new Person
        {
            FullName = cmd.FullName,
            FatherName = cmd.FatherName,
            Gender = cmd.Gender ?? Gender.Male,
            DateOfBirth = cmd.DateOfBirth,
            NationalityCode = cmd.Nationality,
            AadhaarHash = cmd.AadhaarHash,
            IdentificationMarks = cmd.IdentificationMarks,
            CreatedBy = cmd.UserId
        };

        await _uow.Repository<Person>().AddAsync(person, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<PersonSummaryDto>.Created(person.Adapt<PersonSummaryDto>());
    }
}

public class LinkPersonToCaseHandler : IRequestHandler<LinkPersonToCaseCommand, Result<bool>>
{
    private readonly IUnitOfWork _uow;
    public LinkPersonToCaseHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<bool>> Handle(LinkPersonToCaseCommand cmd, CancellationToken ct)
    {
        var exists = await _uow.Repository<CasePerson>().ExistsAsync(
            cp => cp.CaseId == cmd.CaseId && cp.PersonId == cmd.PersonId && cp.Role == cmd.Role, ct);
        if (exists) return Result<bool>.Failure("Person already linked with this role");

        await _uow.Repository<CasePerson>().AddAsync(new CasePerson
        {
            CaseId = cmd.CaseId,
            PersonId = cmd.PersonId,
            Role = cmd.Role,
            IsMainAccused = cmd.IsMainAccused,
            Status = PersonStatus.Active,
            CreatedBy = cmd.UserId
        }, ct);

        await _uow.SaveChangesAsync(ct);
        return Result<bool>.Created(true);
    }
}

public class AddPersonAddressHandler : IRequestHandler<AddPersonAddressCommand, Result<bool>>
{
    private readonly IUnitOfWork _uow;
    public AddPersonAddressHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<bool>> Handle(AddPersonAddressCommand cmd, CancellationToken ct)
    {
        var address = new PersonAddress
        {
            PersonId = cmd.PersonId,
            AddressType = cmd.IsCurrent ? "Current" : "Permanent",
            HouseNumber = cmd.AddressLine1,
            // Assuming AddressLine2 and Country are not part of the current command or are to be added later
            // Street = cmd.AddressLine2, 
            City = cmd.City,
            StateName = cmd.State,
            PinCode = cmd.PinCode,
            // Country = cmd.Country, 
            IsPrimary = cmd.IsCurrent,
            CreatedBy = cmd.UserId
        };
        await _uow.Repository<PersonAddress>().AddAsync(address, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<bool>.Created(true);
    }
}

public class AddPersonContactHandler : IRequestHandler<AddPersonContactCommand, Result<bool>>
{
    private readonly IUnitOfWork _uow;
    public AddPersonContactHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<bool>> Handle(AddPersonContactCommand cmd, CancellationToken ct)
    {
        await _uow.Repository<PersonContact>().AddAsync(new PersonContact
        {
            PersonId = cmd.PersonId,
            ContactType = cmd.ContactType,
            Value = cmd.Value,
            IsPrimary = cmd.IsPrimary,
            CreatedBy = cmd.UserId
        }, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<bool>.Created(true);
    }
}

public class UpdatePersonStatusInCaseHandler : IRequestHandler<UpdatePersonStatusInCaseCommand, Result<bool>>
{
    private readonly IUnitOfWork _uow;
    public UpdatePersonStatusInCaseHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<bool>> Handle(UpdatePersonStatusInCaseCommand cmd, CancellationToken ct)
    {
        var cp = await _uow.Repository<CasePerson>().GetByIdAsync(cmd.CasePersonId, ct);
        if (cp == null) return Result<bool>.NotFound("Case person record not found");

        cp.Status = cmd.Status;
        cp.ArrestDate = cmd.ArrestDate;
        cp.ModifiedBy = cmd.UserId;
        _uow.Repository<CasePerson>().Update(cp);
        await _uow.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}

public class SearchPersonsHandler : IRequestHandler<SearchPersonsQuery, Result<PagedResult<PersonSummaryDto>>>
{
    private readonly IUnitOfWork _uow;
    public SearchPersonsHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<PagedResult<PersonSummaryDto>>> Handle(SearchPersonsQuery query, CancellationToken ct)
    {
        var (items, total) = await _uow.Repository<Person>().GetPagedAsync(
            query.PageNumber, query.PageSize,
            predicate: p => string.IsNullOrEmpty(query.SearchTerm) ||
                p.FullName.Contains(query.SearchTerm) ||
                (p.FatherName != null && p.FatherName.Contains(query.SearchTerm)),
            orderBy: q => q.OrderByDescending(p => p.CreatedDate),
            disableTracking: true, cancellationToken: ct);

        return Result<PagedResult<PersonSummaryDto>>.Success(new PagedResult<PersonSummaryDto>
        {
            Items = items.Adapt<List<PersonSummaryDto>>(),
            TotalCount = total, PageNumber = query.PageNumber, PageSize = query.PageSize
        });
    }
}

public class GetPersonByIdHandler : IRequestHandler<GetPersonByIdQuery, Result<PersonSummaryDto>>
{
    private readonly IUnitOfWork _uow;
    public GetPersonByIdHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<PersonSummaryDto>> Handle(GetPersonByIdQuery query, CancellationToken ct)
    {
        var person = await _uow.Repository<Person>().GetByIdAsync(query.Id, ct);
        if (person == null) return Result<PersonSummaryDto>.NotFound();
        return Result<PersonSummaryDto>.Success(person.Adapt<PersonSummaryDto>());
    }
}

public record UpdatePersonCommand(Guid Id, string FullName, string? FatherName, Gender? Gender,
    DateTime? DateOfBirth, string? Nationality, string? AadhaarHash, string? IdentificationMarks,
    string UserId) : IRequest<Result<PersonSummaryDto>>;

public class UpdatePersonCommandHandler : IRequestHandler<UpdatePersonCommand, Result<PersonSummaryDto>>
{
    private readonly IUnitOfWork _uow;
    public UpdatePersonCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<PersonSummaryDto>> Handle(UpdatePersonCommand cmd, CancellationToken ct)
    {
        var person = await _uow.Repository<Person>().GetByIdAsync(cmd.Id, ct);
        if (person == null) return Result<PersonSummaryDto>.NotFound();

        person.FullName = cmd.FullName;
        person.FatherName = cmd.FatherName;
        person.Gender = cmd.Gender ?? person.Gender;
        person.DateOfBirth = cmd.DateOfBirth;
        person.NationalityCode = cmd.Nationality;
        person.AadhaarHash = cmd.AadhaarHash;
        person.IdentificationMarks = cmd.IdentificationMarks;
        person.ModifiedBy = cmd.UserId;

        _uow.Repository<Person>().Update(person);
        await _uow.SaveChangesAsync(ct);
        return Result<PersonSummaryDto>.Success(person.Adapt<PersonSummaryDto>());
    }
}

public record DeletePersonCommand(Guid Id, string UserId) : IRequest<Result<bool>>;

public class DeletePersonCommandHandler : IRequestHandler<DeletePersonCommand, Result<bool>>
{
    private readonly IUnitOfWork _uow;
    public DeletePersonCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<bool>> Handle(DeletePersonCommand cmd, CancellationToken ct)
    {
        var person = await _uow.Repository<Person>().GetByIdAsync(cmd.Id, ct);
        if (person == null) return Result<bool>.NotFound();

        _uow.Repository<Person>().Delete(person);
        await _uow.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}

