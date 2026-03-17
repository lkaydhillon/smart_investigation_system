using Microsoft.EntityFrameworkCore;
using SmartInvestigation.Domain.Common;
using SmartInvestigation.Domain.Entities;
using SmartInvestigation.Infrastructure.Persistence.Interceptors;

namespace SmartInvestigation.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    private readonly AuditInterceptor _auditInterceptor;

    public AppDbContext(DbContextOptions<AppDbContext> options, AuditInterceptor auditInterceptor)
        : base(options)
    {
        _auditInterceptor = auditInterceptor;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditInterceptor);
        base.OnConfiguring(optionsBuilder);
    }

    // ── CASE MANAGEMENT ──
    public DbSet<FIR> FIRs => Set<FIR>();
    public DbSet<Case> Cases => Set<Case>();
    public DbSet<Complaint> Complaints => Set<Complaint>();
    public DbSet<CaseComplaint> CaseComplaints => Set<CaseComplaint>();
    public DbSet<CaseStatusHistory> CaseStatusHistories => Set<CaseStatusHistory>();
    public DbSet<CaseTransfer> CaseTransfers => Set<CaseTransfer>();
    public DbSet<CaseMerge> CaseMerges => Set<CaseMerge>();
    public DbSet<CaseNote> CaseNotes => Set<CaseNote>();

    // ── People & Organizations ──
    public DbSet<Person> Persons => Set<Person>();
    public DbSet<PersonAlias> PersonAliases => Set<PersonAlias>();
    public DbSet<PersonAddress> PersonAddresses => Set<PersonAddress>();
    public DbSet<PersonContact> PersonContacts => Set<PersonContact>();
    public DbSet<PersonBiometric> PersonBiometrics => Set<PersonBiometric>();
    public DbSet<PersonPhoto> PersonPhotos => Set<PersonPhoto>();
    public DbSet<CasePerson> CasePersons => Set<CasePerson>();
    public DbSet<PersonCriminalHistory> PersonCriminalHistories => Set<PersonCriminalHistory>();
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<OrganizationMember> OrganizationMembers => Set<OrganizationMember>();

    // ── Evidence & Forensics ──
    public DbSet<Evidence> Evidences => Set<Evidence>();
    public DbSet<EvidenceChainOfCustody> EvidenceChainOfCustodies => Set<EvidenceChainOfCustody>();
    public DbSet<EvidenceCategory> EvidenceCategories => Set<EvidenceCategory>();
    public DbSet<ForensicLabRequest> ForensicLabRequests => Set<ForensicLabRequest>();
    public DbSet<ForensicReport> ForensicReports => Set<ForensicReport>();
    public DbSet<DigitalEvidence> DigitalEvidences => Set<DigitalEvidence>();
    public DbSet<SeizedProperty> SeizedProperties => Set<SeizedProperty>();
    public DbSet<PropertyCategory> PropertyCategories => Set<PropertyCategory>();

    // ── Investigation ──
    public DbSet<InvestigationStep> InvestigationSteps => Set<InvestigationStep>();
    public DbSet<InvestigationSOP> InvestigationSOPs => Set<InvestigationSOP>();
    public DbSet<SOPStep> SOPSteps => Set<SOPStep>();
    public DbSet<InvestigationTeam> InvestigationTeams => Set<InvestigationTeam>();
    public DbSet<InvestigationTeamMember> InvestigationTeamMembers => Set<InvestigationTeamMember>();
    public DbSet<Interrogation> Interrogations => Set<Interrogation>();
    public DbSet<WitnessStatement> WitnessStatements => Set<WitnessStatement>();
    public DbSet<SurveillanceRecord> SurveillanceRecords => Set<SurveillanceRecord>();
    public DbSet<CaseDiary> CaseDiaries => Set<CaseDiary>();
    public DbSet<SceneSurvey> SceneSurveys => Set<SceneSurvey>();

    // ── Legal & Court ──
    public DbSet<LegalSection> LegalSections => Set<LegalSection>();
    public DbSet<CaseLegalSection> CaseLegalSections => Set<CaseLegalSection>();
    public DbSet<LegalDeadline> LegalDeadlines => Set<LegalDeadline>();
    public DbSet<CourtHearing> CourtHearings => Set<CourtHearing>();
    public DbSet<CourtOrder> CourtOrders => Set<CourtOrder>();
    public DbSet<Court> Courts => Set<Court>();
    public DbSet<Chargesheet> Chargesheets => Set<Chargesheet>();
    public DbSet<ChargesheetPerson> ChargesheetPersons => Set<ChargesheetPerson>();
    public DbSet<BailApplication> BailApplications => Set<BailApplication>();
    public DbSet<Remand> Remands => Set<Remand>();

    // ── Locations & Geography ──
    public DbSet<PoliceStation> PoliceStations => Set<PoliceStation>();
    public DbSet<District> Districts => Set<District>();
    public DbSet<State> States => Set<State>();
    public DbSet<CrimeScene> CrimeScenes => Set<CrimeScene>();
    public DbSet<PatrolZone> PatrolZones => Set<PatrolZone>();
    public DbSet<GeoFence> GeoFences => Set<GeoFence>();

    // ── Vehicles & Weapons ──
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<VehicleInterception> VehicleInterceptions => Set<VehicleInterception>();
    public DbSet<Weapon> Weapons => Set<Weapon>();
    public DbSet<WeaponBallistic> WeaponBallistics => Set<WeaponBallistic>();

    // ── Documents & Templates ──
    public DbSet<DocumentTemplate> DocumentTemplates => Set<DocumentTemplate>();
    public DbSet<GeneratedDocument> GeneratedDocuments => Set<GeneratedDocument>();
    public DbSet<DocumentAttachment> DocumentAttachments => Set<DocumentAttachment>();
    public DbSet<DocumentCategory> DocumentCategories => Set<DocumentCategory>();

    // ── Notifications ──
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationTemplate> NotificationTemplates => Set<NotificationTemplate>();
    public DbSet<AlertRule> AlertRules => Set<AlertRule>();
    public DbSet<SMSLog> SMSLogs => Set<SMSLog>();
    public DbSet<PushNotificationLog> PushNotificationLogs => Set<PushNotificationLog>();

    // ── Users & Access Control ──
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();
    public DbSet<UserDevice> UserDevices => Set<UserDevice>();
    public DbSet<UserPreference> UserPreferences => Set<UserPreference>();

    // ── Configuration ──
    public DbSet<CrimeType> CrimeTypes => Set<CrimeType>();
    public DbSet<CrimeSubType> CrimeSubTypes => Set<CrimeSubType>();
    public DbSet<CustomFieldDefinition> CustomFieldDefinitions => Set<CustomFieldDefinition>();
    public DbSet<CustomFieldValue> CustomFieldValues => Set<CustomFieldValue>();
    public DbSet<SystemConfiguration> SystemConfigurations => Set<SystemConfiguration>();
    public DbSet<LookupValue> LookupValues => Set<LookupValue>();

    // ── Dynamic Entity Engine ──
    public DbSet<DynamicEntityDefinition> DynamicEntityDefinitions => Set<DynamicEntityDefinition>();
    public DbSet<DynamicEntityField> DynamicEntityFields => Set<DynamicEntityField>();
    public DbSet<DynamicEntityRecord> DynamicEntityRecords => Set<DynamicEntityRecord>();
    public DbSet<DynamicEntityRecordValue> DynamicEntityRecordValues => Set<DynamicEntityRecordValue>();
    public DbSet<DynamicRelationship> DynamicRelationships => Set<DynamicRelationship>();
    public DbSet<DynamicEntityView> DynamicEntityViews => Set<DynamicEntityView>();
    public DbSet<FormLayout> FormLayouts => Set<FormLayout>();
    public DbSet<DashboardWidget> DashboardWidgets => Set<DashboardWidget>();

    // ── Audit & Compliance ──
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<ComplianceCheck> ComplianceChecks => Set<ComplianceCheck>();
    public DbSet<ComplianceViolation> ComplianceViolations => Set<ComplianceViolation>();
    public DbSet<DataAccessLog> DataAccessLogs => Set<DataAccessLog>();

    // ── Analytics ──
    public DbSet<CrimeStatistic> CrimeStatistics => Set<CrimeStatistic>();
    public DbSet<HotspotData> HotspotData => Set<HotspotData>();
    public DbSet<PatrolRecommendation> PatrolRecommendations => Set<PatrolRecommendation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Global query filter for soft delete
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(CreateSoftDeleteFilter(entityType.ClrType));
            }
        }
    }

    private static System.Linq.Expressions.LambdaExpression CreateSoftDeleteFilter(Type entityType)
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(entityType, "e");
        var property = System.Linq.Expressions.Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
        var condition = System.Linq.Expressions.Expression.Equal(property, System.Linq.Expressions.Expression.Constant(false));
        return System.Linq.Expressions.Expression.Lambda(condition, parameter);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Global UTC normalizer: PostgreSQL requires 'timestamp with time zone' = UTC DateTimes.
        // This fixes any DateTime with Kind=Unspecified or Local before it hits Npgsql.
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State is EntityState.Added or EntityState.Modified)
            {
                foreach (var prop in entry.Properties)
                {
                    if (prop.CurrentValue is DateTime dt && dt.Kind != DateTimeKind.Utc)
                    {
                        prop.CurrentValue = dt.Kind == DateTimeKind.Unspecified
                            ? DateTime.SpecifyKind(dt, DateTimeKind.Utc)
                            : dt.ToUniversalTime();
                    }
                    else if (prop.CurrentValue is DateTime?)
                    {
                        var nullable = (DateTime?)prop.CurrentValue;
                        if (nullable.HasValue && nullable.Value.Kind != DateTimeKind.Utc)
                        {
                            var val = nullable.Value;
                            prop.CurrentValue = (DateTime?)(val.Kind == DateTimeKind.Unspecified
                                ? DateTime.SpecifyKind(val, DateTimeKind.Utc)
                                : val.ToUniversalTime());
                        }
                    }
                }
            }
        }

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedDate = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.ModifiedDate = DateTime.UtcNow;
                    break;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
