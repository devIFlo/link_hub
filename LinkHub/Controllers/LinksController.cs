using LinkHub.Models;
using LinkHub.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LinkHub.Controllers
{
    public class LinksController : Controller
    {
        private readonly ILinkRepository _linkRepository;

        public LinksController(ILinkRepository linkRepository)
        {
            _linkRepository = linkRepository;
        }

        // GET: Services
        public IActionResult Index()
        {
            List<Link> links = _linkRepository.GetLinks();
            return View(links);
        }

        // GET: Services/Create
        public IActionResult Create()
        {
            ViewBag.Categories = _linkRepository.GetCategories();

            return PartialView("_Create");
        }

        // POST: Services/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Link service)
        {
            await _linkRepository.Add(service);

            return RedirectToAction("Index");
        }

        // GET: Services/Edit/5
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

        // POST: Services/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Link link)
        {
            await _linkRepository.Update(link);

            return RedirectToAction("Index");
        }

        // GET: Services/Delete/5
        public IActionResult Delete(int id)
        {
            Link link = _linkRepository.GetLink(id);
            if (link == null)
            {
                return NotFound();
            }

            return PartialView("_Delete", link);
        }

        // POST: Services/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _linkRepository.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
