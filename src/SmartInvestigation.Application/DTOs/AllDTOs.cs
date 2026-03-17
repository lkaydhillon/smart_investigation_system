using SmartInvestigation.Domain.Enums;

namespace SmartInvestigation.Application.DTOs;

// ── Auth DTOs ──
public record LoginRequest(string Username, string Password, string? DeviceId = null);
public record LoginResponse(string AccessToken, string RefreshToken, DateTime ExpiresAt, UserDto User);
public record RefreshTokenRequest(string AccessToken, string RefreshToken);
public record ChangePasswordRequest(string CurrentPassword, string NewPassword);

// ── User DTOs ──
public record UserDto(Guid Id, string Username, string Email, string FullName, string? BadgeNumber,
    string? Rank, string? Designation, Guid? PoliceStationId, string? StationName,
    string? ProfilePhotoUrl, List<string> Roles, List<string> Permissions);

// ── Case DTOs ──
public record CreateCaseRequest(string Title, string? Description, Guid? FIRId, Guid? CrimeTypeId,
    Guid? CrimeSubTypeId, Guid PoliceStationId, Guid DistrictId, CasePriority Priority,
    Guid? AssignedOfficerId, bool IsHighProfile, bool IsSensitive, string? Tags);

public record UpdateCaseRequest(string Title, string? Description, CaseStatus Status,
    CasePriority Priority, Guid? AssignedOfficerId, Guid? SupervisingOfficerId, string? Tags);

public record CaseDto(Guid Id, string CaseNumber, string Title, string? Description,
    CaseStatus Status, CasePriority Priority, string? CrimeTypeName, string? StationName,
    string? AssignedOfficerName, string? SupervisingOfficerName,
    DateTime DateOfRegistration, DateTime? DateOfClosure, bool IsHighProfile, bool IsSensitive,
    int EvidenceCount, int PersonCount, int PendingSteps, DateTime CreatedDate);

public record CaseDetailDto(Guid Id, string CaseNumber, string Title, string? Description,
    CaseStatus Status, CasePriority Priority, string? CrimeTypeName, string? CrimeSubTypeName,
    string? StationName, string? DistrictName, string? AssignedOfficerName,
    string? SupervisingOfficerName, DateTime DateOfRegistration, DateTime? DateOfClosure,
    List<CasePersonDto> Persons, List<EvidenceSummaryDto> Evidences,
    List<InvestigationStepDto> InvestigationSteps, List<CaseNoteDto> Notes,
    List<CaseStatusHistoryDto> StatusHistory, DateTime CreatedDate);

public record CasePersonDto(Guid Id, Guid PersonId, string FullName, PersonRole Role,
    PersonStatus Status, DateTime? ArrestDate, bool IsMainAccused);

public record CaseStatusHistoryDto(CaseStatus OldStatus, CaseStatus NewStatus,
    string? Remarks, string ChangedBy, DateTime ChangedDate);

public record CaseNoteDto(Guid Id, string Content, string OfficerName, bool IsPrivate, DateTime CreatedDate);

// ── Complaint DTOs ──
public record CreateComplaintRequest(ComplaintType Type, string Description,
    string? ComplainantName, string? ComplainantPhone, string? ComplainantAddress,
    double? GpsLatitude, double? GpsLongitude, string? LocationAddress,
    DateTime DateOfIncident, Guid PoliceStationId, bool IsUrgent,
    string? VoiceRecordingUrl, bool IsOfflineEntry);

public record ComplaintDto(Guid Id, string ComplaintNumber, ComplaintType Type,
    ComplaintStatus Status, string Description, string? ComplainantName,
    double? GpsLatitude, double? GpsLongitude, DateTime DateOfIncident,
    string? StationName, bool IsUrgent, DateTime CreatedDate);

// ── Evidence DTOs ──
public record UploadEvidenceRequest(Guid CaseId, EvidenceType Type, string Description,
    string? CollectedBy, string? CollectionLocation, double? GpsLatitude,
    double? GpsLongitude, Guid? CategoryId);

public record EvidenceSummaryDto(Guid Id, string EvidenceNumber, EvidenceType Type,
    EvidenceStatus Status, string Description, string? FileUrl, DateTime CollectionDate);

// ── Investigation DTOs ──
public record InvestigationStepDto(Guid Id, string StepTitle, string? StepDescription,
    InvestigationStepStatus Status, string? AssignedToName, DateTime? DueDate,
    DateTime? CompletedDate, int StepOrder, bool IsMandatory);

// ── SOP DTOs ──
public record SOPDto(Guid Id, string Name, string? Description, string CrimeTypeName,
    int Version, bool IsActive, List<SOPStepDto> Steps);

public record SOPStepDto(Guid Id, int StepNumber, string Title, string? Description,
    bool IsMandatory, int? DeadlineDays);

// ── Legal DTOs ──
public record LegalSectionDto(Guid Id, string Code, string Act, string Title,
    string Category, bool IsBailable, bool IsCognizable, string? MaxPenalty);

// ── Document DTOs ──
public record GenerateDocumentRequest(Guid CaseId, Guid TemplateId, Dictionary<string, string>? AdditionalData);
public record DocumentTemplateDto(Guid Id, string Name, string? Category, string? Description,
    string? Format, int Version, bool IsActive);

// ── Analytics DTOs ──
public record CrimeHotspotDto(double Latitude, double Longitude, double Intensity,
    int IncidentCount, string? CrimeTypeName);

public record DashboardStatsDto(int TotalCases, int PendingCases, int SolvedCases,
    int TotalComplaints, int PendingComplaints, int OverdueSteps,
    int UpcomingDeadlines, double ClearanceRate);

// ── Search DTOs ──
public record SearchRequest(string Query, string? EntityType, int PageNumber = 1, int PageSize = 20);
public record SearchResultDto(string EntityType, Guid EntityId, string Title,
    string? Description, double Score, Dictionary<string, object>? Highlights);

// ── Dynamic Entity DTOs ──
public record DynamicEntityDefinitionDto(Guid Id, string Name, string DisplayName,
    string? Description, bool IsActive, List<DynamicEntityFieldDto> Fields);

public record DynamicEntityFieldDto(Guid Id, string FieldName, string DisplayLabel,
    DynamicEntityFieldType FieldType, bool IsRequired, int DisplayOrder,
    string? Options, string? DefaultValue);

public record DynamicEntityRecordDto(Guid Id, Guid EntityDefinitionId,
    string? DisplayTitle, Dictionary<string, object?> FieldValues);

// ── Notification DTOs ──
public record NotificationDto(Guid Id, string Title, string Message,
    NotificationType Type, NotificationPriority Priority,
    bool IsRead, string? ActionUrl, DateTime CreatedDate);
