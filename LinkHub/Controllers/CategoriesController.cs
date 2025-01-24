using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using LinkHub.Models;
using LinkHub.Repositories;
using LinkHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace LinkHub.Controllers
{
    [Authorize(Roles = "Admin, Editor")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IPageRepository _pageRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotyfService _notyfService;
        private readonly IMapper _mapper;

        public CategoriesController(ICategoryRepository categoryRepository,
            IPageRepository pageRepository,
            UserManager<ApplicationUser> userManager,
            INotyfService notyfService,
            IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _pageRepository = pageRepository;
            _userManager = userManager;
            _notyfService = notyfService;
            _mapper = mapper;
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

            List<Category> categories = await _categoryRepository.GetCategoriesPerUserAsync(userId);
            List<Page> pages = await _pageRepository.GetPagePerUserAsync(userId);

            var categoryIndexViewModel = new CategoryIndexViewModel
            {
                Categories = categories,
                Pages = pages
            };

            return View(categoryIndexViewModel);
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

            if (string.IsNullOrEmpty(categoryView.Name))
            {
                _notyfService.Warning("O nome da categoria é obrigatório.");
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
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

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
        public async Task<IActionResult> Edit(CategoryViewModel categoryViewModel)
        {
            if (!ModelState.IsValid)
            {
                _notyfService.Warning("Preencha todos os campos obrigatórios.");
                return RedirectToAction("Index");
            }

            try
            {
                var category = _mapper.Map<Category>(categoryViewModel);

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
            var category = await _categoryRepository.GetCategoryAsync(id);
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
                Log.Error(ex, "Erro ao tentar remover a categoria com id {CategoryId}", id);
            }

            return RedirectToAction("Index");
        }
    }
}