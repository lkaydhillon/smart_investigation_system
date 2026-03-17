using SmartInvestigation.Domain.Common;
using SmartInvestigation.Domain.Enums;

namespace SmartInvestigation.Domain.Entities;

/// <summary>
/// Evidence item with integrity hash verification
/// </summary>
public class Evidence : BaseEntity
{
    public Guid CaseId { get; set; }
    public string EvidenceNumber { get; set; } = string.Empty;
    public EvidenceType Type { get; set; }
    public EvidenceStatus Status { get; set; } = EvidenceStatus.Collected;
    public string Description { get; set; } = string.Empty;
    public string? FileUrl { get; set; }
    public string? SHA256Hash { get; set; }
    public string? CollectedBy { get; set; }
    public DateTime CollectionDate { get; set; }
    public string? CollectionLocation { get; set; }
    public double? GpsLatitude { get; set; }
    public double? GpsLongitude { get; set; }
    public string? StorageLocation { get; set; }
    public Guid? CategoryId { get; set; }
    public string? Remarks { get; set; }
    public long? FileSizeBytes { get; set; }
    public string? MimeType { get; set; }

    public virtual Case Case { get; set; } = null!;
    public virtual EvidenceCategory? Category { get; set; }
    public virtual ICollection<EvidenceChainOfCustody> ChainOfCustody { get; set; } = new List<EvidenceChainOfCustody>();
    public virtual ICollection<ForensicLabRequest> ForensicRequests { get; set; } = new List<ForensicLabRequest>();
    public virtual DigitalEvidence? DigitalEvidence { get; set; }
}

/// <summary>
/// Chain of custody — legally required for evidence integrity
/// </summary>
public class EvidenceChainOfCustody : BaseEntity
{
    public Guid EvidenceId { get; set; }
    public string ReceivedFrom { get; set; } = string.Empty;
    public string ReceivedBy { get; set; } = string.Empty;
    public string? Location { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Purpose { get; set; }
    public string? Condition { get; set; }
    public string? SignatureUrl { get; set; }
    public string? Remarks { get; set; }

    public virtual Evidence Evidence { get; set; } = null!;
}

public class EvidenceCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public Guid? ParentCategoryId { get; set; }
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual EvidenceCategory? ParentCategory { get; set; }
    public virtual ICollection<EvidenceCategory> SubCategories { get; set; } = new List<EvidenceCategory>();
    public virtual ICollection<Evidence> Evidences { get; set; } = new List<Evidence>();
}

public class ForensicLabRequest : BaseEntity
{
    public Guid CaseId { get; set; }
    public Guid EvidenceId { get; set; }
    public string? LabName { get; set; }
    public string RequestType { get; set; } = string.Empty;
    public ForensicRequestStatus Status { get; set; } = ForensicRequestStatus.Pending;
    public CasePriority Priority { get; set; } = CasePriority.Medium;
    public DateTime RequestDate { get; set; }
    public DateTime? ExpectedDate { get; set; }
    public string? RequestedBy { get; set; }
    public string? Remarks { get; set; }

    public virtual Case Case { get; set; } = null!;
    public virtual Evidence Evidence { get; set; } = null!;
    public virtual ForensicReport? Report { get; set; }
}

public class ForensicReport : BaseEntity
{
    public Guid LabRequestId { get; set; }
    public string ReportNumber { get; set; } = string.Empty;
    public string? Findings { get; set; }
    public string? Conclusion { get; set; }
    public string? ReportFileUrl { get; set; }
    public DateTime ReportDate { get; set; }
    public string? AnalystName { get; set; }

    public virtual ForensicLabRequest LabRequest { get; set; } = null!;
}

/// <summary>
/// Digital evidence: CDR, phone dumps, CCTV footage, etc.
/// </summary>
public class DigitalEvidence : BaseEntity
{
    public Guid EvidenceId { get; set; }
    public string? DeviceType { get; set; }
    public string? DeviceMake { get; set; }
    public string? DeviceModel { get; set; }
    public string? IMEI { get; set; }
    public string? SerialNumber { get; set; }
    public string? ExtractedDataUrl { get; set; }
    public string? ExtractionTool { get; set; }
    public DateTime? ExtractionDate { get; set; }
    public string? HashBeforeExtraction { get; set; }
    public string? HashAfterExtraction { get; set; }
    public string? Remarks { get; set; }

    public virtual Evidence Evidence { get; set; } = null!;
}

public class SeizedProperty : BaseEntity
{
    public Guid CaseId { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid? CategoryId { get; set; }
    public decimal? EstimatedValue { get; set; }
    public string? StorageLocation { get; set; }
    public string? SeizureMemoNumber { get; set; }
    public DateTime SeizureDate { get; set; }
    public string? SeizedBy { get; set; }
    public string? Status { get; set; } = "InCustody";
    public string? DisposalRemarks { get; set; }
    public DateTime? DisposalDate { get; set; }
    public string? PhotoUrls { get; set; }

    public virtual Case Case { get; set; } = null!;
    public virtual PropertyCategory? Category { get; set; }
}

public class PropertyCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual ICollection<SeizedProperty> Properties { get; set; } = new List<SeizedProperty>();
}
