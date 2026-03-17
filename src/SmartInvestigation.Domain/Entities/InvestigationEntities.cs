using SmartInvestigation.Domain.Common;
using SmartInvestigation.Domain.Enums;

namespace SmartInvestigation.Domain.Entities;

/// <summary>
/// Actual investigation progress tracking against SOP steps
/// </summary>
public class InvestigationStep : BaseEntity
{
    public Guid CaseId { get; set; }
    public Guid? SOPStepId { get; set; }
    public string StepTitle { get; set; } = string.Empty;
    public string? StepDescription { get; set; }
    public InvestigationStepStatus Status { get; set; } = InvestigationStepStatus.Pending;
    public Guid? AssignedToUserId { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? CompletionRemarks { get; set; }
    public int StepOrder { get; set; }
    public bool IsMandatory { get; set; }

    public virtual Case Case { get; set; } = null!;
    public virtual SOPStep? SOPStep { get; set; }
    public virtual User? AssignedToUser { get; set; }
}

/// <summary>
/// SOP template per crime type — admin configurable
/// </summary>
public class InvestigationSOP : BaseEntity
{
    public Guid CrimeTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Version { get; set; } = 1;
    public bool IsActive { get; set; } = true;
    public Guid? ApprovedByUserId { get; set; }
    public DateTime? ApprovalDate { get; set; }

    public virtual CrimeType CrimeType { get; set; } = null!;
    public virtual User? ApprovedByUser { get; set; }
    public virtual ICollection<SOPStep> Steps { get; set; } = new List<SOPStep>();
}

public class SOPStep : BaseEntity
{
    public Guid SOPId { get; set; }
    public int StepNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? DetailedInstructions { get; set; }
    public bool IsMandatory { get; set; } = true;
    public int? DeadlineDays { get; set; }
    public string? RequiredDocuments { get; set; }
    public string? ChecklistItems { get; set; } // JSON array

    public virtual InvestigationSOP SOP { get; set; } = null!;
    public virtual ICollection<InvestigationStep> InvestigationSteps { get; set; } = new List<InvestigationStep>();
}

/// <summary>
/// Investigation team for a case
/// </summary>
public class InvestigationTeam : BaseEntity
{
    public Guid CaseId { get; set; }
    public Guid LeadOfficerId { get; set; }
    public DateTime FormationDate { get; set; }
    public string? TeamName { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual Case Case { get; set; } = null!;
    public virtual User LeadOfficer { get; set; } = null!;
    public virtual ICollection<InvestigationTeamMember> Members { get; set; } = new List<InvestigationTeamMember>();
}

public class InvestigationTeamMember : BaseEntity
{
    public Guid TeamId { get; set; }
    public Guid UserId { get; set; }
    public string? Role { get; set; }
    public DateTime JoinDate { get; set; }
    public DateTime? LeaveDate { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual InvestigationTeam Team { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}

/// <summary>
/// Interrogation records — linked to case and person
/// </summary>
public class Interrogation : BaseEntity
{
    public Guid CaseId { get; set; }
    public Guid PersonId { get; set; }
    public Guid InterrogatorUserId { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
    public string? Location { get; set; }
    public string? Summary { get; set; }
    public string? TranscriptText { get; set; }
    public string? VideoUrl { get; set; }
    public string? AudioUrl { get; set; }
    public bool IsLawyerPresent { get; set; }
    public string? LawyerName { get; set; }
    public string? AIGeneratedQuestions { get; set; } // JSON

    public virtual Case Case { get; set; } = null!;
    public virtual Person Person { get; set; } = null!;
    public virtual User Interrogator { get; set; } = null!;
}

/// <summary>
/// 161/164 CrPC witness statements
/// </summary>
public class WitnessStatement : BaseEntity
{
    public Guid CaseId { get; set; }
    public Guid PersonId { get; set; }
    public string StatementText { get; set; } = string.Empty;
    public Guid RecordedByUserId { get; set; }
    public DateTime RecordedDate { get; set; }
    public string? StatementType { get; set; } // 161 CrPC, 164 CrPC
    public string? SignatureUrl { get; set; }
    public string? AudioRecordingUrl { get; set; }
    public string? Location { get; set; }
    public bool IsRetracted { get; set; }
    public string? RetractionRemarks { get; set; }

    public virtual Case Case { get; set; } = null!;
    public virtual Person Person { get; set; } = null!;
    public virtual User RecordedByUser { get; set; } = null!;
}

public class SurveillanceRecord : BaseEntity
{
    public Guid CaseId { get; set; }
    public Guid? PersonId { get; set; }
    public SurveillanceType Type { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Findings { get; set; }
    public string? AuthorizationReference { get; set; }
    public Guid AuthorizedByUserId { get; set; }
    public string? Remarks { get; set; }

    public virtual Case Case { get; set; } = null!;
    public virtual Person? Person { get; set; }
    public virtual User AuthorizedByUser { get; set; } = null!;
}

/// <summary>
/// Daily case diary — mandatory investigation record
/// </summary>
public class CaseDiary : BaseEntity
{
    public Guid CaseId { get; set; }
    public DateTime EntryDate { get; set; }
    public Guid OfficerId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int EntryNumber { get; set; }
    public bool IsSubmittedToSupervisor { get; set; }
    public DateTime? SubmissionDate { get; set; }
    public Guid? ReviewedByUserId { get; set; }
    public string? ReviewRemarks { get; set; }
    public string? AttachmentUrls { get; set; }

    public virtual Case Case { get; set; } = null!;
    public virtual User Officer { get; set; } = null!;
    public virtual User? ReviewedByUser { get; set; }
}

/// <summary>
/// Crime scene documentation with GPS, sketches, photos
/// </summary>
public class SceneSurvey : BaseEntity
{
    public Guid CaseId { get; set; }
    public DateTime SurveyDate { get; set; }
    public Guid OfficerId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? SketchUrl { get; set; }
    public string? PhotoUrls { get; set; }
    public string? VideoUrl { get; set; }
    public double? GpsLatitude { get; set; }
    public double? GpsLongitude { get; set; }
    public string? WeatherConditions { get; set; }
    public string? Observations { get; set; }
    public string? EvidenceCollectionNotes { get; set; }

    public virtual Case Case { get; set; } = null!;
    public virtual User Officer { get; set; } = null!;
}
