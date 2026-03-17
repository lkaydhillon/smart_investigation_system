using SmartInvestigation.Domain.Common;
using SmartInvestigation.Domain.Enums;

namespace SmartInvestigation.Domain.Entities;

// ── Users & Access Control ──────────────────────────

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? PasswordSalt { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? BadgeNumber { get; set; }
    public string? Rank { get; set; }
    public string? Designation { get; set; }
    public Guid? PoliceStationId { get; set; }
    public Guid? DistrictId { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsLocked { get; set; }
    public int FailedLoginAttempts { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public DateTime? PasswordChangedDate { get; set; }
    public bool MustChangePassword { get; set; }
    public string? PreferredLanguage { get; set; } = "en";

    public virtual PoliceStation? PoliceStation { get; set; }
    public virtual District? District { get; set; }
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public virtual ICollection<UserSession> Sessions { get; set; } = new List<UserSession>();
    public virtual ICollection<UserDevice> Devices { get; set; } = new List<UserDevice>();
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public virtual UserPreference? Preferences { get; set; }
}

public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Level { get; set; } // Hierarchy: 1=Constable, 10=Admin
    public bool IsSystemRole { get; set; } // Cannot be deleted

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

public class UserRole : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
    public Guid? AssignedByUserId { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual Role Role { get; set; } = null!;
}

public class Permission : BaseEntity
{
    public string Code { get; set; } = string.Empty; // e.g., "cases.create", "evidence.delete"
    public string Name { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public string? Description { get; set; }

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

public class RolePermission : BaseEntity
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }

    public virtual Role Role { get; set; } = null!;
    public virtual Permission Permission { get; set; } = null!;
}

public class RefreshToken : BaseEntity
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? ReplacedByToken { get; set; }
    public string? RevokedReason { get; set; }
    public string? CreatedByIp { get; set; }
    public string? RevokedByIp { get; set; }
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt != null;
    public bool IsActive => !IsRevoked && !IsExpired;

    public virtual User User { get; set; } = null!;
}

public class UserSession : BaseEntity
{
    public Guid UserId { get; set; }
    public string? DeviceInfo { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime LoginAt { get; set; }
    public DateTime? LogoutAt { get; set; }
    public bool IsActive { get; set; } = true;
    public double? GpsLatitude { get; set; }
    public double? GpsLongitude { get; set; }

    public virtual User User { get; set; } = null!;
}

public class UserDevice : BaseEntity
{
    public Guid UserId { get; set; }
    public string DeviceId { get; set; } = string.Empty;
    public string? Platform { get; set; } // Android, iOS
    public string? DeviceModel { get; set; }
    public string? OSVersion { get; set; }
    public string? AppVersion { get; set; }
    public string? PushToken { get; set; }
    public DateTime? LastActiveDate { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual User User { get; set; } = null!;
}

public class UserPreference : BaseEntity
{
    public Guid UserId { get; set; }
    public string Theme { get; set; } = "light"; // light, dark
    public string? DashboardLayout { get; set; } // JSON
    public string? DefaultStationId { get; set; }
    public bool NotificationsEnabled { get; set; } = true;
    public bool EmailNotifications { get; set; } = true;
    public bool SMSNotifications { get; set; } = false;
    public string? PreferredLanguage { get; set; } = "en";
    public string? CustomSettings { get; set; } // JSON for extensibility

    public virtual User User { get; set; } = null!;
}
