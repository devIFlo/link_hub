using LinkHub.Models;
using LinkHub.Repositories;
using Microsoft.AspNetCore.Identity;
using System.DirectoryServices.Protocols;
using System.Net;

namespace LinkHub.Services
{
	public class LdapSyncService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ILogger<LdapSyncService> _logger;
		private readonly ILdapSettingsRepository _ldapSettingsRepository;

		public LdapSyncService(
			UserManager<ApplicationUser> userManager,
			IConfiguration configuration,
			ILogger<LdapSyncService> logger,
			ILdapSettingsRepository ldapSettingsRepository)
		{
			_userManager = userManager;
			_logger = logger;
			_ldapSettingsRepository = ldapSettingsRepository;
		}

		public async Task SyncUsersAsync()
		{
			var ldapUsers = GetLdapUsers();
			var identityUsers = _userManager.Users.ToList();

			foreach (var ldapUser in ldapUsers)
			{
				var identityUser = identityUsers.FirstOrDefault(u => u.UserName == ldapUser.UserName);

				if (identityUser == null)
				{
					var newUser = new ApplicationUser
                    {
						UserName = ldapUser.UserName,
						Email = ldapUser.Email,
						FirstName = ldapUser.FirstName,
						LastName = ldapUser.LastName
					};

					var result = await _userManager.CreateAsync(newUser);
					if (!result.Succeeded)
					{
						_logger.LogError("Failed to create new user {UserName}: {Errors}", ldapUser.UserName, string.Join(", ", result.Errors.Select(e => e.Description)));
					}
				}
				else
				{
					bool isUpdated = false;
					if (identityUser.Email != ldapUser.Email)
					{
						identityUser.Email = ldapUser.Email;
						isUpdated = true;
					}
					if (identityUser.FirstName != ldapUser.FirstName)
					{
						identityUser.FirstName = ldapUser.FirstName;
						isUpdated = true;
					}
					if (identityUser.LastName != ldapUser.LastName)
					{
						identityUser.LastName = ldapUser.LastName;
					    isUpdated = true;
					}

					if (isUpdated)
					{
						var result = await _userManager.UpdateAsync(identityUser);
						if (!result.Succeeded)
						{
							_logger.LogError("Failed to update user {UserName}: {Errors}", ldapUser.UserName, string.Join(", ", result.Errors.Select(e => e.Description)));
						}
					}
				}
			}
		}

		private List<LdapUser> GetLdapUsers()
		{
			var ldapUsers = new List<LdapUser>();
			var ldapSettings = _ldapSettingsRepository.GetLdapSettings();

            var host = ldapSettings.Host;
			var port = ldapSettings.Port;
			var baseDn = ldapSettings.BaseDn;
            var userDn = ldapSettings.UserDn;
			var password = ldapSettings.Password;

			var ldapConnection = new LdapConnection(new LdapDirectoryIdentifier(host, port));
			var networkCredential = new NetworkCredential(userDn, password);
			ldapConnection.Credential = networkCredential;
			ldapConnection.Bind();

			var filter = "(objectClass=person)";
			var searchRequest = new SearchRequest(baseDn, filter, SearchScope.Subtree, null);
			var response = (SearchResponse)ldapConnection.SendRequest(searchRequest);

			foreach (SearchResultEntry entry in response.Entries)
			{
				var ldapUser = new LdapUser
				{
					UserName = entry.Attributes["sAMAccountName"]?[0]?.ToString(),
					Email = entry.Attributes["mail"]?[0]?.ToString(),
					FirstName = entry.Attributes["givenName"]?[0]?.ToString(),
					LastName = entry.Attributes["sn"]?[0]?.ToString()
				};

				ldapUsers.Add(ldapUser);
			}

			return ldapUsers;
		}
	}
}