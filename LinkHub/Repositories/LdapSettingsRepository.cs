using LinkHub.Data;
using LinkHub.Models;
using Microsoft.AspNetCore.DataProtection;

namespace LinkHub.Repositories
{
    public class LdapSettingsRepository : ILdapSettingsRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly IDataProtector _protector;

        public LdapSettingsRepository(ApplicationDbContext context, IDataProtectionProvider dataProtectionProvider)
        {
            _context = context;
            _dataProtectionProvider = dataProtectionProvider;
            _protector = _dataProtectionProvider.CreateProtector("LdapSettingsPasswordProtector");
        }

        public LdapSettings GetLdapSettings()
        {
            return _context.LdapSettings.FirstOrDefault();
        }

        public async Task<LdapSettings> Add(LdapSettings ldapSettings)
        {
            ldapSettings.EncryptPassword(_protector);

            _context.LdapSettings.Add(ldapSettings);
            await _context.SaveChangesAsync();

            return ldapSettings;
        }     

        public async Task<LdapSettings> Update(LdapSettings ldapSettings)
        {
            LdapSettings ldapSettingsDB = GetLdapSettings();

            ldapSettingsDB.FqdnDomain = ldapSettings.FqdnDomain;
            ldapSettingsDB.Port = ldapSettings.Port;
            ldapSettingsDB.BaseDn = ldapSettings.BaseDn;
            ldapSettingsDB.NetBiosDomain = ldapSettings.NetBiosDomain;
            ldapSettingsDB.UserDn = ldapSettings.UserDn;
            ldapSettingsDB.Password = ldapSettings.Password;

            ldapSettingsDB.EncryptPassword(_protector);

            _context.LdapSettings.Update(ldapSettingsDB);
            await _context.SaveChangesAsync();

            return ldapSettingsDB;
        }
    }
}
