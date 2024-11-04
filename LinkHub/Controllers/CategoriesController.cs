using AspNetCoreHero.ToastNotification.Abstractions;
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
        private readonly INotyfService _notyfService;

        public CategoriesController(ICategoryRepository categoryRepository,
            IPageRepository pageRepository,
            UserManager<ApplicationUser> userManager,
            INotyfService notyfService)
        {
            _categoryRepository = categoryRepository;
            _pageRepository = pageRepository;
            _userManager = userManager;
            _notyfService = notyfService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user?.Id;

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var categories = await _categoryRepository.GetCategoriesPerUserAsync(userId);
            return View(categories);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user?.Id;

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

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
            if (!ModelState.IsValid)
            {
                _notyfService.Warning("Preencha todos os campos obrigatórios.");
                return RedirectToAction("Index");
            }

            try
            {
                var category = new Category
                {
                    Name = categoryView.Name,
                    PageId = categoryView.SelectedPageId
                };

                await _categoryRepository.AddAsync(category);
                _notyfService.Success("Categoria adicionada com sucesso.");
            }
            catch (Exception ex)
            {
                _notyfService.Error("Ocorreu um erro ao adicionar a categoria: " + ex.Message);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryRepository.GetCategoryAsync(id);
            if (category == null)
            {
                return Json(new { message = "Categoria não encontrada!" });
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
            if (!ModelState.IsValid)
            {
                _notyfService.Warning("Preencha todos os campos obrigatórios.");
                return RedirectToAction("Index");
            }

            try
            {
                await _categoryRepository.UpdateAsync(category);
                _notyfService.Success("Categoria atualizada com sucesso.");
            }
            catch (Exception ex) 
            {
                _notyfService.Error("Ocorreu um erro ao atualizar a categoria: " + ex.Message);
            }
            
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            Category category = await _categoryRepository.GetCategoryAsync(id);
            if (category == null)
            {
                return Json(new { message = "Categoria não encontrada!" });
            }

            return PartialView("_Delete", category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            { 
                await _categoryRepository.DeleteAsync(id);
                _notyfService.Success("Categoria removida com sucesso.");
            }
            catch (Exception ex) 
            {
                _notyfService.Error("Ocorreu um erro ao remover a categoria: " + ex.Message);
            }

            return RedirectToAction("Index");
        }
    }
}
