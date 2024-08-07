using LinkHub.Models;
using LinkHub.Repositories;
using LinkHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
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

        public IActionResult Index()
        {
            var categories = _categoryRepository.GetCategories();
            return View(categories);
        }

        public IActionResult Create()
        {
            var pages = _pageRepository.GetPages();
            var categoryView = new CategoryViewModel
            {
                Pages = pages
            };

            return PartialView("_Create", categoryView);
        }

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

        public IActionResult Edit(int id)
        {
            var category = _categoryRepository.GetCategory(id);
            if (category == null)
            {
                return NotFound();
            }

            var pages = _pageRepository.GetPages();
            var categoryView = new CategoryViewModel
            {
                Name = category.Name,
                Pages = pages,
                SelectedPageId = category.PageId
            };

            return PartialView("_Edit", categoryView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            await _categoryRepository.Update(category);

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            Category category = _categoryRepository.GetCategory(id);
            if (category == null)
            {
                return NotFound();
            }

            return PartialView("_Delete", category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _categoryRepository.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
