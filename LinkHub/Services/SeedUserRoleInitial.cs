using Microsoft.AspNetCore.Identity;

namespace LinkHub.Services
{
	public class SeedUserRoleInitial : ISeedUserRoleInitial
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		public SeedUserRoleInitial(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			_userManager = userManager;
			_roleManager = roleManager;
		}

		public async Task SeedRolesAsync()
		{
			if(! await _roleManager.RoleExistsAsync("Admin"))
			{
				IdentityRole role = new IdentityRole();
				role.Name = "Admin";
				role.NormalizedName = "ADMIN";
				role.ConcurrencyStamp = Guid.NewGuid().ToString();

				IdentityResult roleResult = await _roleManager.CreateAsync(role);
			}
		}

		public async Task SeedUsersAsync()
		{
			if (await _userManager.FindByNameAsync("admin") == null)
			{
				IdentityUser user = new IdentityUser();
				user.UserName = "admin";
				user.NormalizedUserName = "ADMIN";
				user.LockoutEnabled = false;
				user.SecurityStamp = Guid.NewGuid().ToString();

				IdentityResult result = await _userManager.CreateAsync(user, "Admin@2024");

				if (result.Succeeded)
				{
					await _userManager.AddToRoleAsync(user, "Admin");
				}
			}
		}
	}
}
