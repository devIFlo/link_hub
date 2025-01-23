using LinkHub.Models;
using LinkHub.Repositories;
using LinkHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LinkHub.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILinkRepository _linkRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IPageRepository _pageRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILinkRepository linkRepository, ICategoryRepository categoryRepository, IPageRepository pageRepository, UserManager<ApplicationUser> userManager)
        {
            _linkRepository = linkRepository;
            _categoryRepository = categoryRepository;
            _pageRepository = pageRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user?.Id;

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var pages = await _pageRepository.GetPagePerUserAsync(userId);

            return View(pages);
        }

        [AllowAnonymous]
        [HttpGet("/Home/Link/{page}")]
        public async Task<IActionResult> Link(string page)
        {
            var categories = await _categoryRepository.GetCategoriesPerPageAsync(page);
            if (categories.Count == 0) {
				return RedirectToAction("PageNotFound", "Home");
			}

            var links = await _linkRepository.GetLinksPerPageAsync(page);

            var homePageViewModel = new HomePageViewModel
            {
                Categories = categories,
                Links = links
            };

            ViewData["Page"] = page.ToUpper();

            return View(homePageViewModel);
        }

        [HttpGet]
        public IActionResult PageNotFound()
        {
            return View();
        }
    }
}