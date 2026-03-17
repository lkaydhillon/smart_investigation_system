using SmartInvestigation.Domain.Common;
using SmartInvestigation.Domain.Enums;

namespace SmartInvestigation.Domain.Entities;

/// <summary>
/// IPC/BNS legal sections database
/// </summary>
public class LegalSection : BaseEntity
{
    public string Code { get; set; } = string.Empty; // e.g., "302", "376"
    public string Act { get; set; } = string.Empty; // "IPC", "BNS", "POCSO", etc.
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty; // Bailable, Non-Bailable, Cognizable
    public bool IsBailable { get; set; }
    public bool IsCognizable { get; set; }
    public string? MaxPenalty { get; set; }
    public string? MinPenalty { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Keywords { get; set; } // For AI matching

    public virtual ICollection<CaseLegalSection> CaseLegalSections { get; set; } = new List<CaseLegalSection>();
    public virtual ICollection<LegalDeadline> Deadlines { get; set; } = new List<LegalDeadline>();
}

public class CaseLegalSection : BaseEntity
{
    public Guid CaseId { get; set; }
    public Guid LegalSectionId { get; set; }
    public Guid AddedByUserId { get; set; }
    public DateTime AddedDate { get; set; }
    public bool IsAIRecommended { get; set; }
    public double? ConfidenceScore { get; set; }
    public string? Remarks { get; set; }

    public virtual Case Case { get; set; } = null!;
    public virtual LegalSection LegalSection { get; set; } = null!;
    public virtual User AddedByUser { get; set; } = null!;
}

/// <summary>
/// Statutory deadlines (POCSO timelines, default bail, etc.)
/// </summary>
public class LegalDeadline : BaseEntity
{
    public Guid? LegalSectionId { get; set; }
    public string DeadlineType { get; set; } = string.Empty;
    public int DurationDays { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Consequence { get; set; }
    public bool IsExtendable { get; set; }
    public int? MaxExtensionDays { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual LegalSection? LegalSection { get; set; }
}

/// <summary>
/// Court hearing tracking
/// </summary>
public class CourtHearing : BaseEntity
{
    public Guid CaseId { get; set; }
    public Guid CourtId { get; set; }
    public DateTime HearingDate { get; set; }
    public string? Purpose { get; set; }
    public string? Outcome { get; set; }
    public DateTime? NextHearingDate { get; set; }
    public string? JudgeName { get; set; }
    public string? ProsecutorName { get; set; }
    public string? DefenseLawyerName { get; set; }
    public Guid? AttendedByOfficerId { get; set; }
    public string? Remarks { get; set; }

    public virtual Case Case { get; set; } = null!;
    public virtual Court Court { get; set; } = null!;
    public virtual User? AttendedByOfficer { get; set; }
    public virtual ICollection<CourtOrder> Orders { get; set; } = new List<CourtOrder>();
}

public class CourtOrder : BaseEntity
{
    public Guid HearingId { get; set; }
    public CourtOrderType OrderType { get; set; }
    public string OrderText { get; set; } = string.Empty;
    public string? FileUrl { get; set; }
    public DateTime? ComplianceDeadline { get; set; }
    public bool IsComplied { get; set; }
    public DateTime? ComplianceDate { get; set; }
    public string? ComplianceRemarks { get; set; }

    public virtual CourtHearing Hearing { get; set; } = null!;
}

public class Court : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // District, Sessions, High Court
    public string? DistrictName { get; set; }
    public string? StateName { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual ICollection<CourtHearing> Hearings { get; set; } = new List<CourtHearing>();
}

/// <summary>
/// Final report / charge sheet
/// </summary>
public class Chargesheet : BaseEntity
{
    public Guid CaseId { get; set; }
    public string ChargesheetNumber { get; set; } = string.Empty;
    public DateTime FilingDate { get; set; }
    public ChargesheetStatus Status { get; set; } = ChargesheetStatus.Draft;
    public string? FileUrl { get; set; }
    public Guid FiledByUserId { get; set; }
    public Guid? ReviewedByUserId { get; set; }
    public string? ReviewRemarks { get; set; }
    public string? AIReviewFindings { get; set; } // JSON
    public bool IsSupplementary { get; set; }
    public Guid? OriginalChargesheetId { get; set; }

    public virtual Case Case { get; set; } = null!;
    public virtual User FiledByUser { get; set; } = null!;
    public virtual User? ReviewedByUser { get; set; }
    public virtual Chargesheet? OriginalChargesheet { get; set; }
    public virtual ICollection<ChargesheetPerson> Persons { get; set; } = new List<ChargesheetPerson>();
}

public class ChargesheetPerson : BaseEntity
{
    public Guid ChargesheetId { get; set; }
    public Guid PersonId { get; set; }
    public string? SectionsApplied { get; set; }
    public string? Status { get; set; }
    public string? Remarks { get; set; }

    public virtual Chargesheet Chargesheet { get; set; } = null!;
    public virtual Person Person { get; set; } = null!;
}

public class BailApplication : BaseEntity
{
    public Guid CaseId { get; set; }
    public Guid PersonId { get; set; }
    public Guid CourtId { get; set; }
    public DateTime ApplicationDate { get; set; }
    public BailStatus Status { get; set; } = BailStatus.Applied;
    public string? Conditions { get; set; }
    public decimal? BailAmount { get; set; }
    public string? SuretyDetails { get; set; }
    public DateTime? GrantDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public Guid? OpposingOfficerId { get; set; }
    public string? ProsecutionArguments { get; set; }
    public string? OrderFileUrl { get; set; }

    public virtual Case Case { get; set; } = null!;
    public virtual Person Person { get; set; } = null!;
    public virtual Court Court { get; set; } = null!;
    public virtual User? OpposingOfficer { get; set; }
}

public class Remand : BaseEntity
{
    public Guid CaseId { get; set; }
    public Guid PersonId { get; set; }
    public RemandType RemandType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid CourtId { get; set; }
    public string? JudgeName { get; set; }
    public string? CustodyLocation { get; set; }
    public string? OrderFileUrl { get; set; }
    public bool IsExtended { get; set; }
    public DateTime? ExtendedTill { get; set; }

    public virtual Case Case { get; set; } = null!;
    public virtual Person Person { get; set; } = null!;
    public virtual Court Court { get; set; } = null!;
}
