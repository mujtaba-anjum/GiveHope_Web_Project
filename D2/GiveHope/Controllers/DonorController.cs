using GiveHope.Data;
using GiveHope.Helpers;
using GiveHope.Models;
using GiveHope.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GiveHope.Controllers;

[Authorize(Roles = DbSeeder.DonorRole)]
public class DonorController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public DonorController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User)!;

        var campaigns = await _db.Campaigns
            .AsNoTracking()
            .Include(c => c.Ngo)
            .Include(c => c.Donations)
            .Where(c => c.Status == CampaignStatus.Active)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        var donations = await _db.Donations
            .AsNoTracking()
            .Include(d => d.Campaign)
            .Where(d => d.DonorId == userId)
            .OrderByDescending(d => d.DonatedAt)
            .ToListAsync();

        var model = new DonorDashboardViewModel
        {
            Campaigns = campaigns.Select(CampaignMapper.ToCard).ToList(),
            MyDonations = donations.Select(d => new DonationRowViewModel
            {
                CampaignTitle = d.Campaign?.Title ?? string.Empty,
                Amount = d.Amount,
                DonatedAt = d.DonatedAt
            }).ToList(),
            TotalGiven = donations.Sum(d => d.Amount)
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Donate(int id)
    {
        var campaign = await _db.Campaigns
            .AsNoTracking()
            .Include(c => c.Ngo)
            .Include(c => c.Donations)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (campaign == null || campaign.Status != CampaignStatus.Active)
        {
            return NotFound();
        }

        return View(CampaignMapper.ToDonate(campaign));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Donate(DonateViewModel model)
    {
        var campaign = await _db.Campaigns
            .Include(c => c.Ngo)
            .Include(c => c.Donations)
            .FirstOrDefaultAsync(c => c.Id == model.CampaignId);

        if (campaign == null || campaign.Status != CampaignStatus.Active)
        {
            return NotFound();
        }

        var fresh = CampaignMapper.ToDonate(campaign);
        fresh.Amount = model.Amount;

        if (!ModelState.IsValid)
        {
            return View(fresh);
        }

        if (model.Amount > fresh.Remaining)
        {
            ModelState.AddModelError(nameof(model.Amount),
                $"The campaign only needs Rs. {fresh.Remaining:N0} more.");
            return View(fresh);
        }

        _db.Donations.Add(new Donation
        {
            CampaignId = campaign.Id,
            DonorId = _userManager.GetUserId(User)!,
            Amount = model.Amount,
            DonatedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();

        TempData["Success"] = $"Thank you! Rs. {model.Amount:N0} donated to \"{campaign.Title}\".";
        return RedirectToAction(nameof(Index));
    }
}
