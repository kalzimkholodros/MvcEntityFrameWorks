using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JwtAuthMvc.Data;
using JwtAuthMvc.Models;
using JwtAuthMvc.Services;
using BCrypt.Net;
using System.Security.Cryptography;
using System.Text;

namespace JwtAuthMvc.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;

        public AuthController(ApplicationDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Geçersiz kullanıcı adı veya şifre");
                return View();
            }

            bool isValidPassword = false;

            try
            {
                // Önce BCrypt ile kontrol et
                isValidPassword = BCrypt.Net.BCrypt.Verify(password, user.Password);

                // Eğer BCrypt ile eşleşmezse, SHA256 ile dene
                if (!isValidPassword)
                {
                    var oldHashedPassword = HashPasswordSHA256(password);
                    if (user.Password == oldHashedPassword)
                    {
                        // Eski format eşleşti, BCrypt'e çevir
                        user.Password = BCrypt.Net.BCrypt.HashPassword(password);
                        _context.Update(user);
                        await _context.SaveChangesAsync();
                        isValidPassword = true;
                    }
                }
            }
            catch
            {
                // BCrypt verify hatası verirse (format uyumsuzluğu), SHA256 ile dene
                var oldHashedPassword = HashPasswordSHA256(password);
                if (user.Password == oldHashedPassword)
                {
                    // Eski format eşleşti, BCrypt'e çevir
                    user.Password = BCrypt.Net.BCrypt.HashPassword(password);
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    isValidPassword = true;
                }
            }

            if (!isValidPassword)
            {
                ModelState.AddModelError(string.Empty, "Geçersiz kullanıcı adı veya şifre");
                return View();
            }

            var token = _jwtService.GenerateToken(user);
            HttpContext.Response.Cookies.Append("JWTToken", token,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.Now.AddHours(3)
                });

            return RedirectToAction("Index", "Home");
        }

        private string HashPasswordSHA256(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
            {
                ModelState.AddModelError("Username", "Bu kullanıcı adı zaten kullanılıyor");
                return View(user);
            }

            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Bu e-posta adresi zaten kullanılıyor");
                return View(user);
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.Role = "User";

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Login));
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("JWTToken");
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> CreateAdmin()
        {
            // Admin kullanıcısı zaten var mı kontrol et
            var adminExists = await _context.Users.AnyAsync(u => u.Username == "admin");
            if (adminExists)
            {
                return Content("Admin kullanıcısı zaten mevcut.");
            }

            // Admin kullanıcısını oluştur
            var admin = new User
            {
                Username = "admin",
                Password = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Email = "admin@example.com",
                Role = "Admin",
                Balance = 0,
                IsPremium = true,
                PremiumEndDate = new DateTime(2099, 12, 31)
            };

            await _context.Users.AddAsync(admin);
            await _context.SaveChangesAsync();

            return Content("Admin kullanıcısı başarıyla oluşturuldu.");
        }
    }
} 