using LinkHub.Models;
using System.ComponentModel.DataAnnotations;

namespace LinkHub.ViewModels
{
    public class PermissionViewModel
    {
        public int PageId { get; set; }

        [Display(Name = "Página")]
        public string? PageName { get; set; }

        [Display(Name = "Usuários")]
        public List<string>? SelectedUserIds { get; set; }
        public IEnumerable<ApplicationUser>? Users { get; set; }
    }
}