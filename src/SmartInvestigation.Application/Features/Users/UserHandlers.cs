using Mapster;
using MediatR;
using SmartInvestigation.Application.Common;
using SmartInvestigation.Domain.Entities;
using SmartInvestigation.Domain.Interfaces;

namespace SmartInvestigation.Application.Features.Users;

// ── Commands ──

public record CreateUserCommand(string Username, string Email, string FullName,
    string Rank, string BadgeNumber, string RoleId) : IRequest<Result<Guid>>;

public record AssignRoleCommand(Guid UserId, Guid RoleId) : IRequest<Result<bool>>;

// ── Queries ──

public record GetUsersQuery(string? Rank = null, Guid? PoliceStationId = null) : IRequest<Result<List<UserDto>>>;
public record GetRolesQuery() : IRequest<Result<List<RoleDto>>>;

// ── DTOs ──

public record UserDto(Guid Id, string Username, string Email, string FullName,
    string Rank, string BadgeNumber, bool IsActive);
public record RoleDto(Guid Id, string Name, string Description);

// ── Handlers ──

public class CreateUserHandler : IRequestHandler<CreateUserCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;
    public CreateUserHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<Guid>> Handle(CreateUserCommand cmd, CancellationToken ct)
    {
        var user = new User
        {
            Username = cmd.Username, Email = cmd.Email, FullName = cmd.FullName,
            Rank = cmd.Rank, BadgeNumber = cmd.BadgeNumber,
            PasswordHash = "hashed_default_password", // In a real app, generate securely
            IsActive = true
        };
        await _uow.Repository<User>().AddAsync(user, ct);
        
        // Assign role
        if (Guid.TryParse(cmd.RoleId, out var roleId))
        {
            await _uow.Repository<UserRole>().AddAsync(new UserRole { UserId = user.Id, RoleId = roleId }, ct);
        }

        await _uow.SaveChangesAsync(ct);
        return Result<Guid>.Created(user.Id);
    }
}

public class GetUsersHandler : IRequestHandler<GetUsersQuery, Result<List<UserDto>>>
{
    private readonly IUnitOfWork _uow;
    public GetUsersHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<List<UserDto>>> Handle(GetUsersQuery query, CancellationToken ct)
    {
        var items = await _uow.Repository<User>().GetAsync(
            predicate: u => (query.Rank == null || u.Rank == query.Rank) &&
                            (query.PoliceStationId == null || u.PoliceStationId == query.PoliceStationId),
            orderBy: q => q.OrderBy(u => u.Username),
            includeString: null,
            disableTracking: true, cancellationToken: ct);
        return Result<List<UserDto>>.Success(items.Adapt<List<UserDto>>());
    }
}

public class GetRolesHandler : IRequestHandler<GetRolesQuery, Result<List<RoleDto>>>
{
    private readonly IUnitOfWork _uow;
    public GetRolesHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<List<RoleDto>>> Handle(GetRolesQuery query, CancellationToken ct)
    {
        var items = await _uow.Repository<Role>().GetAsync(
            orderBy: q => q.OrderBy(r => r.Name),
            includeString: null,
            disableTracking: true, cancellationToken: ct);
        return Result<List<RoleDto>>.Success(items.Adapt<List<RoleDto>>());
    }
}
