using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthMvc.Controllers
{
    [Authorize] // Bu controller'a erişim için token gerekli
    public class TestController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            // Giriş yapmış kullanıcının bilgilerini gösterelim
            var username = User.Identity?.Name;
            var userRole = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;

            ViewBag.Username = username;
            ViewBag.Role = userRole;
            
            return View();
        }
    }
} 