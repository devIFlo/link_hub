using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinkHub.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Display(Name = "Nome")]
        public string? Name { get; set; }

        public int PageId { get; set; }

        [ForeignKey("PageId")]
        public Page Page { get; set; }

        public ICollection<Link> Services { get; set; } = new List<Link>();
    }
}