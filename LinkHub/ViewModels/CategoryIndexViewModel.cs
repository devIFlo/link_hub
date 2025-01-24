using LinkHub.Models;
using System.ComponentModel.DataAnnotations;

namespace LinkHub.ViewModels
{
    public class CategoryIndexViewModel
    {
        [Display(Name = "Categoria")]
        public string? Name { get; set; }

        public List<Category>? Categories { get; set; }

        public List<Page>? Pages { get; set; }
    }
}