using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LinkHub.Models
{
    public class Link
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Nome")]
        public string Name { get; set; } = null!;

        [Display(Name = "Descrição")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public string Url { get; set; } = null!;

        public Category? Category { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Categoria")]
        public int CategoryId { get; set; }

        public string FileName { get; set; } = null!;

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [NotMapped]
        [Display(Name = "Imagem")]
        public IFormFile? Image { get; set; }
    }
}
