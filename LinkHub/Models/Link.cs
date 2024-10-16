using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LinkHub.Models
{
    public class Link
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

        [Required]
        public required Category Category { get; set; }

        [Required(ErrorMessage = "A categoria é obrigatória.")]
        [Display(Name = "Categoria")]
        public int CategoryId { get; set; }

        public string FileName { get; set; } = null!;

        [Required(ErrorMessage = "A imagem é obrigatória.")]
        [NotMapped]
        [Display(Name = "Imagem")]
        public IFormFile? Image { get; set; }
    }
}
