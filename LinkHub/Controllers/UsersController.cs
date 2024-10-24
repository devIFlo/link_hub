﻿using LinkHub.Models;
using LinkHub.ViewModels;
using LinkHub.Repositories;
using LinkHub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNetCoreHero.ToastNotification.Abstractions;
using System.DirectoryServices.Protocols;


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
			try
			{
				await _ldapSyncService.SyncUsersAsync();
				_notifyService.Success("Usuários sincronizados com sucesso!");
			}
			catch (InvalidOperationException ex)
			{
				_notifyService.Warning(ex.Message);
			}
			catch (LdapException ex)
			{
				_notifyService.Error(ex.Message);
			}
			catch (Exception)
			{
				_notifyService.Error("Ocorreu um erro inesperado. Procure o administrador do sistema.");
			}

            return RedirectToAction("Index");
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
					_notifyService.Success("Configurações LDAP salvas com sucesso.");
                } 
				else
				{
					await _ldapSettingsRepository.Update(ldapSettings);
                    _notifyService.Success("Configurações LDAP atualizadas com sucesso.");
                }

                return RedirectToAction("Index");
            }

			_notifyService.Warning("Preencha todos os campos obrigatórios.");
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
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByIdAsync(model.UserId);

				if (user != null)
				{
					var userRoles = await _userManager.GetRolesAsync(user);

					if (userRoles.Any())
					{
						var removeRoleResult = await _userManager.RemoveFromRolesAsync(user, userRoles);
						if (!removeRoleResult.Succeeded)
						{
							_notifyService.Error("Erro ao remover os grupos existentes.");
							return View(model);
						}
					}

					var addRoleResult = await _userManager.AddToRoleAsync(user, model.SelectedRole);
					if (!addRoleResult.Succeeded)
					{
                        _notifyService.Error("Erro ao adicionar o novo grupo.");
						return View(model);
					}

					_notifyService.Success("Grupo alterado com sucesso!");

					return RedirectToAction("Index");
				}
			}

            _notifyService.Error("Usuário não encontrado.");

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
                _notifyService.Error($"Usuário com Id = {id} não foi encontrado");
            }
			else
			{
				var result = await _userManager.DeleteAsync(user);

				if (result.Succeeded)
				{
					_notifyService.Success($"Usuário {user.UserName} excluído com sucesso.");
					
				}

				foreach (var error in result.Errors)
				{
					_notifyService.Error(error.Description);
				}
			}

            return RedirectToAction("Index");
        }
	}
}