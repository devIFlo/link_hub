using LinkHub.Models;
using LinkHub.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LinkHub.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // GET: Categories
        public IActionResult Index()
        {
            List<Category> categories = _categoryRepository.GetCategories();
            return View(categories);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            await _categoryRepository.Add(category);
            return RedirectToAction("Index");
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
