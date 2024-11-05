using LinkHub.Repositories;
using LinkHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkHub.Controllers
{
	[Authorize]
	public class HomeController : Controller
    {
        private readonly ILinkRepository _linkRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IPageRepository _pageRepository;

        public HomeController(ILinkRepository linkRepository, ICategoryRepository categoryRepository, IPageRepository pageRepository)
        {
            _linkRepository = linkRepository;
            _categoryRepository = categoryRepository;
            _pageRepository = pageRepository;
        }

        public IActionResult Index()
        {
            var pages = _pageRepository.GetPages();
            return View(pages);
        }

        [AllowAnonymous]
        [HttpGet("/Home/Link/{page}")]
        public async Task<IActionResult> Link(string page)
        {
            var categories = await _categoryRepository.GetCategoriesPerPageAsync(page);
            var links = await _linkRepository.GetLinksPerPageAsync(page);

            var homePageViewModel = new HomePageViewModel
            {
                Categories = categories,
                Links = links
            };

            ViewData["Page"] = page;

            return View(homePageViewModel);
        }
    }
}