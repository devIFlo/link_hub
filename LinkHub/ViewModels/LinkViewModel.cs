using LinkHub.Models;
using System.ComponentModel.DataAnnotations;

namespace LinkHub.ViewModels
{
    public class LinkViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [Display(Name = "Nome")]
        public string Name { get; set; } = null!;

        [Display(Name = "Descrição")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "O link é obrigatório.")]
        [Display(Name = "Link")]
        public string Url { get; set; } = null!;

        [Display(Name = "Imagem")]
        public IFormFile? Image { get; set; }
        
        public string? FileName { get; set; }

        [Required(ErrorMessage = "Selecione uma categoria.")]
        [Display(Name = "Categoria")]
        public int CategoryId { get; set; }

        public IEnumerable<Category>? Categories { get; set; }

        [Required(ErrorMessage = "Selecione uma página.")]
        [Display(Name = "Página")]
        public int PageId { get; set; }

        public IEnumerable<Page>? Pages { get; set; }
    }
}