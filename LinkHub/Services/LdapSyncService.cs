using LinkHub.Models;
using LinkHub.Repositories;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using System.DirectoryServices.Protocols;
using System.Net;
using Serilog;

namespace LinkHub.Services
{
    public class LdapSyncService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ILdapSettingsRepository _ldapSettingsRepository;
        private readonly IDataProtector _protector;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LdapSyncService(
			UserManager<ApplicationUser> userManager,
			ILdapSettingsRepository ldapSettingsRepository,
            IDataProtectionProvider dataProtectionProvider,
            IHttpContextAccessor httpContextAccessor)
		{
			_userManager = userManager;
			_ldapSettingsRepository = ldapSettingsRepository;
            _protector = dataProtectionProvider.CreateProtector("LdapSettingsPasswordProtector");
            _httpContextAccessor = httpContextAccessor;
        }
			
        public async Task SyncUsersAsync()
		{
			var ldapUsers = await GetLdapUsers();
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
					if (result.Succeeded)
					{
						await _userManager.AddToRoleAsync(newUser, "VIEWER");
                    }
					else
					{
						Log.Error("{Timestamp} - Falha ao criar o usuário {UserName}: {Errors}", 
							DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ldapUser.UserName, string.Join(", ", result.Errors.Select(e => e.Description)));
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
                            Log.Error("{Timestamp} - Falha ao atualizar o usuário {UserName}: {Errors}",
								DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ldapUser.UserName, string.Join(", ", result.Errors.Select(e => e.Description)));
                        }
					}
				}
			}
		}

		private async Task<List<LdapSyncModel>> GetLdapUsers()
		{
			var ldapUsers = new List<LdapSyncModel>();
			var ldapSettings = await _ldapSettingsRepository.GetLdapSettings();

            if (ldapSettings == null) throw new InvalidOperationException("Configurações LDAP não encontradas.");

            var fqdnDomain = ldapSettings.FqdnDomain;
            var port = ldapSettings.Port;
            var netBiosDomain = ldapSettings.NetBiosDomain;
            var baseDn = ldapSettings.BaseDn;
            var userDn = ldapSettings.UserDn;
            var password = ldapSettings.DecryptPassword(_protector);

            var ldapConnection = new LdapConnection(
                    new LdapDirectoryIdentifier(fqdnDomain, port),
                    new NetworkCredential(userDn, password, netBiosDomain),
                    AuthType.Basic);

            ldapConnection.SessionOptions.ProtocolVersion = 3;
            ldapConnection.SessionOptions.ReferralChasing = ReferralChasingOptions.None;
            ldapConnection.Timeout = TimeSpan.FromMinutes(1);
            
			try
            {
                ldapConnection.Bind();
            }
            catch (LdapException ex)
            {
                throw new LdapException("Falha ao conectar ao servidor LDAP. Verifique as configurações da conexão.", ex);
            }

            var filter = "(objectClass=person)";
            var searchRequest = new SearchRequest(baseDn, filter, SearchScope.Subtree, null);
            var response = (SearchResponse)ldapConnection.SendRequest(searchRequest);

            foreach (SearchResultEntry entry in response.Entries)
            {
                var ldapUser = new LdapSyncModel
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