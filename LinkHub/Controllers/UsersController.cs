using LinkHub.Models;
using LinkHub.ViewModels;
using LinkHub.Repositories;
using LinkHub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNetCoreHero.ToastNotification.Abstractions;


namespace LinkHub.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly LdapSyncService _ldapSyncService;
		private readonly ILdapSettingsRepository _ldapSettingsRepository;
        private readonly INotyfService _notifyService;


        public UsersController(UserManager<ApplicationUser> userManager,
			RoleManager<IdentityRole> roleManager,
			LdapSyncService ldapSyncService, 
			ILdapSettingsRepository ldapSettingsRepository,
            INotyfService notifyService)
		{
			_userManager = userManager;
			_ldapSyncService = ldapSyncService;
			_ldapSettingsRepository = ldapSettingsRepository;
			_roleManager = roleManager;
            _notifyService = notifyService;
        }

		public IActionResult Index()
		{
            var users = _userManager.Users;
			return View(users);
		}

		[HttpPost]
		public async Task<IActionResult> SyncLdapUsers()
		{
            var users = _userManager.Users;

            try
			{
				await _ldapSyncService.SyncUsersAsync();
				return RedirectToAction("Index", "Users");
			}
            catch (InvalidOperationException ex)
			{
                _notifyService.Error(ex.Message);
                return View("Index", users);
			}
			catch (Exception)
            {
                _notifyService.Error("Ocorreu um erro inesperado. Procure o administrador do sistema.");
                return View("Index", users);
            }
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

		public async Task<IActionResult> Group(string id)
		{
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
			{
				return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var roles = await _roleManager.Roles.ToListAsync();

            var selectedRole = roles.FirstOrDefault(r => r.Name != null && userRoles.Contains(r.Name))?.Name;

			var rolesView = new RoleViewModel
			{
				UserId = id,
				UserName = user.UserName,
				Roles = roles,
				SelectedRole = selectedRole
            };

            return PartialView("_Group", rolesView);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Group(RoleViewModel model)
		{
			if (!ModelState.IsValid)
			{
				var user = await _userManager.FindByIdAsync(model.UserId);

				if (user != null)
				{
					var userRoles = await _userManager.GetRolesAsync(user);

					if (userRoles.Any())
					{
						var remevoRoleResult = await _userManager.RemoveFromRolesAsync(user, userRoles);
						if (!remevoRoleResult.Succeeded)
						{
							ModelState.AddModelError("", "Erro ao remover os grupos existentes.");
							return View(model);
						}
					}

					var addRoleResult = await _userManager.AddToRoleAsync(user, model.SelectedRole);
					if (!addRoleResult.Succeeded)
					{
						ModelState.AddModelError("", "Erro ao adicionar o novo grupo.");
						return View(model);
					}

					return RedirectToAction("Index");
				}
			}

            ModelState.AddModelError("", "Usuário não encontrado.");

            return RedirectToAction("Index");
        }

		public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return PartialView("_Delete", user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
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