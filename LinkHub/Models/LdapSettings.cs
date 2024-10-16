using Microsoft.AspNetCore.DataProtection;
using System.ComponentModel.DataAnnotations;

namespace LinkHub.Models
{
    public class LdapSettings
    {
        public int Id { get; set; }

		[Display(Name = "Dominio FQDN")]
        [Required]
		public required string FqdnDomain { get; set; }

        [Display(Name = "Porta")]
        [Required]
        public required int Port { get; set; }

        [Display(Name = "Dominio NetBios")]
        [Required]
        public required string NetBiosDomain { get; set; }

		public required string BaseDn { get; set; }

        [Display(Name = "Usuário")]
        [Required]
        public required string UserDn { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        [Required]
        public required string Password { get; set; }

        public void EncryptPassword(IDataProtector protector)
        {
            Password = protector.Protect(Password);
        }

        public string DecryptPassword(IDataProtector protector)
        {
            return protector.Unprotect(Password);
        }
    }
}
