using LinkHub.Models;
using System.ComponentModel.DataAnnotations;

namespace LinkHub.ViewModels
{
    public class LinkHomeViewModel
    {
        public int LinkId { get; set; }

        [Display(Name = "Link")]
        public string? LinkName { get; set; }

        [Display(Name = "Páginas")]
        public List<int>? SelectedPageIds { get; set; }
        public IEnumerable<Page>? Pages { get; set; }
    }
}