using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace LinkHub.ViewModels
{
    public class RoleViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }

        [Display(Name = "Grupo")]
        public string SelectedRole { get; set; }

        public IEnumerable<IdentityRole>? Roles { get; set; }
    }
}
