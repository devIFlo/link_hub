using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LinkHub.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Usuário")]
        public override string UserName { get; set; }

        [Display(Name = "Nome")]
        public string? FirstName { get; set; }

        public string? LastName { get; set; }
    }
}
