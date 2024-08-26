using LinkHub.Models;
using System.ComponentModel.DataAnnotations;

namespace LinkHub.ViewModels
{
    public class PageViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [Display(Name = "Nome")]
        public string Name { get; set; }

        [Display(Name = "Descrição")]
        public string Description { get; set; }

        [Display(Name = "Usuários")]
        public int SelectedUsersIds { get; set; }
        public IEnumerable<ApplicationUser>? Users { get; set; }
    }
}
