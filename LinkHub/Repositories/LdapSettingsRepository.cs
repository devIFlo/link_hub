using LinkHub.Data;
using LinkHub.Models;

namespace LinkHub.Repositories
{
    public class LdapSettingsRepository : ILdapSettingsRepository
    {
        private readonly ApplicationDbContext _context;

        public LdapSettingsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public LdapSettings GetLdapSettings()
        {
            return _context.LdapSettings.FirstOrDefault();
        }

        public async Task<LdapSettings> Add(LdapSettings ldapSettings)
        {
            _context.LdapSettings.Add(ldapSettings);
            await _context.SaveChangesAsync();

            return ldapSettings;
        }     

        public async Task<LdapSettings> Update(LdapSettings ldapSettings)
        {
            LdapSettings ldapSettingsDB = GetLdapSettings();

            ldapSettingsDB.Host = ldapSettings.Host;
            ldapSettingsDB.Port = ldapSettings.Port;
            ldapSettingsDB.BaseDn = ldapSettings.BaseDn;
            ldapSettingsDB.Domain = ldapSettings.Domain;
            ldapSettingsDB.UserDn = ldapSettings.UserDn;
            ldapSettingsDB.Password = ldapSettings.Password;

            _context.LdapSettings.Update(ldapSettingsDB);
            await _context.SaveChangesAsync();

            return ldapSettingsDB;
        }
    }
}
