using LinkHub.Models;
using LinkHub.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
        public IActionResult Link(string page)
        {
            ViewBag.Categories = _categoryRepository.GetCategories().Where(c => c.Page.Name == page);
            ViewData["Page"] = page;

            var links = _linkRepository.GetLinks().Where(l => l.Category.Page.Name == page);
            return View(links);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}