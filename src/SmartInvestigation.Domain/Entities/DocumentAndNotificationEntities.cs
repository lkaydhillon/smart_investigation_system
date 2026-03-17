using SmartInvestigation.Domain.Common;
using SmartInvestigation.Domain.Enums;

namespace SmartInvestigation.Domain.Entities;

// ── Document Templates & Generation ──────────────────

public class DocumentTemplate : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public Guid? CategoryId { get; set; }
    public string TemplateContent { get; set; } = string.Empty;
    public string? Placeholders { get; set; } // JSON: ["{{CaseNumber}}", "{{OfficerName}}"]
    public int Version { get; set; } = 1;
    public string? Format { get; set; } = "HTML"; // HTML, DOCX, PDF
    public bool IsActive { get; set; } = true;
    public string? Description { get; set; }

    public virtual DocumentCategory? Category { get; set; }
    public virtual ICollection<GeneratedDocument> GeneratedDocuments { get; set; } = new List<GeneratedDocument>();
}

public class GeneratedDocument : BaseEntity
{
    public Guid? CaseId { get; set; }
    public Guid TemplateId { get; set; }
    public Guid GeneratedByUserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? FileUrl { get; set; }
    public DocumentStatus Status { get; set; } = DocumentStatus.Draft;
    public Guid? SignedByUserId { get; set; }
    public DateTime? SignedDate { get; set; }
    public string? DigitalSignatureHash { get; set; }
    public string? Remarks { get; set; }

    public virtual Case? Case { get; set; }
    public virtual DocumentTemplate Template { get; set; } = null!;
    public virtual User GeneratedByUser { get; set; } = null!;
    public virtual User? SignedByUser { get; set; }
}

public class DocumentAttachment : BaseEntity
{
    public string ParentType { get; set; } = string.Empty; // Case, Complaint, Evidence, etc.
    public Guid ParentId { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string? MimeType { get; set; }
    public string? SHA256Hash { get; set; }
    public string? Description { get; set; }
    public Guid UploadedByUserId { get; set; }

    public virtual User UploadedByUser { get; set; } = null!;
}

public class DocumentCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual ICollection<DocumentTemplate> Templates { get; set; } = new List<DocumentTemplate>();
}

// ── Notifications & Alerts ──────────────────────────

public class Notification : BaseEntity
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public NotificationPriority Priority { get; set; } = NotificationPriority.Medium;
    public bool IsRead { get; set; }
    public DateTime? ReadDate { get; set; }
    public string? ActionUrl { get; set; }
    public string? EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public DateTime? ExpiresAt { get; set; }

    public virtual User User { get; set; } = null!;
}

public class NotificationTemplate : BaseEntity
{
    public string EventType { get; set; } = string.Empty;
    public string TitleTemplate { get; set; } = string.Empty;
    public string BodyTemplate { get; set; } = string.Empty;
    public string Channel { get; set; } = "InApp"; // InApp, SMS, Email, Push
    public bool IsActive { get; set; } = true;
    public NotificationPriority DefaultPriority { get; set; } = NotificationPriority.Medium;
}

public class AlertRule : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string ConditionExpression { get; set; } = string.Empty; // JSON rule definition
    public AlertSeverity Severity { get; set; }
    public string? RecipientRoles { get; set; } // JSON array of role names
    public string? RecipientUserIds { get; set; } // JSON array of specific users
    public bool IsActive { get; set; } = true;
    public string? NotificationTemplateKey { get; set; }
    public int? CooldownMinutes { get; set; }
    public DateTime? LastTriggered { get; set; }
}

public class SMSLog : BaseEntity
{
    public string Recipient { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public DateTime? SentDate { get; set; }
    public string? ErrorMessage { get; set; }
    public string? Provider { get; set; }
    public string? ReferenceId { get; set; }
}

public class PushNotificationLog : BaseEntity
{
    public Guid UserId { get; set; }
    public string? DeviceToken { get; set; }
    public string Payload { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public DateTime? SentDate { get; set; }
    public string? ErrorMessage { get; set; }

    public virtual User User { get; set; } = null!;
}
