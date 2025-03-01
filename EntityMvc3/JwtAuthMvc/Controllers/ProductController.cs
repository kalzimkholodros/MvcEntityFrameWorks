using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JwtAuthMvc.Data;
using JwtAuthMvc.Models;

namespace JwtAuthMvc.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
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

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                product.CreatedAt = DateTime.UtcNow;
                product.CreatedBy = User.Identity?.Name ?? "Unknown";
                
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Ürün başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }
    }
} 