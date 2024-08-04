using LinkHub.Models;
using LinkHub.Repositories;
using LinkHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LinkHub.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IPageRepository _pageRepository;

        public CategoriesController(ICategoryRepository categoryRepository, IPageRepository pageRepository)
        {
            _categoryRepository = categoryRepository;
            _pageRepository = pageRepository;
        }

        // GET: Categories
        public IActionResult Index()
        {
            var categories = _categoryRepository.GetCategories();
            return View(categories);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            var pages = _pageRepository.GetPages();
            var categoryView = new CategoryViewModel
            {
                Pages = pages
            };

            return View(categoryView);
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel categoryView)
        {
            if (ModelState.IsValid)
            {
                var category = new Category
                {
                    Name = categoryView.Name,
                    PageId = categoryView.SelectedPageId
                };

                await _categoryRepository.Add(category);
                return RedirectToAction("Index");
            }

            categoryView.Pages = _pageRepository.GetPages();
            return View(categoryView);
        }

        // GET: Categories/Edit
        public IActionResult Edit(int id)
        {
            Category category = _categoryRepository.GetCategory(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            await _categoryRepository.Update(category);

            return RedirectToAction("Index");
        }

        // GET: Categories/Delete
        public IActionResult Delete(int id)
        {
            Category category = _categoryRepository.GetCategory(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _categoryRepository.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
