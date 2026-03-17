using SmartInvestigation.Domain.Common;
using SmartInvestigation.Domain.Enums;

namespace SmartInvestigation.Domain.Entities;

/// <summary>
/// First Information Report — the starting point of any criminal investigation
/// </summary>
public class FIR : BaseEntity
{
    public string FIRNumber { get; set; } = string.Empty;
    public Guid PoliceStationId { get; set; }
    public DateTime DateFiled { get; set; }
    public DateTime DateOfOccurrence { get; set; }
    public DateTime? TimeOfOccurrence { get; set; }
    public string PlaceOfOccurrence { get; set; } = string.Empty;
    public double? GpsLatitude { get; set; }
    public double? GpsLongitude { get; set; }
    public string Description { get; set; } = string.Empty;
    public FIRStatus Status { get; set; } = FIRStatus.Registered;
    public Guid? ComplainantPersonId { get; set; }
    public Guid? IOAssignedUserId { get; set; }
    public string? ActsAndSections { get; set; }
    public DateTime? ClosureDate { get; set; }
    public string? ClosureRemarks { get; set; }

    // Navigation
    public virtual PoliceStation PoliceStation { get; set; } = null!;
    public virtual Person? Complainant { get; set; }
    public virtual User? IOAssigned { get; set; }
    public virtual ICollection<Case> Cases { get; set; } = new List<Case>();
}

/// <summary>
/// Core investigation case with full lifecycle tracking
/// </summary>
public class Case : BaseEntity
{
    public string CaseNumber { get; set; } = string.Empty;
    public Guid? FIRId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public CaseStatus Status { get; set; } = CaseStatus.Registered;
    public CasePriority Priority { get; set; } = CasePriority.Medium;
    public Guid? CrimeTypeId { get; set; }
    public Guid? CrimeSubTypeId { get; set; }
    public Guid PoliceStationId { get; set; }
    public Guid DistrictId { get; set; }
    public Guid? AssignedOfficerId { get; set; }
    public Guid? SupervisingOfficerId { get; set; }
    public DateTime DateOfRegistration { get; set; } = DateTime.UtcNow;
    public DateTime? DateOfClosure { get; set; }
    public string? ClosureRemarks { get; set; }
    public bool IsHighProfile { get; set; }
    public bool IsSensitive { get; set; }
    public string? Tags { get; set; }

    // Navigation
    public virtual FIR? FIR { get; set; }
    public virtual CrimeType? CrimeType { get; set; }
    public virtual CrimeSubType? CrimeSubType { get; set; }
    public virtual PoliceStation PoliceStation { get; set; } = null!;
    public virtual District District { get; set; } = null!;
    public virtual User? AssignedOfficer { get; set; }
    public virtual User? SupervisingOfficer { get; set; }
    public virtual ICollection<CaseComplaint> CaseComplaints { get; set; } = new List<CaseComplaint>();
    public virtual ICollection<CasePerson> CasePersons { get; set; } = new List<CasePerson>();
    public virtual ICollection<CaseStatusHistory> StatusHistory { get; set; } = new List<CaseStatusHistory>();
    public virtual ICollection<CaseTransfer> Transfers { get; set; } = new List<CaseTransfer>();
    public virtual ICollection<CaseNote> Notes { get; set; } = new List<CaseNote>();
    public virtual ICollection<Evidence> Evidences { get; set; } = new List<Evidence>();
    public virtual ICollection<CaseLegalSection> LegalSections { get; set; } = new List<CaseLegalSection>();
    public virtual ICollection<InvestigationStep> InvestigationSteps { get; set; } = new List<InvestigationStep>();
    public virtual ICollection<CaseDiary> CaseDiaries { get; set; } = new List<CaseDiary>();
    public virtual ICollection<CourtHearing> CourtHearings { get; set; } = new List<CourtHearing>();
    public virtual ICollection<Chargesheet> Chargesheets { get; set; } = new List<Chargesheet>();
    public virtual ICollection<CrimeScene> CrimeScenes { get; set; } = new List<CrimeScene>();
    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    public virtual ICollection<Weapon> Weapons { get; set; } = new List<Weapon>();
    public virtual ICollection<GeneratedDocument> Documents { get; set; } = new List<GeneratedDocument>();
    public virtual ICollection<CustomFieldValue> CustomFieldValues { get; set; } = new List<CustomFieldValue>();
}

/// <summary>
/// Citizen/officer complaint entry with media and GPS support
/// </summary>
public class Complaint : BaseEntity
{
    public string ComplaintNumber { get; set; } = string.Empty;
    public ComplaintType Type { get; set; }
    public ComplaintStatus Status { get; set; } = ComplaintStatus.Received;
    public string Description { get; set; } = string.Empty;
    public string? ComplainantName { get; set; }
    public string? ComplainantPhone { get; set; }
    public string? ComplainantAddress { get; set; }
    public Guid? ComplainantPersonId { get; set; }
    public double? GpsLatitude { get; set; }
    public double? GpsLongitude { get; set; }
    public string? LocationAddress { get; set; }
    public DateTime DateOfIncident { get; set; }
    public string? MediaAttachments { get; set; } // JSON array of file URLs
    public string? VoiceRecordingUrl { get; set; }
    public string? TranscribedText { get; set; }
    public Guid? ReceivedByUserId { get; set; }
    public Guid PoliceStationId { get; set; }
    public bool IsUrgent { get; set; }
    public string? RejectionReason { get; set; }
    public bool IsOfflineEntry { get; set; }
    public DateTime? SyncedAt { get; set; }

    // Navigation
    public virtual Person? ComplainantPerson { get; set; }
    public virtual User? ReceivedByUser { get; set; }
    public virtual PoliceStation PoliceStation { get; set; } = null!;
    public virtual ICollection<CaseComplaint> CaseComplaints { get; set; } = new List<CaseComplaint>();
    public virtual ICollection<DocumentAttachment> Attachments { get; set; } = new List<DocumentAttachment>();
}

public class CaseComplaint : BaseEntity
{
    public Guid CaseId { get; set; }
    public Guid ComplaintId { get; set; }
    public string? Remarks { get; set; }

    public virtual Case Case { get; set; } = null!;
    public virtual Complaint Complaint { get; set; } = null!;
}

public class CaseStatusHistory : BaseEntity
{
    public Guid CaseId { get; set; }
    public CaseStatus OldStatus { get; set; }
    public CaseStatus NewStatus { get; set; }
    public string? Remarks { get; set; }
    public Guid ChangedByUserId { get; set; }

    public virtual Case Case { get; set; } = null!;
    public virtual User ChangedByUser { get; set; } = null!;
}

public class CaseTransfer : BaseEntity
{
    public Guid CaseId { get; set; }
    public Guid FromOfficerId { get; set; }
    public Guid ToOfficerId { get; set; }
    public Guid? FromStationId { get; set; }
    public Guid? ToStationId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime TransferDate { get; set; }
    public bool IsApproved { get; set; }
    public Guid? ApprovedByUserId { get; set; }

    public virtual Case Case { get; set; } = null!;
    public virtual User FromOfficer { get; set; } = null!;
    public virtual User ToOfficer { get; set; } = null!;
    public virtual PoliceStation? FromStation { get; set; }
    public virtual PoliceStation? ToStation { get; set; }
    public virtual User? ApprovedByUser { get; set; }
}

public class CaseMerge : BaseEntity
{
    public Guid PrimaryCaseId { get; set; }
    public Guid MergedCaseId { get; set; }
    public Guid MergedByUserId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime MergeDate { get; set; }

    public virtual Case PrimaryCase { get; set; } = null!;
    public virtual Case MergedCase { get; set; } = null!;
    public virtual User MergedByUser { get; set; } = null!;
}

public class CaseNote : BaseEntity
{
    public Guid CaseId { get; set; }
    public Guid OfficerId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsPrivate { get; set; }
    public string? AttachmentUrls { get; set; }

    public virtual Case Case { get; set; } = null!;
    public virtual User Officer { get; set; } = null!;
}
