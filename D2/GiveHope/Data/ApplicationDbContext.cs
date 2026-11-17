using GiveHope.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GiveHope.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Campaign> Campaigns => Set<Campaign>();
    public DbSet<Donation> Donations => Set<Donation>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Campaign>()
            .Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Entity<Campaign>()
            .HasOne(c => c.Ngo)
            .WithMany()
            .HasForeignKey(c => c.NgoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Donation>()
            .HasOne(d => d.Campaign)
            .WithMany(c => c.Donations)
            .HasForeignKey(d => d.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Donation>()
            .HasOne(d => d.Donor)
            .WithMany()
            .HasForeignKey(d => d.DonorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
