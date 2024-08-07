using LinkHub.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LinkHub.ViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [Display(Name = "Nome")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Selecione um página.")]
        [Display(Name = "Página")]
        public int SelectedPageId { get; set; }

        public IEnumerable<Page>? Pages { get; set; }
    }
}