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

        public bool IsAuthenticated(string username, string password)
        {
            var ldapSettings = _settingsRepository.GetLdapSettings();

            try
            {
                using (LdapConnection ldapConnection = new LdapConnection(new LdapDirectoryIdentifier(ldapSettings.Host, ldapSettings.Port)))
                {
                    ldapConnection.Bind(new NetworkCredential(username, password, ldapSettings.Domain));
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