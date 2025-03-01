using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JwtAuthMvc.Models;
using JwtAuthMvc.Data;

namespace JwtAuthMvc.Controllers;

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
        var username = User.Identity?.Name;
        var user = username != null ? await _context.Users.FirstOrDefaultAsync(u => u.Username == username) : null;
            
        var products = await _context.Products
            .Where(p => !p.IsPremium || (user != null && user.IsPremium))
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        ViewBag.IsAdmin = user?.Role == "Admin";
        ViewBag.IsPremium = user?.IsPremium ?? false;
            
        return View(products);
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
