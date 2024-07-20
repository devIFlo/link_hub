using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LinkHub.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Nome")]
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
