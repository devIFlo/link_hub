using LinkHub.Models;

namespace LinkHub.Repositories
{
    public interface ILdapSettingsRepository
    {
        Task<LdapSettings?> GetLdapSettings();
        Task<LdapSettings> Add(LdapSettings ldapSettings);
        Task<LdapSettings> Update(LdapSettings ldapSettings);
    }
}
