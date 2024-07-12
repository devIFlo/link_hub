using System.DirectoryServices.Protocols;
using System.Net;

namespace LinkHub.Services
{
    public class LdapAuthentication
    {
        private readonly string _ldapHost;
        private readonly int _ldapPort;
        private readonly string _ldapDomain;

        public LdapAuthentication(string ldapHost, int ldapPort, string ldapDomain)
        {
            _ldapHost = ldapHost;
            _ldapPort = ldapPort;
            _ldapDomain = ldapDomain;
        }

        public bool IsAuthenticated(string username, string password)
        {
            try
            {
                using (LdapConnection ldapConnection = new LdapConnection(new LdapDirectoryIdentifier(_ldapHost, _ldapPort)))
                {
                    ldapConnection.Bind(new NetworkCredential(username, password, _ldapDomain));
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