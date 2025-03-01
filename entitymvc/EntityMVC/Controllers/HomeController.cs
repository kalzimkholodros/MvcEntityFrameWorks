using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EntityMVC.Models;
using EntityMVC.Data;
using Microsoft.EntityFrameworkCore;

namespace EntityMVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Önce tüm üyeleri çekelim
        var members = await _context.Members.ToListAsync();
        
        var stats = new
        {
            TotalMembers = members.Count,
            NewMembersToday = members.Count(m => m.RegisterDate.Date == DateTime.UtcNow.Date),
            AverageAge = members.Any() ? 
                members.Where(m => m.BirthDate != default)
                      .Select(m => (int)((DateTime.UtcNow - m.BirthDate).TotalDays / 365.25))
                      .DefaultIfEmpty(0)
                      .Average() 
                : 0
        };

        return View(stats);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
