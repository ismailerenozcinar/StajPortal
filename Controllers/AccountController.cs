using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StajPortal.Data;
using StajPortal.Models.Entities;
using StajPortal.Models.ViewModels;

namespace StajPortal.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ActionName("Register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterPost(RegisterViewModel model)
        {
            // DEBUG: Log that we're in POST method
            Console.WriteLine("POST Register method called!");
            
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    Role = model.Role,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Role'e göre profil oluştur
                    if (model.Role == "Student")
                    {
                        var studentProfile = new StudentProfile
                        {
                            UserId = user.Id,
                            FullName = model.FullName,
                            UpdatedAt = DateTime.UtcNow
                        };
                        _context.StudentProfiles.Add(studentProfile);
                    }
                    else if (model.Role == "Company")
                    {
                        var companyProfile = new CompanyProfile
                        {
                            UserId = user.Id,
                            CompanyName = model.FullName,
                            UpdatedAt = DateTime.UtcNow
                        };
                        _context.CompanyProfiles.Add(companyProfile);
                    }

                    await _context.SaveChangesAsync();

                    // Role Claim ekle
                    await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("Role", model.Role));

                    // Otomatik login
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    TempData["Success"] = "Kayıt başarılı! Hoş geldiniz.";

                    // Role'e göre yönlendir
                    if (model.Role == "Student")
                        return RedirectToAction("Dashboard", "Student");
                    else if (model.Role == "Company")
                        return RedirectToAction("Dashboard", "Company");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ActionName("Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginPost(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    
                    // Pasif kullanıcıları engelle
                    if (user != null && !user.IsActive)
                    {
                        await _signInManager.SignOutAsync();
                        ModelState.AddModelError(string.Empty, "Hesabınız pasif durumda. Lütfen yönetici ile iletişime geçin.");
                        return View(model);
                    }
                    
                    // Eğer kullanıcının role claim'i yoksa ekle (eski kullanıcılar için)
                    var claims = await _userManager.GetClaimsAsync(user!);
                    if (!claims.Any(c => c.Type == "Role"))
                    {
                        await _userManager.AddClaimAsync(user!, new System.Security.Claims.Claim("Role", user!.Role));
                    }
                    
                    TempData["Success"] = $"Hoş geldiniz, {user?.FullName}!";

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    // Role'e göre yönlendir
                    if (user?.Role == "Admin")
                        return RedirectToAction("Dashboard", "Admin");
                    else if (user?.Role == "Company")
                        return RedirectToAction("Dashboard", "Company");
                    else
                        return RedirectToAction("Dashboard", "Student");
                }

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Hesabınız kilitlendi. Lütfen daha sonra tekrar deneyin.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "E-posta veya şifre hatalı.");
                }
            }

            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["Success"] = "Başarıyla çıkış yaptınız.";
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}

