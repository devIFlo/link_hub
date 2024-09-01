using LinkHub.Models;
using LinkHub.Repositories;
using LinkHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LinkHub.Controllers
{
    [Authorize(Roles = "Admin, Editor")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IPageRepository _pageRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CategoriesController(ICategoryRepository categoryRepository,
            IPageRepository pageRepository,
            UserManager<ApplicationUser> userManager)
        {
            _categoryRepository = categoryRepository;
            _pageRepository = pageRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user?.Id;

            var categories = await _categoryRepository.GetCategoriesPerUserAsync(userId);
            return View(categories);
        }

        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user?.Id;

            var pages = await _pageRepository.GetPagePerUserAsync(userId);
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

                await _categoryRepository.AddAsync(category);
                return RedirectToAction("Index");
            }

            categoryView.Pages = _pageRepository.GetPages();
            return View(categoryView);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryRepository.GetCategoryAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var userId = user?.Id;

            var pages = await _pageRepository.GetPagePerUserAsync(userId);
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
            await _categoryRepository.UpdateAsync(category);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            Category category = await _categoryRepository.GetCategoryAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return PartialView("_Delete", category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _categoryRepository.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
