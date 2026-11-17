using GiveHope.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GiveHope.Data;

public static class DbSeeder
{
    public const string AdminRole = "Admin";
    public const string DonorRole = "Donor";
    public const string NgoRole = "Ngo";

    public const string DemoPassword = "Demo@123";

    public static async Task SeedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var role in new[] { AdminRole, DonorRole, NgoRole })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var admin = await CreateUserAsync(userManager, "admin@givehope.com", "System Administrator", null, AdminRole, "admin123");
        var ali = await CreateUserAsync(userManager, "donor@givehope.local", "Ali Donor", "Lahore", DonorRole);
        var sara = await CreateUserAsync(userManager, "sara@givehope.local", "Sara Khan", "Islamabad", DonorRole);
        var bilal = await CreateUserAsync(userManager, "bilal@givehope.local", "Bilal Ahmed", "Karachi", DonorRole);
        var roshni = await CreateUserAsync(userManager, "roshni@givehope.local", "Roshni Education Trust", "Lahore", NgoRole);
        var umeed = await CreateUserAsync(userManager, "umeed@givehope.local", "Umeed Relief Society", "Karachi", NgoRole);

        if (await db.Campaigns.AnyAsync())
        {
            return;
        }

        var schoolFees = new Campaign
        {
            Title = "School Fees for Children",
            Description = "Paying school fees for children from low-income families so they can keep learning.",
            TargetAmount = 50000,
            Status = CampaignStatus.Active,
            Ngo = roshni,
            CreatedAt = new DateTime(2026, 10, 1)
        };

        var rations = new Campaign
        {
            Title = "Ration Packs for Families",
            Description = "Monthly ration packs with flour, rice, pulses and oil for families in need.",
            TargetAmount = 100000,
            Status = CampaignStatus.Active,
            Ngo = umeed,
            CreatedAt = new DateTime(2026, 10, 5)
        };

        var books = new Campaign
        {
            Title = "Books for Library",
            Description = "Buying textbooks and story books for a community library.",
            TargetAmount = 20000,
            Status = CampaignStatus.Active,
            Ngo = roshni,
            CreatedAt = new DateTime(2026, 11, 10)
        };

        db.Campaigns.AddRange(schoolFees, rations, books);

        db.Donations.AddRange(
            new Donation { Campaign = schoolFees, Donor = ali, Amount = 2000, DonatedAt = new DateTime(2026, 11, 2) },
            new Donation { Campaign = schoolFees, Donor = sara, Amount = 28000, DonatedAt = new DateTime(2026, 10, 20) },
            new Donation { Campaign = rations, Donor = ali, Amount = 1000, DonatedAt = new DateTime(2026, 10, 25) },
            new Donation { Campaign = rations, Donor = bilal, Amount = 39000, DonatedAt = new DateTime(2026, 10, 12) }
        );

        await db.SaveChangesAsync();
    }

    private static async Task<ApplicationUser> CreateUserAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string fullName,
        string? city,
        string role,
        string password = DemoPassword)
    {
        var existing = await userManager.FindByEmailAsync(email);
        if (existing != null)
        {
            return existing;
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            FullName = fullName,
            City = city
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                "Seeding user failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        await userManager.AddToRoleAsync(user, role);
        return user;
    }
}
