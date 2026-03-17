using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartInvestigation.Domain.Entities;

namespace SmartInvestigation.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasIndex(e => e.Username).IsUnique();
        builder.HasIndex(e => e.Email).IsUnique();
        builder.HasIndex(e => e.BadgeNumber);
        builder.Property(e => e.Username).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Email).HasMaxLength(200).IsRequired();
        builder.Property(e => e.FullName).HasMaxLength(300).IsRequired();
        builder.Property(e => e.PasswordHash).HasMaxLength(512).IsRequired();
        builder.HasOne(e => e.PoliceStation).WithMany(s => s.Officers).HasForeignKey(e => e.PoliceStationId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");
        builder.HasIndex(e => e.Name).IsUnique();
        builder.Property(e => e.Name).HasMaxLength(100).IsRequired();
    }
}

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles");
        builder.HasIndex(e => new { e.UserId, e.RoleId }).IsUnique();
        builder.HasOne(e => e.User).WithMany(u => u.UserRoles).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Role).WithMany(r => r.UserRoles).HasForeignKey(e => e.RoleId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");
        builder.HasIndex(e => e.Code).IsUnique();
        builder.Property(e => e.Code).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
    }
}

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermissions");
        builder.HasIndex(e => new { e.RoleId, e.PermissionId }).IsUnique();
        builder.HasOne(e => e.Role).WithMany(r => r.RolePermissions).HasForeignKey(e => e.RoleId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Permission).WithMany(p => p.RolePermissions).HasForeignKey(e => e.PermissionId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");
        builder.HasIndex(e => e.Token).IsUnique();
        builder.HasIndex(e => e.UserId);
        builder.Property(e => e.Token).HasMaxLength(512).IsRequired();
        builder.HasOne(e => e.User).WithMany(u => u.RefreshTokens).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.Ignore(e => e.IsExpired);
        builder.Ignore(e => e.IsRevoked);
        builder.Ignore(e => e.IsActive);
    }
}

public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.ToTable("UserSessions");
        builder.HasIndex(e => e.UserId);
        builder.HasOne(e => e.User).WithMany(u => u.Sessions).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class UserDeviceConfiguration : IEntityTypeConfiguration<UserDevice>
{
    public void Configure(EntityTypeBuilder<UserDevice> builder)
    {
        builder.ToTable("UserDevices");
        builder.HasIndex(e => new { e.UserId, e.DeviceId }).IsUnique();
        builder.Property(e => e.DeviceId).HasMaxLength(200).IsRequired();
        builder.HasOne(e => e.User).WithMany(u => u.Devices).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class UserPreferenceConfiguration : IEntityTypeConfiguration<UserPreference>
{
    public void Configure(EntityTypeBuilder<UserPreference> builder)
    {
        builder.ToTable("UserPreferences");
        builder.HasIndex(e => e.UserId).IsUnique();
        builder.HasOne(e => e.User).WithOne(u => u.Preferences).HasForeignKey<UserPreference>(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

// ── Document & Notification Configurations ──

public class DocumentTemplateConfiguration : IEntityTypeConfiguration<DocumentTemplate>
{
    public void Configure(EntityTypeBuilder<DocumentTemplate> builder)
    {
        builder.ToTable("DocumentTemplates");
        builder.Property(e => e.Name).HasMaxLength(300).IsRequired();
        builder.HasOne(e => e.Category).WithMany(c => c.Templates).HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class GeneratedDocumentConfiguration : IEntityTypeConfiguration<GeneratedDocument>
{
    public void Configure(EntityTypeBuilder<GeneratedDocument> builder)
    {
        builder.ToTable("GeneratedDocuments");
        builder.HasIndex(e => e.CaseId);
        builder.HasOne(e => e.Case).WithMany(c => c.Documents).HasForeignKey(e => e.CaseId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(e => e.Template).WithMany(t => t.GeneratedDocuments).HasForeignKey(e => e.TemplateId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.GeneratedByUser).WithMany().HasForeignKey(e => e.GeneratedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");
        builder.HasIndex(e => new { e.UserId, e.IsRead });
        builder.HasIndex(e => e.CreatedDate);
        builder.HasOne(e => e.User).WithMany(u => u.Notifications).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");
        builder.HasIndex(e => e.EntityType);
        builder.HasIndex(e => e.EntityId);
        builder.HasIndex(e => e.CreatedDate);
        builder.HasIndex(e => e.UserId);
        builder.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.SetNull);
    }
}

// ── Config & Dynamic Entity Configurations ──

public class CrimeTypeConfiguration : IEntityTypeConfiguration<CrimeType>
{
    public void Configure(EntityTypeBuilder<CrimeType> builder)
    {
        builder.ToTable("CrimeTypes");
        builder.HasIndex(e => e.Code).IsUnique();
        builder.Property(e => e.Name).HasMaxLength(300).IsRequired();
        builder.Property(e => e.Code).HasMaxLength(50).IsRequired();
        builder.HasOne(e => e.Parent).WithMany(c => c.Children).HasForeignKey(e => e.ParentId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class CustomFieldDefinitionConfiguration : IEntityTypeConfiguration<CustomFieldDefinition>
{
    public void Configure(EntityTypeBuilder<CustomFieldDefinition> builder)
    {
        builder.ToTable("CustomFieldDefinitions");
        builder.HasIndex(e => new { e.EntityType, e.FieldName }).IsUnique();
    }
}

public class CustomFieldValueConfiguration : IEntityTypeConfiguration<CustomFieldValue>
{
    public void Configure(EntityTypeBuilder<CustomFieldValue> builder)
    {
        builder.ToTable("CustomFieldValues");
        builder.HasIndex(e => new { e.DefinitionId, e.EntityId }).IsUnique();
        builder.HasOne(e => e.Definition).WithMany(d => d.Values).HasForeignKey(e => e.DefinitionId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class SystemConfigurationConfig : IEntityTypeConfiguration<SystemConfiguration>
{
    public void Configure(EntityTypeBuilder<SystemConfiguration> builder)
    {
        builder.ToTable("SystemConfigurations");
        builder.HasIndex(e => e.Key).IsUnique();
        builder.Property(e => e.Key).HasMaxLength(200).IsRequired();
    }
}

public class LookupValueConfiguration : IEntityTypeConfiguration<LookupValue>
{
    public void Configure(EntityTypeBuilder<LookupValue> builder)
    {
        builder.ToTable("LookupValues");
        builder.HasIndex(e => new { e.Category, e.Code }).IsUnique();
    }
}

public class DynamicEntityDefinitionConfiguration : IEntityTypeConfiguration<DynamicEntityDefinition>
{
    public void Configure(EntityTypeBuilder<DynamicEntityDefinition> builder)
    {
        builder.ToTable("DynamicEntityDefinitions");
        builder.HasIndex(e => e.Name).IsUnique();
        builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
        builder.Property(e => e.DisplayName).HasMaxLength(300).IsRequired();
    }
}

public class DynamicEntityFieldConfiguration : IEntityTypeConfiguration<DynamicEntityField>
{
    public void Configure(EntityTypeBuilder<DynamicEntityField> builder)
    {
        builder.ToTable("DynamicEntityFields");
        builder.HasIndex(e => new { e.EntityDefinitionId, e.FieldName }).IsUnique();
        builder.HasOne(e => e.EntityDefinition).WithMany(d => d.Fields).HasForeignKey(e => e.EntityDefinitionId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class DynamicEntityRecordConfiguration : IEntityTypeConfiguration<DynamicEntityRecord>
{
    public void Configure(EntityTypeBuilder<DynamicEntityRecord> builder)
    {
        builder.ToTable("DynamicEntityRecords");
        builder.HasIndex(e => e.EntityDefinitionId);
        builder.HasOne(e => e.EntityDefinition).WithMany(d => d.Records).HasForeignKey(e => e.EntityDefinitionId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class DynamicEntityRecordValueConfiguration : IEntityTypeConfiguration<DynamicEntityRecordValue>
{
    public void Configure(EntityTypeBuilder<DynamicEntityRecordValue> builder)
    {
        builder.ToTable("DynamicEntityRecordValues");
        builder.HasIndex(e => new { e.RecordId, e.FieldId }).IsUnique();
        builder.HasOne(e => e.Record).WithMany(r => r.Values).HasForeignKey(e => e.RecordId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Field).WithMany().HasForeignKey(e => e.FieldId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class DynamicRelationshipConfiguration : IEntityTypeConfiguration<DynamicRelationship>
{
    public void Configure(EntityTypeBuilder<DynamicRelationship> builder)
    {
        builder.ToTable("DynamicRelationships");
        builder.HasOne(e => e.SourceEntity).WithMany(d => d.SourceRelationships).HasForeignKey(e => e.SourceEntityId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.TargetEntity).WithMany(d => d.TargetRelationships).HasForeignKey(e => e.TargetEntityId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class DynamicEntityViewConfiguration : IEntityTypeConfiguration<DynamicEntityView>
{
    public void Configure(EntityTypeBuilder<DynamicEntityView> builder)
    {
        builder.ToTable("DynamicEntityViews");
        builder.HasOne(e => e.EntityDefinition).WithMany(d => d.Views).HasForeignKey(e => e.EntityDefinitionId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.OwnerUser).WithMany().HasForeignKey(e => e.OwnerUserId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class FormLayoutConfiguration : IEntityTypeConfiguration<FormLayout>
{
    public void Configure(EntityTypeBuilder<FormLayout> builder)
    {
        builder.ToTable("FormLayouts");
        builder.HasOne(e => e.EntityDefinition).WithMany(d => d.FormLayouts).HasForeignKey(e => e.EntityDefinitionId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class DashboardWidgetConfiguration : IEntityTypeConfiguration<DashboardWidget>
{
    public void Configure(EntityTypeBuilder<DashboardWidget> builder)
    {
        builder.ToTable("DashboardWidgets");
    }
}
