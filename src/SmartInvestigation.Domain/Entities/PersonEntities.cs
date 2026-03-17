using SmartInvestigation.Domain.Common;
using SmartInvestigation.Domain.Enums;

namespace SmartInvestigation.Domain.Entities;

/// <summary>
/// Central person registry — suspects, victims, witnesses, etc.
/// </summary>
public class Person : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string? FatherName { get; set; }
    public string? MotherName { get; set; }
    public string? AadhaarHash { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int? ApproximateAge { get; set; }
    public Gender Gender { get; set; }
    public string? NationalityCode { get; set; }
    public string? ReligionCode { get; set; }
    public string? CasteCategory { get; set; }
    public string? Occupation { get; set; }
    public string? EducationLevel { get; set; }
    public string? IdentificationMarks { get; set; }
    public string? PhysicalDescription { get; set; }
    public double? Height { get; set; }
    public string? BloodGroup { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public bool IsRepeatOffender { get; set; }

    // Navigation
    public virtual ICollection<PersonAlias> Aliases { get; set; } = new List<PersonAlias>();
    public virtual ICollection<PersonAddress> Addresses { get; set; } = new List<PersonAddress>();
    public virtual ICollection<PersonContact> Contacts { get; set; } = new List<PersonContact>();
    public virtual ICollection<PersonBiometric> Biometrics { get; set; } = new List<PersonBiometric>();
    public virtual ICollection<PersonPhoto> Photos { get; set; } = new List<PersonPhoto>();
    public virtual ICollection<CasePerson> CasePersons { get; set; } = new List<CasePerson>();
    public virtual ICollection<PersonCriminalHistory> CriminalHistory { get; set; } = new List<PersonCriminalHistory>();
    public virtual ICollection<OrganizationMember> OrganizationMemberships { get; set; } = new List<OrganizationMember>();
}

public class PersonAlias : BaseEntity
{
    public Guid PersonId { get; set; }
    public string AliasName { get; set; } = string.Empty;
    public string? AliasType { get; set; }

    public virtual Person Person { get; set; } = null!;
}

public class PersonAddress : BaseEntity
{
    public Guid PersonId { get; set; }
    public string AddressType { get; set; } = "Current"; // Current, Permanent, Office
    public string? HouseNumber { get; set; }
    public string? Street { get; set; }
    public string? Locality { get; set; }
    public string? City { get; set; }
    public string? DistrictName { get; set; }
    public string? StateName { get; set; }
    public string? PinCode { get; set; }
    public string? Country { get; set; } = "India";
    public double? GpsLatitude { get; set; }
    public double? GpsLongitude { get; set; }
    public bool IsPrimary { get; set; }

    public virtual Person Person { get; set; } = null!;
}

public class PersonContact : BaseEntity
{
    public Guid PersonId { get; set; }
    public string ContactType { get; set; } = "Phone"; // Phone, Email, Social
    public string Value { get; set; } = string.Empty;
    public string? Platform { get; set; } // WhatsApp, Facebook, etc.
    public bool IsPrimary { get; set; }
    public bool IsVerified { get; set; }

    public virtual Person Person { get; set; } = null!;
}

public class PersonBiometric : BaseEntity
{
    public Guid PersonId { get; set; }
    public BiometricType BiometricType { get; set; }
    public string DataHash { get; set; } = string.Empty;
    public string? StorageReference { get; set; }
    public DateTime CapturedDate { get; set; }
    public string? CapturedBy { get; set; }

    public virtual Person Person { get; set; } = null!;
}

public class PersonPhoto : BaseEntity
{
    public Guid PersonId { get; set; }
    public string PhotoUrl { get; set; } = string.Empty;
    public bool IsMugshot { get; set; }
    public string? PhotoType { get; set; } // Front, Side, Full
    public DateTime CapturedDate { get; set; }
    public string? Description { get; set; }

    public virtual Person Person { get; set; } = null!;
}

public class CasePerson : BaseEntity
{
    public Guid CaseId { get; set; }
    public Guid PersonId { get; set; }
    public PersonRole Role { get; set; }
    public PersonStatus Status { get; set; } = PersonStatus.Active;
    public DateTime? ArrestDate { get; set; }
    public string? ArrestLocation { get; set; }
    public DateTime? BailDate { get; set; }
    public string? Remarks { get; set; }
    public bool IsMainAccused { get; set; }

    public virtual Case Case { get; set; } = null!;
    public virtual Person Person { get; set; } = null!;
}

public class PersonCriminalHistory : BaseEntity
{
    public Guid PersonId { get; set; }
    public string? CaseReference { get; set; }
    public string? CrimeDescription { get; set; }
    public string? CourtName { get; set; }
    public bool IsConvicted { get; set; }
    public string? Sentence { get; set; }
    public DateTime? ConvictionDate { get; set; }
    public string? PoliceStationName { get; set; }
    public string? Sections { get; set; }

    public virtual Person Person { get; set; } = null!;
}

public class Organization : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Gang, Company, NGO, Terror
    public string? Description { get; set; }
    public string? HeadquartersAddress { get; set; }
    public bool IsActive { get; set; } = true;
    public string? ThreatLevel { get; set; }

    public virtual ICollection<OrganizationMember> Members { get; set; } = new List<OrganizationMember>();
}

public class OrganizationMember : BaseEntity
{
    public Guid OrganizationId { get; set; }
    public Guid PersonId { get; set; }
    public string? Role { get; set; }
    public DateTime? JoinDate { get; set; }
    public DateTime? LeaveDate { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual Organization Organization { get; set; } = null!;
    public virtual Person Person { get; set; } = null!;
}
