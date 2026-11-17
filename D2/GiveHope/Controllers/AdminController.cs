using GiveHope.Data;
using GiveHope.Helpers;
using GiveHope.Models;
using GiveHope.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GiveHope.Controllers;

[Authorize(Roles = DbSeeder.AdminRole)]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _db;

    public AdminController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var campaigns = await _db.Campaigns
            .AsNoTracking()
            .Include(c => c.Ngo)
            .Include(c => c.Donations)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        var cards = campaigns.Select(CampaignMapper.ToCard).ToList();

        return View(new AdminDashboardViewModel
        {
            Campaigns = cards,
            TotalRaised = cards.Sum(c => c.Raised)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveCampaign(int id)
    {
        return await SetStatusAsync(id, CampaignStatus.Active, "Campaign approved and now live.");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectCampaign(int id)
    {
        return await SetStatusAsync(id, CampaignStatus.Rejected, "Campaign rejected.");
    }

    private async Task<IActionResult> SetStatusAsync(int id, CampaignStatus status, string message)
    {
        var campaign = await _db.Campaigns.FindAsync(id);
        if (campaign == null)
        {
            return NotFound();
        }

        campaign.Status = status;
        await _db.SaveChangesAsync();

        TempData["Success"] = message;
        return RedirectToAction(nameof(Index));
    }
}
