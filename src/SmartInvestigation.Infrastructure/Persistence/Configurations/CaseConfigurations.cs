using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartInvestigation.Domain.Entities;

namespace SmartInvestigation.Infrastructure.Persistence.Configurations;

public class FIRConfiguration : IEntityTypeConfiguration<FIR>
{
    public void Configure(EntityTypeBuilder<FIR> builder)
    {
        builder.ToTable("FIRs");
        builder.HasIndex(e => e.FIRNumber).IsUnique();
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.DateFiled);
        builder.HasIndex(e => e.PoliceStationId);
        builder.Property(e => e.FIRNumber).HasMaxLength(50).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(4000);
        builder.Property(e => e.PlaceOfOccurrence).HasMaxLength(500);
        builder.HasOne(e => e.PoliceStation).WithMany().HasForeignKey(e => e.PoliceStationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Complainant).WithMany().HasForeignKey(e => e.ComplainantPersonId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(e => e.IOAssigned).WithMany().HasForeignKey(e => e.IOAssignedUserId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class CaseConfiguration : IEntityTypeConfiguration<Case>
{
    public void Configure(EntityTypeBuilder<Case> builder)
    {
        builder.ToTable("Cases");
        builder.HasIndex(e => e.CaseNumber).IsUnique();
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.Priority);
        builder.HasIndex(e => e.AssignedOfficerId);
        builder.HasIndex(e => e.PoliceStationId);
        builder.HasIndex(e => e.DistrictId);
        builder.HasIndex(e => e.DateOfRegistration);
        builder.HasIndex(e => new { e.Status, e.Priority });
        builder.HasIndex(e => new { e.AssignedOfficerId, e.Status });
        builder.Property(e => e.CaseNumber).HasMaxLength(50).IsRequired();
        builder.Property(e => e.Title).HasMaxLength(500).IsRequired();
        builder.Property(e => e.Tags).HasMaxLength(1000);
        builder.HasOne(e => e.FIR).WithMany(f => f.Cases).HasForeignKey(e => e.FIRId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(e => e.CrimeType).WithMany().HasForeignKey(e => e.CrimeTypeId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(e => e.CrimeSubType).WithMany().HasForeignKey(e => e.CrimeSubTypeId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(e => e.PoliceStation).WithMany().HasForeignKey(e => e.PoliceStationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.District).WithMany().HasForeignKey(e => e.DistrictId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.AssignedOfficer).WithMany().HasForeignKey(e => e.AssignedOfficerId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(e => e.SupervisingOfficer).WithMany().HasForeignKey(e => e.SupervisingOfficerId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class ComplaintConfiguration : IEntityTypeConfiguration<Complaint>
{
    public void Configure(EntityTypeBuilder<Complaint> builder)
    {
        builder.ToTable("Complaints");
        builder.HasIndex(e => e.ComplaintNumber).IsUnique();
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.PoliceStationId);
        builder.HasIndex(e => e.DateOfIncident);
        builder.Property(e => e.ComplaintNumber).HasMaxLength(50).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(4000);
        builder.Property(e => e.ComplainantName).HasMaxLength(200);
        builder.Property(e => e.ComplainantPhone).HasMaxLength(20);
        builder.HasOne(e => e.PoliceStation).WithMany().HasForeignKey(e => e.PoliceStationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.ReceivedByUser).WithMany().HasForeignKey(e => e.ReceivedByUserId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class CaseComplaintConfiguration : IEntityTypeConfiguration<CaseComplaint>
{
    public void Configure(EntityTypeBuilder<CaseComplaint> builder)
    {
        builder.ToTable("CaseComplaints");
        builder.HasIndex(e => new { e.CaseId, e.ComplaintId }).IsUnique();
        builder.HasOne(e => e.Case).WithMany(c => c.CaseComplaints).HasForeignKey(e => e.CaseId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Complaint).WithMany(c => c.CaseComplaints).HasForeignKey(e => e.ComplaintId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class CaseStatusHistoryConfiguration : IEntityTypeConfiguration<CaseStatusHistory>
{
    public void Configure(EntityTypeBuilder<CaseStatusHistory> builder)
    {
        builder.ToTable("CaseStatusHistories");
        builder.HasIndex(e => e.CaseId);
        builder.HasOne(e => e.Case).WithMany(c => c.StatusHistory).HasForeignKey(e => e.CaseId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.ChangedByUser).WithMany().HasForeignKey(e => e.ChangedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class CaseTransferConfiguration : IEntityTypeConfiguration<CaseTransfer>
{
    public void Configure(EntityTypeBuilder<CaseTransfer> builder)
    {
        builder.ToTable("CaseTransfers");
        builder.HasIndex(e => e.CaseId);
        builder.HasOne(e => e.Case).WithMany(c => c.Transfers).HasForeignKey(e => e.CaseId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.FromOfficer).WithMany().HasForeignKey(e => e.FromOfficerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.ToOfficer).WithMany().HasForeignKey(e => e.ToOfficerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.FromStation).WithMany().HasForeignKey(e => e.FromStationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.ToStation).WithMany().HasForeignKey(e => e.ToStationId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class CaseMergeConfiguration : IEntityTypeConfiguration<CaseMerge>
{
    public void Configure(EntityTypeBuilder<CaseMerge> builder)
    {
        builder.ToTable("CaseMerges");
        builder.HasOne(e => e.PrimaryCase).WithMany().HasForeignKey(e => e.PrimaryCaseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.MergedCase).WithMany().HasForeignKey(e => e.MergedCaseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.MergedByUser).WithMany().HasForeignKey(e => e.MergedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class CaseNoteConfiguration : IEntityTypeConfiguration<CaseNote>
{
    public void Configure(EntityTypeBuilder<CaseNote> builder)
    {
        builder.ToTable("CaseNotes");
        builder.HasIndex(e => e.CaseId);
        builder.Property(e => e.Content).HasMaxLength(4000);
        builder.HasOne(e => e.Case).WithMany(c => c.Notes).HasForeignKey(e => e.CaseId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Officer).WithMany().HasForeignKey(e => e.OfficerId).OnDelete(DeleteBehavior.Restrict);
    }
}
