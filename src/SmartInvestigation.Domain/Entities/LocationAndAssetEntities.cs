using SmartInvestigation.Domain.Common;
using SmartInvestigation.Domain.Enums;

namespace SmartInvestigation.Domain.Entities;

public class PoliceStation : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public Guid DistrictId { get; set; }
    public string? Address { get; set; }
    public double? GpsLatitude { get; set; }
    public double? GpsLongitude { get; set; }
    public string? JurisdictionGeoJson { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public Guid? StationHouseOfficerId { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual District District { get; set; } = null!;
    public virtual User? StationHouseOfficer { get; set; }
    public virtual ICollection<User> Officers { get; set; } = new List<User>();
    public virtual ICollection<PatrolZone> PatrolZones { get; set; } = new List<PatrolZone>();
}

public class District : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public Guid StateId { get; set; }
    public Guid? SPUserId { get; set; }

    public virtual State State { get; set; } = null!;
    public virtual User? SP { get; set; }
    public virtual ICollection<PoliceStation> PoliceStations { get; set; } = new List<PoliceStation>();
}

public class State : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    public virtual ICollection<District> Districts { get; set; } = new List<District>();
}

public class CrimeScene : BaseEntity
{
    public Guid CaseId { get; set; }
    public string? Address { get; set; }
    public double? GpsLatitude { get; set; }
    public double? GpsLongitude { get; set; }
    public string? Description { get; set; }
    public string? SceneType { get; set; }
    public DateTime? DiscoveredDate { get; set; }
    public string? PhotoUrls { get; set; }

    public virtual Case Case { get; set; } = null!;
}

public class PatrolZone : BaseEntity
{
    public Guid StationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? BoundaryGeoJson { get; set; }
    public RiskLevel RiskLevel { get; set; } = RiskLevel.Low;
    public string? Description { get; set; }
    public string? AssignedPatrolTeam { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual PoliceStation Station { get; set; } = null!;
}

public class GeoFence : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Surveillance, Restricted, AlertZone
    public string? BoundaryGeoJson { get; set; }
    public double? AlertRadiusMeters { get; set; }
    public Guid? LinkedCaseId { get; set; }
    public Guid? LinkedPersonId { get; set; }
    public bool IsActive { get; set; } = true;
    public string? AlertRecipients { get; set; } // JSON array of user IDs

    public virtual Case? LinkedCase { get; set; }
    public virtual Person? LinkedPerson { get; set; }
}

// ── Vehicles & Weapons ──────────────────────────────

public class Vehicle : BaseEntity
{
    public Guid CaseId { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? Make { get; set; }
    public string? Model { get; set; }
    public string? Color { get; set; }
    public string? ChassisNumber { get; set; }
    public string? EngineNumber { get; set; }
    public int? YearOfManufacture { get; set; }
    public Guid? OwnerPersonId { get; set; }
    public string? Role { get; set; } // Used in crime, Seized, Recovery
    public string? CurrentStatus { get; set; }
    public string? PhotoUrls { get; set; }

    public virtual Case Case { get; set; } = null!;
    public virtual Person? Owner { get; set; }
    public virtual ICollection<VehicleInterception> Interceptions { get; set; } = new List<VehicleInterception>();
}

public class VehicleInterception : BaseEntity
{
    public Guid VehicleId { get; set; }
    public Guid InterceptedByUserId { get; set; }
    public DateTime DateTime { get; set; }
    public string? Location { get; set; }
    public double? GpsLatitude { get; set; }
    public double? GpsLongitude { get; set; }
    public string? Outcome { get; set; }
    public string? Remarks { get; set; }

    public virtual Vehicle Vehicle { get; set; } = null!;
    public virtual User InterceptedByUser { get; set; } = null!;
}

public class Weapon : BaseEntity
{
    public Guid CaseId { get; set; }
    public WeaponType Type { get; set; }
    public string? Make { get; set; }
    public string? Model { get; set; }
    public string? Caliber { get; set; }
    public string? SerialNumber { get; set; }
    public string? LicenseNumber { get; set; }
    public Guid? RecoveredFromPersonId { get; set; }
    public string? RecoveryLocation { get; set; }
    public DateTime? RecoveryDate { get; set; }
    public string? Status { get; set; }
    public string? PhotoUrls { get; set; }

    public virtual Case Case { get; set; } = null!;
    public virtual Person? RecoveredFromPerson { get; set; }
    public virtual WeaponBallistic? BallisticReport { get; set; }
}

public class WeaponBallistic : BaseEntity
{
    public Guid WeaponId { get; set; }
    public string? BallisticReportUrl { get; set; }
    public string? ReportNumber { get; set; }
    public string? Findings { get; set; }
    public string? MatchedCaseIds { get; set; } // JSON array
    public DateTime? ReportDate { get; set; }

    public virtual Weapon Weapon { get; set; } = null!;
}
