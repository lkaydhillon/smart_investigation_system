using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartInvestigation.Domain.Entities;

namespace SmartInvestigation.Infrastructure.Persistence.Configurations;

public class PoliceStationConfiguration : IEntityTypeConfiguration<PoliceStation>
{
    public void Configure(EntityTypeBuilder<PoliceStation> builder)
    {
        builder.ToTable("PoliceStations");
        builder.HasIndex(e => e.Code).IsUnique();
        builder.Property(e => e.Name).HasMaxLength(300).IsRequired();
        builder.Property(e => e.Code).HasMaxLength(20).IsRequired();
        builder.HasOne(e => e.District).WithMany(d => d.PoliceStations).HasForeignKey(e => e.DistrictId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class DistrictConfiguration : IEntityTypeConfiguration<District>
{
    public void Configure(EntityTypeBuilder<District> builder)
    {
        builder.ToTable("Districts");
        builder.HasIndex(e => e.Code).IsUnique();
        builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Code).HasMaxLength(20).IsRequired();
        builder.HasOne(e => e.State).WithMany(s => s.Districts).HasForeignKey(e => e.StateId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class StateConfiguration : IEntityTypeConfiguration<State>
{
    public void Configure(EntityTypeBuilder<State> builder)
    {
        builder.ToTable("States");
        builder.HasIndex(e => e.Code).IsUnique();
        builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Code).HasMaxLength(10).IsRequired();
    }
}

public class CrimeSceneConfiguration : IEntityTypeConfiguration<CrimeScene>
{
    public void Configure(EntityTypeBuilder<CrimeScene> builder)
    {
        builder.ToTable("CrimeScenes");
        builder.HasIndex(e => e.CaseId);
        builder.HasOne(e => e.Case).WithMany(c => c.CrimeScenes).HasForeignKey(e => e.CaseId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class PatrolZoneConfiguration : IEntityTypeConfiguration<PatrolZone>
{
    public void Configure(EntityTypeBuilder<PatrolZone> builder)
    {
        builder.ToTable("PatrolZones");
        builder.HasOne(e => e.Station).WithMany(s => s.PatrolZones).HasForeignKey(e => e.StationId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class GeoFenceConfiguration : IEntityTypeConfiguration<GeoFence>
{
    public void Configure(EntityTypeBuilder<GeoFence> builder)
    {
        builder.ToTable("GeoFences");
        builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
    }
}

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicles");
        builder.HasIndex(e => e.RegistrationNumber);
        builder.HasIndex(e => e.CaseId);
        builder.HasOne(e => e.Case).WithMany(c => c.Vehicles).HasForeignKey(e => e.CaseId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Owner).WithMany().HasForeignKey(e => e.OwnerPersonId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class VehicleInterceptionConfiguration : IEntityTypeConfiguration<VehicleInterception>
{
    public void Configure(EntityTypeBuilder<VehicleInterception> builder)
    {
        builder.ToTable("VehicleInterceptions");
        builder.HasOne(e => e.Vehicle).WithMany(v => v.Interceptions).HasForeignKey(e => e.VehicleId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.InterceptedByUser).WithMany().HasForeignKey(e => e.InterceptedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class WeaponConfiguration : IEntityTypeConfiguration<Weapon>
{
    public void Configure(EntityTypeBuilder<Weapon> builder)
    {
        builder.ToTable("Weapons");
        builder.HasIndex(e => e.CaseId);
        builder.HasOne(e => e.Case).WithMany(c => c.Weapons).HasForeignKey(e => e.CaseId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.RecoveredFromPerson).WithMany().HasForeignKey(e => e.RecoveredFromPersonId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class WeaponBallisticConfiguration : IEntityTypeConfiguration<WeaponBallistic>
{
    public void Configure(EntityTypeBuilder<WeaponBallistic> builder)
    {
        builder.ToTable("WeaponBallistics");
        builder.HasOne(e => e.Weapon).WithOne(w => w.BallisticReport).HasForeignKey<WeaponBallistic>(e => e.WeaponId).OnDelete(DeleteBehavior.Cascade);
    }
}
