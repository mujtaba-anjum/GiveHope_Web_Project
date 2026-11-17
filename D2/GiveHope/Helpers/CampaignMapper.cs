using GiveHope.Models;
using GiveHope.ViewModels;

namespace GiveHope.Helpers;

// Converts database campaigns into view models. Donations and Ngo must be included in the query.
public static class CampaignMapper
{
    public static CampaignCardViewModel ToCard(Campaign campaign) => new()
    {
        Id = campaign.Id,
        Title = campaign.Title,
        Description = campaign.Description,
        NgoName = campaign.Ngo?.FullName ?? string.Empty,
        Target = campaign.TargetAmount,
        Raised = campaign.Donations.Sum(d => d.Amount),
        Status = campaign.Status.ToString()
    };

    public static DonateViewModel ToDonate(Campaign campaign) => new()
    {
        CampaignId = campaign.Id,
        CampaignTitle = campaign.Title,
        NgoName = campaign.Ngo?.FullName ?? string.Empty,
        Target = campaign.TargetAmount,
        Raised = campaign.Donations.Sum(d => d.Amount)
    };
}
