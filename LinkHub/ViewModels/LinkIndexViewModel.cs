using LinkHub.Models;
using System.ComponentModel.DataAnnotations;

namespace LinkHub.ViewModels
{
    public class LinkIndexViewModel
    {
        [Display(Name = "Nome")]
        public string? Name { get; set; }

        [Display(Name = "Link")]
        public string? Url { get; set; }

        [Display(Name = "Categoria")]
        public Category? Category { get; set; }

        public List<Link>? Links { get; set; }

        public List<Page>? Pages { get; set; }
    }
}