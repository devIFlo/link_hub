using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinkHub.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Usuário")]
        public override string? UserName { get; set; }

        [Display(Name = "Nome")]
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        [NotMapped]
        public string? DisplayUser => $"{UserName} ({FirstName} {LastName})";
    }
}
