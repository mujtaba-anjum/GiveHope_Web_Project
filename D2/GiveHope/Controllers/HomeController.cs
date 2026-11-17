using System.Diagnostics;
using GiveHope.Data;
using GiveHope.Helpers;
using GiveHope.Models;
using GiveHope.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GiveHope.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;

    public HomeController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var campaigns = await _db.Campaigns
            .AsNoTracking()
            .Include(c => c.Ngo)
            .Include(c => c.Donations)
            .Where(c => c.Status == CampaignStatus.Active)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return View(new HomeIndexViewModel
        {
            Campaigns = campaigns.Select(CampaignMapper.ToCard).ToList()
        });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
