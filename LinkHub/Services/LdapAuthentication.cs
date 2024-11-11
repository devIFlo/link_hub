using LinkHub.Repositories;
using System.DirectoryServices.Protocols;
using System.Net;

namespace LinkHub.Services
{
    public class LdapAuthentication
    {
        private readonly ILdapSettingsRepository _settingsRepository;

        public LdapAuthentication(ILdapSettingsRepository settingsRepository)
        {
            _settingsRepository = settingsRepository;
        }

        public async Task<bool> IsAuthenticated(string username, string password)
        {
            var ldapSettings = await _settingsRepository.GetLdapSettings();

            if (ldapSettings == null) return false;

            try
            {
                using (LdapConnection ldapConnection = new LdapConnection(new LdapDirectoryIdentifier(ldapSettings.FqdnDomain, ldapSettings.Port)))
                {
					ldapConnection.AuthType = AuthType.Basic;
					ldapConnection.SessionOptions.ProtocolVersion = 3;
					ldapConnection.SessionOptions.ReferralChasing = ReferralChasingOptions.None;
					ldapConnection.Timeout = TimeSpan.FromMinutes(1);

					ldapConnection.Bind(new NetworkCredential(username, password, ldapSettings.NetBiosDomain));

                    return true;
                }
            }
            catch (LdapException)
            {
                return false;
            }
        }
    }
}