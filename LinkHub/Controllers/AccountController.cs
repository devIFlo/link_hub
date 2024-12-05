using LinkHub.Models;
using LinkHub.Services;
using LinkHub.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;

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
                var username = model.Username;
                var password = model.Password;
                var rememberMe = model.RememberMe;

                if (username != null && password != null)
                {
                    if (username == "admin") {
					    var result = await _signInManager.PasswordSignInAsync(username, password, rememberMe, false);

                        if (result.Succeeded)
                        {
                            if (rememberMe)
                            {
                                Response.Cookies.Append("RememberedUsername", username, new CookieOptions
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

                            Log.Information("O usuário {User} realizou o login no sistema em {Timestamp}",
                                username, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

                            return RedirectToAction("Index", "Home");
					    }

                        ModelState.AddModelError(string.Empty, "Usuário ou senha incorreto.");

                        Log.Warning("Tentativa de login mal sucedida com o usuário {User} em {Timestamp}",
                                username, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

                        return View(model);
				    }

                    if (await _ldapService.IsAuthenticated(username, password))
                    {
                        var user = await _userManager.FindByNameAsync(username);
                        if (user == null)
                        {
                            ModelState.AddModelError(string.Empty, "Usuário não tem permissão para acessar o sistema.");
                            return View(model);
                        }

                        await _signInManager.SignInAsync(user, isPersistent: false);

                        if (model.RememberMe)
                        {
                            Response.Cookies.Append("RememberedUsername", username, new CookieOptions
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

                        Log.Information("O usuário {User} realizou o login no sistema em {Timestamp}",
                                username, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

                        return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError(string.Empty, "Usuário ou senha incorreto.");

                Log.Warning("Tentativa de login mal sucedida com o usuário {User} em {Timestamp}",
                                username, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
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