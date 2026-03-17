namespace SmartInvestigation.Domain.Enums;

public enum CaseStatus
{
    Registered = 0,
    UnderInvestigation = 1,
    PendingSupervision = 2,
    ChargesheetFiled = 3,
    FinalReportFiled = 4,
    CourtTrial = 5,
    Convicted = 6,
    Acquitted = 7,
    Closed = 8,
    Reopened = 9,
    Transferred = 10
}

public enum CasePriority
{
    Low = 0,
    Medium = 1,
    High = 2,
    Critical = 3,
    Urgent = 4
}

public enum FIRStatus
{
    Registered = 0,
    UnderInvestigation = 1,
    ChargesheetFiled = 2,
    FinalReportSubmitted = 3,
    Closed = 4,
    Reopened = 5
}

public enum ComplaintType
{
    Written = 0,
    Verbal = 1,
    Online = 2,
    PhoneCall = 3,
    WalkIn = 4,
    ReferredFromCourt = 5,
    ReferredFromAuthority = 6
}

public enum ComplaintStatus
{
    Received = 0,
    UnderReview = 1,
    FIRRegistered = 2,
    Rejected = 3,
    ForwardedToOtherStation = 4,
    Closed = 5
}

public enum PersonRole
{
    Accused = 0,
    Victim = 1,
    Witness = 2,
    Informer = 3,
    Complainant = 4,
    SuspectedPerson = 5,
    MissingPerson = 6,
    UnidentifiedBody = 7
}

public enum PersonStatus
{
    Active = 0,
    Arrested = 1,
    OnBail = 2,
    Absconding = 3,
    Surrendered = 4,
    Deceased = 5,
    Acquitted = 6,
    Convicted = 7
}

public enum Gender
{
    Male = 0,
    Female = 1,
    Transgender = 2,
    Other = 3,
    Unknown = 4
}

public enum EvidenceType
{
    Physical = 0,
    Digital = 1,
    Documentary = 2,
    Testimonial = 3,
    Forensic = 4,
    Photographic = 5,
    AudioVideo = 6,
    Biological = 7,
    Circumstantial = 8
}

public enum EvidenceStatus
{
    Collected = 0,
    InCustody = 1,
    SentToLab = 2,
    AnalysisComplete = 3,
    ReturnedFromLab = 4,
    ProducedInCourt = 5,
    Disposed = 6,
    Missing = 7
}

public enum ForensicRequestStatus
{
    Pending = 0,
    InTransit = 1,
    Received = 2,
    UnderAnalysis = 3,
    ReportReady = 4,
    Returned = 5
}

public enum InvestigationStepStatus
{
    Pending = 0,
    InProgress = 1,
    Completed = 2,
    Skipped = 3,
    Overdue = 4,
    Blocked = 5
}

public enum RemandType
{
    PoliceRemand = 0,
    JudicialRemand = 1,
    TransitRemand = 2
}

public enum BailStatus
{
    Applied = 0,
    Granted = 1,
    Rejected = 2,
    Cancelled = 3,
    Surrendered = 4
}

public enum ChargesheetStatus
{
    Draft = 0,
    UnderReview = 1,
    Filed = 2,
    Accepted = 3,
    Returned = 4,
    Supplementary = 5
}

public enum NotificationType
{
    Info = 0,
    Warning = 1,
    Alert = 2,
    Deadline = 3,
    Assignment = 4,
    Compliance = 5,
    System = 6
}

public enum NotificationPriority
{
    Low = 0,
    Medium = 1,
    High = 2,
    Critical = 3
}

public enum AlertSeverity
{
    Info = 0,
    Warning = 1,
    Critical = 2,
    Emergency = 3
}

public enum CustomFieldType
{
    Text = 0,
    Number = 1,
    Decimal = 2,
    Date = 3,
    DateTime = 4,
    Boolean = 5,
    Dropdown = 6,
    MultiSelect = 7,
    TextArea = 8,
    FileUpload = 9,
    GpsLocation = 10,
    PhoneNumber = 11,
    Email = 12,
    Url = 13
}

public enum BiometricType
{
    Fingerprint = 0,
    IrisScan = 1,
    FaceEncoding = 2,
    DNAProfile = 3,
    VoiceSample = 4
}

public enum SurveillanceType
{
    Physical = 0,
    Electronic = 1,
    PhoneTap = 2,
    CCTV = 3,
    Online = 4,
    Financial = 5
}

public enum DocumentStatus
{
    Draft = 0,
    Pending = 1,
    Approved = 2,
    Signed = 3,
    Filed = 4,
    Rejected = 5
}

public enum WeaponType
{
    Firearm = 0,
    BluntWeapon = 1,
    SharpWeapon = 2,
    Explosive = 3,
    Chemical = 4,
    Biological = 5,
    Vehicle = 6,
    Other = 7
}

public enum CourtOrderType
{
    Bail = 0,
    Remand = 1,
    SearchWarrant = 2,
    ArrestWarrant = 3,
    SummonsOrder = 4,
    StayOrder = 5,
    CompensationOrder = 6,
    AcquittalOrder = 7,
    ConvictionOrder = 8,
    Other = 9
}

public enum RiskLevel
{
    Low = 0,
    Medium = 1,
    High = 2,
    Critical = 3
}

public enum DynamicEntityFieldType
{
    Text = 0,
    Number = 1,
    Decimal = 2,
    Date = 3,
    DateTime = 4,
    Boolean = 5,
    Dropdown = 6,
    MultiSelect = 7,
    TextArea = 8,
    RichText = 9,
    FileUpload = 10,
    ImageUpload = 11,
    GpsLocation = 12,
    Reference = 13,
    Computed = 14,
    Json = 15
}
