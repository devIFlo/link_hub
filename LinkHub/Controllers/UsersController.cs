using LinkHub.Models;
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

		public UsersController(UserManager<ApplicationUser> userManager,
			LdapSyncService ldapSyncService)
		{
			_userManager = userManager;
			_ldapSyncService = ldapSyncService;
		}

		[HttpGet]
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
