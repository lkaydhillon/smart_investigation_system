using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartInvestigation.Application.DTOs;
using SmartInvestigation.Domain.Entities;
using SmartInvestigation.Infrastructure.Persistence;
using SmartInvestigation.Infrastructure.Services;

namespace SmartInvestigation.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IJwtTokenService _jwtService;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext context, IJwtTokenService jwtService, IConfiguration config)
    {
        _context = context;
        _jwtService = jwtService;
        _config = config;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role).ThenInclude(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Username == request.Username && u.IsActive);

        if (user == null || !VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
            return Unauthorized(new { error = "Invalid username or password" });

        if (user.IsLocked)
            return Unauthorized(new { error = "Account is locked. Contact administrator." });

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Code)
            .Distinct().ToList();

        var accessToken = _jwtService.GenerateAccessToken(user, roles, permissions);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Save refresh token
        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(int.Parse(_config["Jwt:RefreshTokenExpirationDays"] ?? "7")),
            CreatedByIp = HttpContext.Connection.RemoteIpAddress?.ToString(),
            CreatedBy = user.Id.ToString()
        };
        _context.RefreshTokens.Add(refreshTokenEntity);

        // Update login info
        user.LastLoginDate = DateTime.UtcNow;
        user.FailedLoginAttempts = 0;

        // Track session
        _context.UserSessions.Add(new UserSession
        {
            UserId = user.Id,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = Request.Headers.UserAgent.ToString(),
            LoginAt = DateTime.UtcNow,
            IsActive = true,
            CreatedBy = user.Id.ToString()
        });

        // Register device if provided
        if (!string.IsNullOrEmpty(request.DeviceId))
        {
            var existing = await _context.UserDevices.FirstOrDefaultAsync(d => d.UserId == user.Id && d.DeviceId == request.DeviceId);
            if (existing == null)
            {
                _context.UserDevices.Add(new UserDevice
                {
                    UserId = user.Id,
                    DeviceId = request.DeviceId,
                    LastActiveDate = DateTime.UtcNow,
                    IsActive = true,
                    CreatedBy = user.Id.ToString()
                });
            }
            else existing.LastActiveDate = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        var expiresAt = DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:AccessTokenExpirationMinutes"] ?? "60"));
        var userDto = new UserDto(user.Id, user.Username, user.Email, user.FullName,
            user.BadgeNumber, user.Rank, user.Designation, user.PoliceStationId, null,
            user.ProfilePhotoUrl, roles, permissions);

        return Ok(new LoginResponse(accessToken, refreshToken, expiresAt, userDto));
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var principal = _jwtService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null) return Unauthorized(new { error = "Invalid token" });

        var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? "");
        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == request.RefreshToken && t.UserId == userId);

        if (storedToken == null || !storedToken.IsActive)
            return Unauthorized(new { error = "Invalid refresh token" });

        // Revoke old token
        storedToken.RevokedAt = DateTime.UtcNow;
        storedToken.RevokedByIp = HttpContext.Connection.RemoteIpAddress?.ToString();

        var user = await _context.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role).ThenInclude(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return Unauthorized(new { error = "User not found" });

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var permissions = user.UserRoles.SelectMany(ur => ur.Role.RolePermissions).Select(rp => rp.Permission.Code).Distinct().ToList();

        var newAccessToken = _jwtService.GenerateAccessToken(user, roles, permissions);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        storedToken.ReplacedByToken = newRefreshToken;
        _context.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id, Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(int.Parse(_config["Jwt:RefreshTokenExpirationDays"] ?? "7")),
            CreatedBy = user.Id.ToString()
        });

        await _context.SaveChangesAsync();
        var expiresAt = DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:AccessTokenExpirationMinutes"] ?? "60"));

        return Ok(new { accessToken = newAccessToken, refreshToken = newRefreshToken, expiresAt });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "");
        var activeSessions = await _context.UserSessions.Where(s => s.UserId == userId && s.IsActive).ToListAsync();
        foreach (var session in activeSessions) { session.IsActive = false; session.LogoutAt = DateTime.UtcNow; }
        await _context.SaveChangesAsync();
        return Ok(new { message = "Logged out successfully" });
    }

    private static bool VerifyPassword(string password, string storedHash, string? storedSalt)
    {
        if (string.IsNullOrEmpty(storedSalt))
            return BCryptVerify(password, storedHash);

        using var hmac = new HMACSHA512(Convert.FromBase64String(storedSalt));
        var computedHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
        return computedHash == storedHash;
    }

    private static bool BCryptVerify(string password, string hash)
    {
        try { return BCrypt.Net.BCrypt.Verify(password, hash); }
        catch { return false; }
    }
}
