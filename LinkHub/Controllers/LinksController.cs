using LinkHub.Models;
using LinkHub.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkHub.Controllers
{
    [Authorize(Roles = "Admin, Editor")]
    public class LinksController : Controller
    {
        private readonly ILinkRepository _linkRepository;

        public LinksController(ILinkRepository linkRepository)
        {
            _linkRepository = linkRepository;
        }

        public IActionResult Index()
        {
            List<Link> links = _linkRepository.GetLinks();
            return View(links);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = _linkRepository.GetCategories();

            return PartialView("_Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Link service)
        {
            await _linkRepository.Add(service);

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            ViewBag.Categories = _linkRepository.GetCategories();

            Link link = _linkRepository.GetLink(id);
            if (link == null)
            {
                return NotFound();
            }

            return PartialView("_Edit", link);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Link link)
        {
            await _linkRepository.Update(link);

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            Link link = _linkRepository.GetLink(id);
            if (link == null)
            {
                return NotFound();
            }

            return PartialView("_Delete", link);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _linkRepository.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
