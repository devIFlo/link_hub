using System.ComponentModel.DataAnnotations;

namespace LinkHub.Models
{
    public class Link
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [Display(Name = "Nome")]
        public required string Name { get; set; }

        [Display(Name = "Descrição")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "O link é obrigatório.")]
        [Display(Name = "Link")]
        public required string Url { get; set; }

        public Category? Category { get; set; }

        [Required(ErrorMessage = "A categoria é obrigatória.")]
        [Display(Name = "Categoria")]
        public int CategoryId { get; set; }

        public required string FileName { get; set; }
    }
}
