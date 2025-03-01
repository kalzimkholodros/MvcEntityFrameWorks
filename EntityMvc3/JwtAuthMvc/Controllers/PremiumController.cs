using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JwtAuthMvc.Data;
using JwtAuthMvc.Models;
using System.Security.Claims;
using JwtAuthMvc.Services;
using Microsoft.Extensions.Logging;

namespace JwtAuthMvc.Controllers
{
    public class PremiumController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;
        private const decimal PREMIUM_PRICE = 99.99M;
        private readonly ILogger<PremiumController> _logger;

        public PremiumController(ApplicationDbContext context, JwtService jwtService, ILogger<PremiumController> logger)
        {
            _context = context;
            _jwtService = jwtService;
            _logger = logger;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await GetCurrentUser();
                
                // Premium üyelik bilgilerini getir
                var activePremium = await _context.Premiums
                    .Where(p => p.UserId == user.Id && p.IsActive)
                    .OrderByDescending(p => p.ExpiryDate)
                    .FirstOrDefaultAsync();

                ViewBag.ActivePremium = activePremium;

                // Premium üye sayısı ve son 5 üyeyi getir
                ViewBag.TotalPremiumMembers = await _context.Premiums
                    .Where(p => p.IsActive)
                    .Select(p => p.UserId)
                    .Distinct()
                    .CountAsync();

                ViewBag.RecentPremiumMembers = await _context.Premiums
                    .Where(p => p.IsActive)
                    .Include(p => p.User)
                    .OrderByDescending(p => p.SubscriptionDate)
                    .Take(5)
                    .ToListAsync();

                return View(user);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Bir hata oluştu: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> BuyPremium()
        {
            try
            {
                var user = await GetCurrentUser();
                
                if (user.IsPremium)
                {
                    TempData["Error"] = "Zaten premium üyesiniz!";
                    return RedirectToAction(nameof(Index));
                }

                if (user.Balance < PREMIUM_PRICE)
                {
                    TempData["Error"] = $"Yetersiz bakiye. Premium üyelik için {PREMIUM_PRICE:C} gerekiyor.";
                    return RedirectToAction(nameof(Index));
                }

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var now = DateTime.UtcNow;
                    var expiryDate = now.AddMonths(1);

                    // Kullanıcı bakiyesini güncelle
                    user.Balance -= PREMIUM_PRICE;
                    user.IsPremium = true;
                    user.PremiumEndDate = expiryDate;

                    _context.Entry(user).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    // Premium kaydı oluştur
                    var premium = new Premium
                    {
                        UserId = user.Id,
                        SubscriptionDate = now,
                        ExpiryDate = expiryDate,
                        PaidAmount = PREMIUM_PRICE,
                        IsActive = true
                    };

                    await _context.Premiums.AddAsync(premium);
                    await _context.SaveChangesAsync();

                    // Yeni token oluştur ve güncelle
                    var token = _jwtService.GenerateToken(user);
                    HttpContext.Response.Cookies.Append("JWTToken", token,
                        new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict,
                            Expires = DateTime.UtcNow.AddHours(3)
                        });

                    await transaction.CommitAsync();
                    TempData["Success"] = "Premium üyelik başarıyla satın alındı!";
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(dbEx, "Veritabanı güncelleme hatası");
                    
                    var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                    TempData["Error"] = $"Veritabanı hatası: {innerMessage}";
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Premium üyelik satın alma hatası");
                    TempData["Error"] = $"Premium üyelik satın alınırken bir hata oluştu: {ex.Message}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Genel bir hata oluştu");
                TempData["Error"] = $"İşlem sırasında bir hata oluştu: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddBalance(decimal amount)
        {
            try
            {
                if (amount <= 0)
                {
                    TempData["Error"] = "Geçersiz miktar!";
                    return RedirectToAction(nameof(Index));
                }

                var user = await GetCurrentUser();
                user.Balance += amount;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"{amount:C} bakiyenize eklendi.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Bakiye eklenirken bir hata oluştu: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Policy = "PremiumOnly")]
        public IActionResult PremiumContent()
        {
            return View();
        }

        private async Task<User> GetCurrentUser()
        {
            var username = User.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                throw new Exception("Kullanıcı bulunamadı");
            }
            return user;
        }
    }
} 