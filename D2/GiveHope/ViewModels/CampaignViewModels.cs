using System.ComponentModel.DataAnnotations;

namespace GiveHope.ViewModels;

public class CampaignCardViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string NgoName { get; set; } = string.Empty;
    public decimal Target { get; set; }
    public decimal Raised { get; set; }

    public decimal Remaining => Math.Max(0, Target - Raised);

    public string Status { get; set; } = string.Empty;

    public int Percent => Target <= 0
        ? 0
        : (int)Math.Min(100, Math.Round(Raised / Target * 100, MidpointRounding.AwayFromZero));
}

public class HomeIndexViewModel
{
    public List<CampaignCardViewModel> Campaigns { get; set; } = new();
}

public class DonateViewModel
{
    public int CampaignId { get; set; }
    public string CampaignTitle { get; set; } = string.Empty;
    public string NgoName { get; set; } = string.Empty;
    public decimal Target { get; set; }
    public decimal Raised { get; set; }

    public decimal Remaining => Math.Max(0, Target - Raised);

    [Required(ErrorMessage = "Please enter an amount.")]
    [Range(1, 10000000, ErrorMessage = "Enter an amount between Rs. 1 and Rs. 10,000,000.")]
    [Display(Name = "Amount (Rs.)")]
    public decimal Amount { get; set; }
}

public class DonationRowViewModel
{
    public string CampaignTitle { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime DonatedAt { get; set; }
}

public class DonorDashboardViewModel
{
    public List<CampaignCardViewModel> Campaigns { get; set; } = new();
    public List<DonationRowViewModel> MyDonations { get; set; } = new();
    public decimal TotalGiven { get; set; }
}

public class CampaignFormViewModel
{
    [Required]
    [StringLength(150)]
    [Display(Name = "Campaign title")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [Range(1, 100000000, ErrorMessage = "Target must be between Rs. 1 and Rs. 100,000,000.")]
    [Display(Name = "Target amount (Rs.)")]
    public decimal TargetAmount { get; set; }

    [Required]
    [StringLength(1000)]
    [Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;
}

public class NgoDashboardViewModel
{
    public List<CampaignCardViewModel> Campaigns { get; set; } = new();
    public CampaignFormViewModel NewCampaign { get; set; } = new();
}

public class AdminDashboardViewModel
{
    public List<CampaignCardViewModel> Campaigns { get; set; } = new();
    public decimal TotalRaised { get; set; }
}
