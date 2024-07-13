using LinkHub.Models;
using LinkHub.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LinkHub.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly LdapAuthentication _ldapService;

        public AccountController(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            LdapAuthentication ldapService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _ldapService = ldapService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
				return RedirectToAction("Index", "Home");
			}

			return View();
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
						return RedirectToAction("Index", "Home");
					}

                    ModelState.AddModelError(string.Empty, "Login Inválido");
					return View(model);
				}

                if (_ldapService.IsAuthenticated(model.Username, model.Password))
                {
                    var user = await _userManager.FindByNameAsync(model.Username);
                    
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("login.Invalid", "Usuário ou senha incorreto.");
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