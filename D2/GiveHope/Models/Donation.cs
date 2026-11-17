using System.ComponentModel.DataAnnotations.Schema;

namespace GiveHope.Models;

public class Donation
{
    public int Id { get; set; }

    public int CampaignId { get; set; }
    public Campaign? Campaign { get; set; }

    public string DonorId { get; set; } = string.Empty;
    public ApplicationUser? Donor { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    public DateTime DonatedAt { get; set; } = DateTime.UtcNow;
}
