﻿using AspNetCoreHero.ToastNotification.Abstractions;
using LinkHub.Models;
using LinkHub.Services;
using LinkHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
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
        private readonly INotyfService _notyfService;

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            LdapAuthentication ldapService,
            INotyfService notyfService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _ldapService = ldapService;
            _notyfService = notyfService;
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
                    if (username == "admin")
                    {
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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null && (user.UserType != "LOCAL" || user.Id != id))
            {
                return Json(new { message = "Usuário incorreto!" });
            }

            var resetPasswordView = new ResetPasswordViewModel
            {
                UserId = id
            };

            return PartialView("_ResetPassword", resetPasswordView);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var currentPassword = model.CurrentPassword;
                var newPassword = model.NewPassword;

                if (currentPassword == null || newPassword == null)
                {
                    _notyfService.Error("Preencha todos os campos obrigarórios.");
                    return RedirectToAction("Profile");
                }

                var passwordValid = await _userManager.CheckPasswordAsync(user, currentPassword);
                if (passwordValid)
                {
                    if (model.NewPassword == model.ConfirmPassword)
                    {
                        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
                        if (result.Succeeded)
                        {
                            await _signInManager.RefreshSignInAsync(user);
                            _notyfService.Success("Senha alterada com sucesso.");
                        }

                        foreach (var error in result.Errors)
                        {
                            _notyfService.Error(error.Description);
                        }
                    }
                    else
                    {
                        _notyfService.Error("A nova senha e a confirmação da senha não coincidem.");
                    }
                }
                else
                {
                    _notyfService.Error("A senha atual está incorreta.");
                }

                return RedirectToAction("Profile");
            }

            _notyfService.Error("Usuário não encontrado.");

            return RedirectToAction("Profile");
        }
    }
}