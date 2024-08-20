using LinkHub.Models;
using LinkHub.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkHub.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PagesController : Controller
    {
        private readonly IPageRepository _pageRepository;

        public PagesController(IPageRepository pageRepository)
        {
            _pageRepository = pageRepository;
        }

        public IActionResult Index()
        {
            var pages = _pageRepository.GetPages();
            return View(pages);
        }

        public IActionResult Create()
        {
            return PartialView("_Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Page page)
        {
            if (ModelState.IsValid)
            {
                await _pageRepository.Add(page);
                return RedirectToAction("Index");
            }

            return View();
        }

        public IActionResult Edit(int id)
        {
            Page page = _pageRepository.GetPage(id);
            if (page == null)
            {
                return NotFound();
            }

            return PartialView("_Edit", page);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Page page)
        {
            await _pageRepository.Update(page);

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            Page page = _pageRepository.GetPage(id);
            if(page == null)
            {
                return NotFound();
            }

            return PartialView("_Delete", page);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _pageRepository.Delete(id);
            return RedirectToAction("Index");
        }
    }
}