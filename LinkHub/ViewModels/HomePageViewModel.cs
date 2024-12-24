using LinkHub.Models;

namespace LinkHub.ViewModels
{
    public class HomePageViewModel
    {
        public IEnumerable<Link>? Links { get; set; }
        public IEnumerable<Category>? Categories { get; set; }
        public IEnumerable<Link>? HomePageLinks { get; set; }
    }
}