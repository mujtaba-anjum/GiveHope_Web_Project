using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiveHope.Models;

public enum CampaignStatus
{
    Pending,
    Active,
    Rejected
}

public class Campaign
{
    public int Id { get; set; }

    [Required, StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal TargetAmount { get; set; }

    public CampaignStatus Status { get; set; } = CampaignStatus.Pending;

    public string NgoId { get; set; } = string.Empty;
    public ApplicationUser? Ngo { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Donation> Donations { get; set; } = new List<Donation>();
}
