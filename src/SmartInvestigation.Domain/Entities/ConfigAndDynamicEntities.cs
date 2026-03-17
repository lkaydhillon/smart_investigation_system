using SmartInvestigation.Domain.Common;
using SmartInvestigation.Domain.Enums;

namespace SmartInvestigation.Domain.Entities;

// ── Configuration & Customization ──────────────────

public class CrimeType : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public string? Severity { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Description { get; set; }
    public string? IconName { get; set; }
    public int SortOrder { get; set; }

    public virtual CrimeType? Parent { get; set; }
    public virtual ICollection<CrimeType> Children { get; set; } = new List<CrimeType>();
    public virtual ICollection<CrimeSubType> SubTypes { get; set; } = new List<CrimeSubType>();
    public virtual ICollection<InvestigationSOP> SOPs { get; set; } = new List<InvestigationSOP>();
}

public class CrimeSubType : BaseEntity
{
    public Guid CrimeTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual CrimeType CrimeType { get; set; } = null!;
}

/// <summary>
/// Dynamic custom field definitions — admin creates fields without code changes
/// </summary>
public class CustomFieldDefinition : BaseEntity
{
    public string EntityType { get; set; } = string.Empty; // Case, Complaint, Person, Evidence
    public string FieldName { get; set; } = string.Empty;
    public string DisplayLabel { get; set; } = string.Empty;
    public CustomFieldType FieldType { get; set; }
    public string? Options { get; set; } // JSON for dropdowns/multi-select
    public bool IsRequired { get; set; }
    public int DisplayOrder { get; set; }
    public string? Placeholder { get; set; }
    public string? HelpText { get; set; }
    public string? DefaultValue { get; set; }
    public string? ValidationRegex { get; set; }
    public string? GroupName { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsSearchable { get; set; }
    public string? VisibilityCondition { get; set; } // JSON rule for conditional display

    public virtual ICollection<CustomFieldValue> Values { get; set; } = new List<CustomFieldValue>();
}

public class CustomFieldValue : BaseEntity
{
    public Guid DefinitionId { get; set; }
    public Guid EntityId { get; set; }
    public string? Value { get; set; }
    public string? EntityType { get; set; }

    public virtual CustomFieldDefinition Definition { get; set; } = null!;
}

/// <summary>
/// System-wide configuration settings
/// </summary>
public class SystemConfiguration : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string? Value { get; set; }
    public string Category { get; set; } = string.Empty;
    public string DataType { get; set; } = "string";
    public string? Description { get; set; }
    public bool IsEditable { get; set; } = true;
    public string? ValidationRule { get; set; }
}

/// <summary>
/// Lookup values for dropdowns: Nationality, Religion, Caste, etc.
/// </summary>
public class LookupValue : BaseEntity
{
    public string Category { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? ParentCode { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Metadata { get; set; } // JSON for extra config
}

// ── Dynamic Entity Engine — admin-created tables ──────────

/// <summary>
/// Admin-defined custom entity type. Allows creating entirely new "tables"
/// from the admin panel without any code changes.
/// </summary>
public class DynamicEntityDefinition : BaseEntity
{
    public string Name { get; set; } = string.Empty; // e.g., "InformantReport"
    public string DisplayName { get; set; } = string.Empty;
    public string? PluralDisplayName { get; set; }
    public string? Description { get; set; }
    public string? IconName { get; set; }
    public string? Category { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsSearchable { get; set; } = true;
    public bool EnableAuditLog { get; set; } = true;
    public bool EnableAttachments { get; set; }
    public bool EnableComments { get; set; }
    public bool EnableWorkflow { get; set; }
    public string? DefaultSortField { get; set; }
    public string? DefaultSortDirection { get; set; } = "DESC";
    public int SortOrder { get; set; }

    public virtual ICollection<DynamicEntityField> Fields { get; set; } = new List<DynamicEntityField>();
    public virtual ICollection<DynamicEntityRecord> Records { get; set; } = new List<DynamicEntityRecord>();
    public virtual ICollection<DynamicRelationship> SourceRelationships { get; set; } = new List<DynamicRelationship>();
    public virtual ICollection<DynamicRelationship> TargetRelationships { get; set; } = new List<DynamicRelationship>();
    public virtual ICollection<DynamicEntityView> Views { get; set; } = new List<DynamicEntityView>();
    public virtual ICollection<FormLayout> FormLayouts { get; set; } = new List<FormLayout>();
}

/// <summary>
/// Field definition for a dynamic entity — each field is like a column in a table
/// </summary>
public class DynamicEntityField : BaseEntity
{
    public Guid EntityDefinitionId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string DisplayLabel { get; set; } = string.Empty;
    public DynamicEntityFieldType FieldType { get; set; }
    public bool IsRequired { get; set; }
    public bool IsUnique { get; set; }
    public bool IsSearchable { get; set; }
    public bool IsFilterable { get; set; }
    public bool IsSortable { get; set; }
    public bool ShowInList { get; set; } = true;
    public int DisplayOrder { get; set; }
    public string? DefaultValue { get; set; }
    public string? Options { get; set; } // JSON for dropdown/multiselect
    public string? ValidationRegex { get; set; }
    public string? ValidationMessage { get; set; }
    public int? MinLength { get; set; }
    public int? MaxLength { get; set; }
    public double? MinValue { get; set; }
    public double? MaxValue { get; set; }
    public string? Placeholder { get; set; }
    public string? HelpText { get; set; }
    public string? VisibilityCondition { get; set; } // JSON
    public string? ComputedExpression { get; set; } // For computed fields
    public Guid? ReferencedEntityId { get; set; } // For Reference type fields
    public string? ReferencedDisplayField { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual DynamicEntityDefinition EntityDefinition { get; set; } = null!;
    public virtual DynamicEntityDefinition? ReferencedEntity { get; set; }
}

/// <summary>
/// Actual data record for a dynamic entity
/// </summary>
public class DynamicEntityRecord : BaseEntity
{
    public Guid EntityDefinitionId { get; set; }
    public string? DisplayTitle { get; set; }
    public string? Status { get; set; }

    public virtual DynamicEntityDefinition EntityDefinition { get; set; } = null!;
    public virtual ICollection<DynamicEntityRecordValue> Values { get; set; } = new List<DynamicEntityRecordValue>();
}

/// <summary>
/// Field values for a dynamic entity record (EAV pattern)
/// </summary>
public class DynamicEntityRecordValue : BaseEntity
{
    public Guid RecordId { get; set; }
    public Guid FieldId { get; set; }
    public string? TextValue { get; set; }
    public double? NumberValue { get; set; }
    public DateTime? DateValue { get; set; }
    public bool? BooleanValue { get; set; }
    public string? JsonValue { get; set; }

    public virtual DynamicEntityRecord Record { get; set; } = null!;
    public virtual DynamicEntityField Field { get; set; } = null!;
}

/// <summary>
/// Relationships between dynamic entities or linking to core entities
/// </summary>
public class DynamicRelationship : BaseEntity
{
    public Guid SourceEntityId { get; set; }
    public Guid TargetEntityId { get; set; }
    public string RelationshipType { get; set; } = "OneToMany"; // OneToOne, OneToMany, ManyToMany
    public string? DisplayName { get; set; }
    public string? InverseDisplayName { get; set; }
    public bool IsCascadeDelete { get; set; }
    public bool IsRequired { get; set; }

    public virtual DynamicEntityDefinition SourceEntity { get; set; } = null!;
    public virtual DynamicEntityDefinition TargetEntity { get; set; } = null!;
}

/// <summary>
/// Saved views/filters for dynamic entities
/// </summary>
public class DynamicEntityView : BaseEntity
{
    public Guid EntityDefinitionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? FilterExpression { get; set; } // JSON filter rules
    public string? ColumnConfiguration { get; set; } // JSON: which fields, widths, order
    public string? SortConfiguration { get; set; }
    public bool IsDefault { get; set; }
    public bool IsPublic { get; set; }
    public Guid? OwnerUserId { get; set; }

    public virtual DynamicEntityDefinition EntityDefinition { get; set; } = null!;
    public virtual User? OwnerUser { get; set; }
}

/// <summary>
/// Configurable form layouts per entity
/// </summary>
public class FormLayout : BaseEntity
{
    public Guid EntityDefinitionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LayoutDefinition { get; set; } = string.Empty; // JSON: sections, tabs, field grouping
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;
    public string? ApplicableRoles { get; set; } // JSON: show different forms per role

    public virtual DynamicEntityDefinition EntityDefinition { get; set; } = null!;
}

/// <summary>
/// Dashboard widget configuration
/// </summary>
public class DashboardWidget : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string WidgetType { get; set; } = string.Empty; // Chart, Counter, Table, Map
    public string? DataSource { get; set; } // Entity or API endpoint
    public string? Configuration { get; set; } // JSON: query, display options
    public string? ApplicableRoles { get; set; }
    public int DefaultWidth { get; set; } = 4; // Grid columns (1-12)
    public int DefaultHeight { get; set; } = 2; // Grid rows
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
}

// ── Audit & Compliance ──────────────────────────────

public class AuditLog : BaseEntity
{
    public Guid? UserId { get; set; }
    public string Action { get; set; } = string.Empty; // Create, Update, Delete, Access
    public string EntityType { get; set; } = string.Empty;
    public Guid? EntityId { get; set; }
    public string? OldValues { get; set; } // JSON
    public string? NewValues { get; set; } // JSON
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? CorrelationId { get; set; }
    public string? AdditionalInfo { get; set; }

    public virtual User? User { get; set; }
}

public class ComplianceCheck : BaseEntity
{
    public Guid CaseId { get; set; }
    public string CheckType { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public DateTime DueDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? Description { get; set; }
    public string? AutomatedCheckResult { get; set; }
    public Guid? NotifiedUserId { get; set; }

    public virtual Case Case { get; set; } = null!;
    public virtual User? NotifiedUser { get; set; }
    public virtual ICollection<ComplianceViolation> Violations { get; set; } = new List<ComplianceViolation>();
}

public class ComplianceViolation : BaseEntity
{
    public Guid CheckId { get; set; }
    public string ViolationType { get; set; } = string.Empty;
    public AlertSeverity Severity { get; set; }
    public string? Description { get; set; }
    public Guid? NotifiedToUserId { get; set; }
    public DateTime? ResolvedDate { get; set; }
    public string? Resolution { get; set; }
    public Guid? ResolvedByUserId { get; set; }

    public virtual ComplianceCheck Check { get; set; } = null!;
    public virtual User? NotifiedToUser { get; set; }
    public virtual User? ResolvedByUser { get; set; }
}

public class DataAccessLog : BaseEntity
{
    public Guid UserId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string AccessType { get; set; } = string.Empty; // Read, Export, Print
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? IpAddress { get; set; }
    public string? Purpose { get; set; }

    public virtual User User { get; set; } = null!;
}

// ── Analytics ───────────────────────────────────────

public class CrimeStatistic : BaseEntity
{
    public Guid? StationId { get; set; }
    public Guid? DistrictId { get; set; }
    public Guid? CrimeTypeId { get; set; }
    public string Period { get; set; } = string.Empty; // "2024-Q1", "2024-03"
    public string PeriodType { get; set; } = "Monthly"; // Daily, Weekly, Monthly, Quarterly, Yearly
    public int TotalCases { get; set; }
    public int SolvedCases { get; set; }
    public int PendingCases { get; set; }
    public double? ClearanceRate { get; set; }
    public double? AverageResolutionDays { get; set; }

    public virtual PoliceStation? Station { get; set; }
    public virtual District? District { get; set; }
    public virtual CrimeType? CrimeType { get; set; }
}

public class HotspotData : BaseEntity
{
    public double GpsLatitude { get; set; }
    public double GpsLongitude { get; set; }
    public Guid? CrimeTypeId { get; set; }
    public double Intensity { get; set; }
    public string Period { get; set; } = string.Empty;
    public int IncidentCount { get; set; }
    public string? ClusterInfo { get; set; } // JSON

    public virtual CrimeType? CrimeType { get; set; }
}

public class PatrolRecommendation : BaseEntity
{
    public Guid? ZoneId { get; set; }
    public DateTime Date { get; set; }
    public string TimeSlot { get; set; } = string.Empty;
    public double RiskScore { get; set; }
    public string Recommendation { get; set; } = string.Empty;
    public string? BasedOnFactors { get; set; } // JSON
    public bool IsAcknowledged { get; set; }
    public Guid? AcknowledgedByUserId { get; set; }

    public virtual PatrolZone? Zone { get; set; }
    public virtual User? AcknowledgedByUser { get; set; }
}
