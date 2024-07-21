using System.ComponentModel.DataAnnotations;

namespace LinkHub.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Display(Name = "Nome")]
        public string? Name { get; set; }

        public ICollection<Link> Services { get; set; } = new List<Link>();
    }
}
