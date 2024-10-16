using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LinkHub.ViewModels
{
    public class RoleViewModel
    {      
        [Required]
        public required string UserId { get; set; }

        [Required]
        public required string UserName { get; set; }

        [Display(Name = "Grupo")]
        [Required]
        public required string SelectedRole { get; set; }

        public IEnumerable<IdentityRole>? Roles { get; set; }
    }
}
