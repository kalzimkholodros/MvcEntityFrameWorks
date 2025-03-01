using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using EntityMvc.Models;
using EntityMvc.ViewModels;
using System.Threading.Tasks;

namespace EntityMvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if username already exists
                var existingUser = await _userManager.FindByNameAsync(model.UserName);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "Bu kullanıcı adı zaten kullanılıyor.");
                    return View(model);
                }

                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = $"{model.UserName}@example.com", // Geçici email oluştur
                    Balance = 100 // Başlangıç bakiyesi
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    TempData["SuccessMessage"] = "Kayıt işlemi başarılı! Hoş geldiniz.";
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    string errorMessage = error.Code switch
                    {
                        "PasswordRequiresUpper" => "Şifre en az bir büyük harf içermelidir.",
                        "PasswordRequiresLower" => "Şifre en az bir küçük harf içermelidir.",
                        "PasswordRequiresDigit" => "Şifre en az bir rakam içermelidir.",
                        "PasswordTooShort" => "Şifre en az 6 karakter uzunluğunda olmalıdır.",
                        _ => error.Description
                    };
                    ModelState.AddModelError(string.Empty, errorMessage);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Kullanıcı adı bulunamadı.");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(
                    model.UserName,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Giriş başarılı! Hoş geldiniz.";
                    return RedirectToLocal(returnUrl);
                }
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Hesabınız kilitlendi. Lütfen daha sonra tekrar deneyin.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Geçersiz şifre.");
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["SuccessMessage"] = "Başarıyla çıkış yapıldı.";
            return RedirectToAction("Index", "Home");
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
} 