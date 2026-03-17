using Microsoft.EntityFrameworkCore;
using SmartInvestigation.Domain.Entities;
using SmartInvestigation.Domain.Enums;

namespace SmartInvestigation.Infrastructure.Persistence.SeedData;

public static class DataSeeder
{
    private const string DefaultAdminUsername = "admin";
    private const string DefaultAdminPasswordHash = "$2a$11$f365PtGvQ30YswbIaWhul.i7uoRBV.7RILCJEj1kjcITvGUL5tevK";

    public static async Task SeedAsync(AppDbContext context)
    {
        // Render Workaround: EnsureCreatedAsync causes Status 139 (Segfault) on Alpine/Linux.
        // We will run `dotnet ef database update` manually or via CI/CD instead of at runtime.
        // await context.Database.EnsureCreatedAsync(); 

        Role? adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "SuperAdmin");

        if (!await context.Roles.AnyAsync())
        {
            adminRole = new Role { Name = "SuperAdmin", Description = "System Administrator", Level = 10, IsSystemRole = true, CreatedBy = "System" };
            var shoRole = new Role { Name = "StationHouseOfficer", Description = "Station Commander", Level = 8, IsSystemRole = true, CreatedBy = "System" };
            var ioRole = new Role { Name = "InvestigatingOfficer", Description = "Case IO", Level = 6, IsSystemRole = true, CreatedBy = "System" };
            
            await context.Roles.AddRangeAsync(adminRole, shoRole, ioRole);
            await context.SaveChangesAsync();
        }

        adminRole ??= await context.Roles.FirstAsync(r => r.Name == "SuperAdmin");

        var adminUser = await context.Set<User>()
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Username == DefaultAdminUsername);

        if (adminUser == null)
        {
            adminUser = new User
            {
                Username = DefaultAdminUsername,
                Email = "admin@police.gov",
                FullName = "System Administrator",
                Rank = "Admin",
                PasswordHash = DefaultAdminPasswordHash,
                PasswordSalt = null,
                IsActive = true,
                CreatedBy = "System"
            };
            await context.Set<User>().AddAsync(adminUser);
            await context.SaveChangesAsync();
        }
        else if (adminUser.CreatedBy == "System" || adminUser.Email == "admin@police.gov")
        {
            // Keep the development seed account usable if an old incorrect hash was stored.
            adminUser.PasswordHash = DefaultAdminPasswordHash;
            adminUser.PasswordSalt = null;
            adminUser.IsActive = true;
            await context.SaveChangesAsync();
        }

        if (!await context.Set<UserRole>().AnyAsync(ur => ur.UserId == adminUser.Id && ur.RoleId == adminRole.Id))
        {
            await context.Set<UserRole>().AddAsync(new UserRole { UserId = adminUser.Id, RoleId = adminRole.Id, CreatedBy = "System" });
            await context.SaveChangesAsync();
        }

        // --- Seed SHO user ---
        var seedShoRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "StationHouseOfficer");
        if (seedShoRole != null && !await context.Set<User>().AnyAsync(u => u.Username == "sho01"))
        {
            var shoUser = new User
            {
                Username = "sho01",
                Email = "sho01@police.gov",
                FullName = "Inspector Vikram Rathore",
                Rank = "Inspector",
                BadgeNumber = "SHO-2024-001",
                PasswordHash = DefaultAdminPasswordHash, // admin123
                PasswordSalt = null,
                IsActive = true,
                CreatedBy = "System"
            };
            await context.Set<User>().AddAsync(shoUser);
            await context.SaveChangesAsync();
            await context.Set<UserRole>().AddAsync(new UserRole { UserId = shoUser.Id, RoleId = seedShoRole.Id, CreatedBy = "System" });
            await context.SaveChangesAsync();
        }

        // --- Seed IO user ---
        var seedIoRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "InvestigatingOfficer");
        if (seedIoRole != null && !await context.Set<User>().AnyAsync(u => u.Username == "io01"))
        {
            var ioUser = new User
            {
                Username = "io01",
                Email = "io01@police.gov",
                FullName = "SI Anjali Mehta",
                Rank = "Sub-Inspector",
                BadgeNumber = "IO-2024-001",
                PasswordHash = DefaultAdminPasswordHash, // admin123
                PasswordSalt = null,
                IsActive = true,
                CreatedBy = "System"
            };
            await context.Set<User>().AddAsync(ioUser);
            await context.SaveChangesAsync();
            await context.Set<UserRole>().AddAsync(new UserRole { UserId = ioUser.Id, RoleId = seedIoRole.Id, CreatedBy = "System" });
            await context.SaveChangesAsync();
        }

        if (!await context.Set<CrimeType>().AnyAsync())
        {
            var theft = new CrimeType { Name = "Theft", Code = "THFT", Severity = "Low", SortOrder = 1, CreatedBy = "System" };
            var murder = new CrimeType { Name = "Murder", Code = "MURD", Severity = "Critical", SortOrder = 2, CreatedBy = "System" };
            var cyber = new CrimeType { Name = "Cyber Crime", Code = "CYBR", Severity = "High", SortOrder = 3, CreatedBy = "System" };
            
            await context.Set<CrimeType>().AddRangeAsync(theft, murder, cyber);
            await context.SaveChangesAsync();
        }

        if (!await context.Set<LegalSection>().AnyAsync())
        {
            await context.Set<LegalSection>().AddRangeAsync(
                new LegalSection { Act = "Bharatiya Nyaya Sanhita", Code = "103", Title = "Murder", Description = "Punishment for Murder", MaxPenalty = "Death or imprisonment for life, and fine", IsBailable = false, IsCognizable = true, CreatedBy = "System" },
                new LegalSection { Act = "Bharatiya Nyaya Sanhita", Code = "303", Title = "Theft", Description = "Theft", MaxPenalty = "Imprisonment up to 3 years, or fine, or both", IsBailable = false, IsCognizable = true, CreatedBy = "System" }
            );
            await context.SaveChangesAsync();
        }

        if (!await context.Set<State>().AnyAsync())
        {
            var dl = new State { Name = "Delhi", Code = "DL", CreatedBy = "System" };
            var mh = new State { Name = "Maharashtra", Code = "MH", CreatedBy = "System" };
            await context.Set<State>().AddRangeAsync(dl, mh);
            await context.SaveChangesAsync();

            var nd = new District { Name = "New Delhi", Code = "NDL", StateId = dl.Id, CreatedBy = "System" };
            await context.Set<District>().AddAsync(nd);
            await context.SaveChangesAsync();

            await context.Set<PoliceStation>().AddAsync(new PoliceStation { Name = "Parliament Street", Code = "PS-ND-01", DistrictId = nd.Id, CreatedBy = "System" });
            await context.SaveChangesAsync();
        }
    }
}
