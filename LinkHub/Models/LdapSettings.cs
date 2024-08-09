using System.ComponentModel.DataAnnotations;

namespace LinkHub.Models
{
    public class LdapSettings
    {
        public int Id { get; set; }
        public string Host { get; set; }

        [Display(Name = "Porta")]
        public int Port { get; set; }

        [Display(Name = "Dominio")]
        public string Domain { get; set; }
        public string BaseDn { get; set; }

        [Display(Name = "Usuário")]
        public string UserDn { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; }
    }
}
