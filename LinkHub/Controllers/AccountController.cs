using LinkHub.Models;
using LinkHub.Services;
using LinkHub.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LinkHub.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly LdapAuthentication _ldapService;

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            LdapAuthentication ldapService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _ldapService = ldapService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
				return RedirectToAction("Index", "Home");
			}

            var model = new LoginViewModel();

            var rememberedUsername = Request.Cookies["RememberedUsername"];
            if (!string.IsNullOrEmpty(rememberedUsername))
            {
                model.Username = rememberedUsername;
            }

            return View(model);
        }

		[HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Username == "admin") {
					var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);

                    if (result.Succeeded)
                    {
                        if (model.RememberMe)
                        {
                            Response.Cookies.Append("RememberedUsername", model.Username, new CookieOptions
                            {
                                Expires = DateTime.UtcNow.AddDays(30),
                                HttpOnly = true,
                                SameSite = SameSiteMode.Lax
                            });
                        }
                        else
                        {
                            Response.Cookies.Delete("RememberedUsername");
                        }

                        return RedirectToAction("Index", "Home");
					}

                    ModelState.AddModelError(string.Empty, "Usuário ou senha incorreto.");
					return View(model);
				}

                if (_ldapService.IsAuthenticated(model.Username, model.Password))
                {
                    var user = await _userManager.FindByNameAsync(model.Username);
                    if (user == null)
                    {
                        ModelState.AddModelError(string.Empty, "Usuário não tem permissão para acessar o sistema.");
                        return View(model);
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);

                    if (model.RememberMe)
                    {
                        Response.Cookies.Append("RememberedUsername", model.Username, new CookieOptions
                        {
                            Expires = DateTime.UtcNow.AddDays(30),
                            HttpOnly = true,
                            SameSite = SameSiteMode.Lax
                        });
                    }
                    else
                    {
                        Response.Cookies.Delete("RememberedUsername");
                    }

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Usuário ou senha incorreto.");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}