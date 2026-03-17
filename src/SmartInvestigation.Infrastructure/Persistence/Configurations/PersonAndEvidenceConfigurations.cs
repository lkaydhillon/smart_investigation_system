using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartInvestigation.Domain.Entities;

namespace SmartInvestigation.Infrastructure.Persistence.Configurations;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("Persons");
        builder.HasIndex(e => e.FullName);
        builder.HasIndex(e => e.AadhaarHash);
        builder.HasIndex(e => e.IsRepeatOffender);
        builder.Property(e => e.FullName).HasMaxLength(300).IsRequired();
        builder.Property(e => e.FatherName).HasMaxLength(300);
        builder.Property(e => e.AadhaarHash).HasMaxLength(128);
        builder.Property(e => e.IdentificationMarks).HasMaxLength(1000);
    }
}

public class PersonAliasConfiguration : IEntityTypeConfiguration<PersonAlias>
{
    public void Configure(EntityTypeBuilder<PersonAlias> builder)
    {
        builder.ToTable("PersonAliases");
        builder.HasIndex(e => e.AliasName);
        builder.Property(e => e.AliasName).HasMaxLength(300).IsRequired();
        builder.HasOne(e => e.Person).WithMany(p => p.Aliases).HasForeignKey(e => e.PersonId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class PersonAddressConfiguration : IEntityTypeConfiguration<PersonAddress>
{
    public void Configure(EntityTypeBuilder<PersonAddress> builder)
    {
        builder.ToTable("PersonAddresses");
        builder.HasIndex(e => e.PersonId);
        builder.HasOne(e => e.Person).WithMany(p => p.Addresses).HasForeignKey(e => e.PersonId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class PersonContactConfiguration : IEntityTypeConfiguration<PersonContact>
{
    public void Configure(EntityTypeBuilder<PersonContact> builder)
    {
        builder.ToTable("PersonContacts");
        builder.HasIndex(e => new { e.ContactType, e.Value });
        builder.HasIndex(e => e.PersonId);
        builder.Property(e => e.Value).HasMaxLength(200).IsRequired();
        builder.HasOne(e => e.Person).WithMany(p => p.Contacts).HasForeignKey(e => e.PersonId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class PersonBiometricConfiguration : IEntityTypeConfiguration<PersonBiometric>
{
    public void Configure(EntityTypeBuilder<PersonBiometric> builder)
    {
        builder.ToTable("PersonBiometrics");
        builder.HasIndex(e => e.PersonId);
        builder.Property(e => e.DataHash).HasMaxLength(512).IsRequired();
        builder.HasOne(e => e.Person).WithMany(p => p.Biometrics).HasForeignKey(e => e.PersonId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class PersonPhotoConfiguration : IEntityTypeConfiguration<PersonPhoto>
{
    public void Configure(EntityTypeBuilder<PersonPhoto> builder)
    {
        builder.ToTable("PersonPhotos");
        builder.HasIndex(e => e.PersonId);
        builder.Property(e => e.PhotoUrl).HasMaxLength(1000).IsRequired();
        builder.HasOne(e => e.Person).WithMany(p => p.Photos).HasForeignKey(e => e.PersonId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class CasePersonConfiguration : IEntityTypeConfiguration<CasePerson>
{
    public void Configure(EntityTypeBuilder<CasePerson> builder)
    {
        builder.ToTable("CasePersons");
        builder.HasIndex(e => new { e.CaseId, e.PersonId, e.Role }).IsUnique();
        builder.HasIndex(e => e.PersonId);
        builder.HasIndex(e => e.Status);
        builder.HasOne(e => e.Case).WithMany(c => c.CasePersons).HasForeignKey(e => e.CaseId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Person).WithMany(p => p.CasePersons).HasForeignKey(e => e.PersonId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class PersonCriminalHistoryConfiguration : IEntityTypeConfiguration<PersonCriminalHistory>
{
    public void Configure(EntityTypeBuilder<PersonCriminalHistory> builder)
    {
        builder.ToTable("PersonCriminalHistories");
        builder.HasIndex(e => e.PersonId);
        builder.HasOne(e => e.Person).WithMany(p => p.CriminalHistory).HasForeignKey(e => e.PersonId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("Organizations");
        builder.HasIndex(e => e.Name);
        builder.Property(e => e.Name).HasMaxLength(300).IsRequired();
    }
}

public class OrganizationMemberConfiguration : IEntityTypeConfiguration<OrganizationMember>
{
    public void Configure(EntityTypeBuilder<OrganizationMember> builder)
    {
        builder.ToTable("OrganizationMembers");
        builder.HasIndex(e => new { e.OrganizationId, e.PersonId }).IsUnique();
        builder.HasOne(e => e.Organization).WithMany(o => o.Members).HasForeignKey(e => e.OrganizationId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Person).WithMany(p => p.OrganizationMemberships).HasForeignKey(e => e.PersonId).OnDelete(DeleteBehavior.Restrict);
    }
}

// ── Evidence Configurations ──

public class EvidenceConfiguration : IEntityTypeConfiguration<Evidence>
{
    public void Configure(EntityTypeBuilder<Evidence> builder)
    {
        builder.ToTable("Evidences");
        builder.HasIndex(e => e.EvidenceNumber).IsUnique();
        builder.HasIndex(e => e.CaseId);
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.SHA256Hash);
        builder.Property(e => e.EvidenceNumber).HasMaxLength(50).IsRequired();
        builder.Property(e => e.SHA256Hash).HasMaxLength(128);
        builder.HasOne(e => e.Case).WithMany(c => c.Evidences).HasForeignKey(e => e.CaseId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Category).WithMany(c => c.Evidences).HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class EvidenceChainOfCustodyConfiguration : IEntityTypeConfiguration<EvidenceChainOfCustody>
{
    public void Configure(EntityTypeBuilder<EvidenceChainOfCustody> builder)
    {
        builder.ToTable("EvidenceChainOfCustodies");
        builder.HasIndex(e => e.EvidenceId);
        builder.HasIndex(e => e.Timestamp);
        builder.HasOne(e => e.Evidence).WithMany(e => e.ChainOfCustody).HasForeignKey(e => e.EvidenceId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class EvidenceCategoryConfiguration : IEntityTypeConfiguration<EvidenceCategory>
{
    public void Configure(EntityTypeBuilder<EvidenceCategory> builder)
    {
        builder.ToTable("EvidenceCategories");
        builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
        builder.HasOne(e => e.ParentCategory).WithMany(e => e.SubCategories).HasForeignKey(e => e.ParentCategoryId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class ForensicLabRequestConfiguration : IEntityTypeConfiguration<ForensicLabRequest>
{
    public void Configure(EntityTypeBuilder<ForensicLabRequest> builder)
    {
        builder.ToTable("ForensicLabRequests");
        builder.HasIndex(e => e.CaseId);
        builder.HasIndex(e => e.Status);
        builder.HasOne(e => e.Case).WithMany().HasForeignKey(e => e.CaseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Evidence).WithMany(e => e.ForensicRequests).HasForeignKey(e => e.EvidenceId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class ForensicReportConfiguration : IEntityTypeConfiguration<ForensicReport>
{
    public void Configure(EntityTypeBuilder<ForensicReport> builder)
    {
        builder.ToTable("ForensicReports");
        builder.HasIndex(e => e.ReportNumber).IsUnique();
        builder.Property(e => e.ReportNumber).HasMaxLength(100).IsRequired();
        builder.HasOne(e => e.LabRequest).WithOne(r => r.Report).HasForeignKey<ForensicReport>(e => e.LabRequestId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class DigitalEvidenceConfiguration : IEntityTypeConfiguration<DigitalEvidence>
{
    public void Configure(EntityTypeBuilder<DigitalEvidence> builder)
    {
        builder.ToTable("DigitalEvidences");
        builder.HasIndex(e => e.IMEI);
        builder.HasOne(e => e.Evidence).WithOne(e => e.DigitalEvidence).HasForeignKey<DigitalEvidence>(e => e.EvidenceId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class SeizedPropertyConfiguration : IEntityTypeConfiguration<SeizedProperty>
{
    public void Configure(EntityTypeBuilder<SeizedProperty> builder)
    {
        builder.ToTable("SeizedProperties");
        builder.HasIndex(e => e.CaseId);
        builder.Property(e => e.EstimatedValue).HasPrecision(18, 2);
        builder.HasOne(e => e.Case).WithMany().HasForeignKey(e => e.CaseId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Category).WithMany(c => c.Properties).HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class PropertyCategoryConfiguration : IEntityTypeConfiguration<PropertyCategory>
{
    public void Configure(EntityTypeBuilder<PropertyCategory> builder)
    {
        builder.ToTable("PropertyCategories");
        builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
    }
}
