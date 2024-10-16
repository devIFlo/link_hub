using System.ComponentModel.DataAnnotations;

namespace LinkHub.Models
{
    public class Page
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [Display(Name = "Nome")]
        public required string Name { get; set; }

        [Display(Name = "Descrição")]
        public string? Description { get; set; }
    }
}
