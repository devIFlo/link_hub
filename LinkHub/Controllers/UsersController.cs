using LinkHub.Models;
using LinkHub.Repositories;
using LinkHub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace LinkHub.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly LdapSyncService _ldapSyncService;
		private readonly ILdapSettingsRepository _ldapSettingsRepository;

		public UsersController(UserManager<ApplicationUser> userManager,
			LdapSyncService ldapSyncService, ILdapSettingsRepository ldapSettingsRepository)
		{
			_userManager = userManager;
			_ldapSyncService = ldapSyncService;
			_ldapSettingsRepository = ldapSettingsRepository;
		}

		public IActionResult Index()
		{
			var users = _userManager.Users;
			return View(users);
		}

		[HttpPost]
		public async Task<IActionResult> SyncLdapUsers()
		{
			await _ldapSyncService.SyncUsersAsync();
			return RedirectToAction("Index", "Users");
		}

		public IActionResult Settings()
		{
			LdapSettings ldapSettings = _ldapSettingsRepository.GetLdapSettings();
			if (ldapSettings == null)
			{
                return PartialView("_Settings");
            }

			return PartialView("_Settings", ldapSettings);			
		}

		[HttpPost]
		public async Task<IActionResult> Settings(LdapSettings ldapSettings)
		{
			if (ModelState.IsValid)
			{
                LdapSettings ldapSettingsDB = _ldapSettingsRepository.GetLdapSettings();
				if (ldapSettingsDB == null)
				{
                    await _ldapSettingsRepository.Add(ldapSettings);
                    return RedirectToAction("Index");
                }

				await _ldapSettingsRepository.Update(ldapSettings);
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
		public async Task<IActionResult> DeleteUser(string id)
		{
			var user = await _userManager.FindByIdAsync(id);

			if (user == null)
			{
				ViewBag.ErrorMessage = $"Usuário com Id = {id} não foi encontrado";
				return View("NotFound");
			}
			else
			{
				var result = await _userManager.DeleteAsync(user);

				if (result.Succeeded)
				{
					return RedirectToAction("Index");
				}

				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}

				return View("Index");
			}
		}
	}
}