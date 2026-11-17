using GiveHope.Data;
using GiveHope.Helpers;
using GiveHope.Models;
using GiveHope.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GiveHope.Controllers;

[Authorize(Roles = DbSeeder.NgoRole)]
public class NgoController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public NgoController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        return View(await BuildDashboardAsync(new CampaignFormViewModel()));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CampaignFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", await BuildDashboardAsync(model));
        }

        _db.Campaigns.Add(new Campaign
        {
            Title = model.Title,
            Description = model.Description,
            TargetAmount = model.TargetAmount,
            NgoId = _userManager.GetUserId(User)!,
            CreatedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();

        TempData["Success"] = "Campaign submitted. It will show on the site after admin approval.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<NgoDashboardViewModel> BuildDashboardAsync(CampaignFormViewModel form)
    {
        var userId = _userManager.GetUserId(User)!;

        var campaigns = await _db.Campaigns
            .AsNoTracking()
            .Include(c => c.Ngo)
            .Include(c => c.Donations)
            .Where(c => c.NgoId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return new NgoDashboardViewModel
        {
            Campaigns = campaigns.Select(CampaignMapper.ToCard).ToList(),
            NewCampaign = form
        };
    }
}
